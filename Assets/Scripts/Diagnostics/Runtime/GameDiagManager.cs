using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Miner.Diagnostics
{
    public class GameDiagManager : MonoBehaviour
    {
        public static GameDiagManager Instance { get; private set; }

        public static bool IsReady => Instance != null && Instance._initialized;

        public string SessionId => _sessionInfo != null ? _sessionInfo.sessionId : string.Empty;
        public string SessionFolderPath => _sessionFolderPath;
        public string EventsFilePath => _eventsFilePath;
        public string SummaryFilePath => _summaryFilePath;
        public HeroDiagSnapshot LatestSnapshot => _latestSnapshot;

        [SerializeField] private bool echoDiagEventsToConsole = true;
        [SerializeField] private bool captureUnityLogs = true;
        [SerializeField] private bool hudVisibleAtStart = true;
        [SerializeField] private KeyCode toggleHudKey = KeyCode.F8;

        private readonly object _ioGate = new object();
        private readonly Dictionary<string, int> _eventCounts = new Dictionary<string, int>();

        private bool _initialized;
        private bool _sessionClosed;
        private HeroDiagSnapshot _latestSnapshot = HeroDiagSnapshot.Empty();
        private DiagSessionInfo _sessionInfo;
        private string _rootFolderPath;
        private string _sessionFolderPath;
        private string _eventsFilePath;
        private string _summaryFilePath;
        private bool _hudVisible;

        public static GameDiagManager EnsureExists()
        {
            if (Instance != null)
            {
                return Instance;
            }

            GameObject go = new GameObject("GameDiagManager");
            Instance = go.AddComponent<GameDiagManager>();
            DontDestroyOnLoad(go);
            return Instance;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (!_initialized)
            {
                InitializeSession();
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            Application.logMessageReceived += OnUnityLogMessageReceived;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Application.logMessageReceived -= OnUnityLogMessageReceived;
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleHudKey))
            {
                _hudVisible = !_hudVisible;
            }
        }

        private void OnApplicationQuit()
        {
            CloseSession();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                CloseSession();
                Instance = null;
            }
        }

        public void SetHeroSnapshot(HeroDiagSnapshot snapshot)
        {
            _latestSnapshot = snapshot ?? HeroDiagSnapshot.Empty();
            if (string.IsNullOrWhiteSpace(_latestSnapshot.scene))
            {
                _latestSnapshot.scene = SceneManager.GetActiveScene().name;
            }
        }

        public void Emit(DiagChannel channel, DiagSeverity severity, string eventName, string message, string dataJson, string stackTrace, UnityEngine.Object context)
        {
            if (!_initialized)
            {
                InitializeSession();
            }

            string sceneName = SceneManager.GetActiveScene().name;
            DiagRecord record = new DiagRecord
            {
                sessionId = _sessionInfo.sessionId,
                projectName = Application.productName,
                unityVersion = Application.unityVersion,
                platform = Application.platform.ToString(),
                buildType = BuildTypeLabel(),
                scene = sceneName,
                utcUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                utcIso = DateTime.UtcNow.ToString("O"),
                realtimeSinceStartup = Time.realtimeSinceStartup,
                timeSinceLevelLoad = Time.timeSinceLevelLoad,
                frame = Time.frameCount,
                channel = channel.ToString(),
                severity = severity.ToString(),
                eventName = string.IsNullOrWhiteSpace(eventName) ? "UnnamedEvent" : eventName,
                message = message ?? string.Empty,
                dataJson = dataJson ?? string.Empty,
                stackTrace = stackTrace ?? string.Empty,
                hero = CloneSnapshot(_latestSnapshot, sceneName)
            };

            string line = JsonUtility.ToJson(record);

            lock (_ioGate)
            {
                File.AppendAllText(_eventsFilePath, line + Environment.NewLine, Encoding.UTF8);

                string key = record.channel + "/" + record.eventName;
                if (_eventCounts.ContainsKey(key))
                {
                    _eventCounts[key]++;
                }
                else
                {
                    _eventCounts[key] = 1;
                }
            }

            if (echoDiagEventsToConsole)
            {
                string prefix = $"[DIAG][{record.channel}][{record.eventName}]";
                if (severity == DiagSeverity.Error)
                {
                    Debug.LogError($"{prefix} {record.message}", context);
                }
                else if (severity == DiagSeverity.Warning)
                {
                    Debug.LogWarning($"{prefix} {record.message}", context);
                }
                else
                {
                    Debug.Log($"{prefix} {record.message}", context);
                }
            }
        }

        public bool IsHudVisible()
        {
            return _hudVisible;
        }

        private void InitializeSession()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            _hudVisible = hudVisibleAtStart;

            string root = Path.Combine(Application.persistentDataPath, "DiagnosticsSessions");
            Directory.CreateDirectory(root);

            string sessionFolderName = "session-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 6);
            string sessionFolderPath = Path.Combine(root, sessionFolderName);
            Directory.CreateDirectory(sessionFolderPath);

            _rootFolderPath = root;
            _sessionFolderPath = sessionFolderPath;
            _eventsFilePath = Path.Combine(sessionFolderPath, "events.jsonl");
            _summaryFilePath = Path.Combine(sessionFolderPath, "summary.txt");

            _sessionInfo = new DiagSessionInfo
            {
                sessionId = Guid.NewGuid().ToString("N"),
                projectName = Application.productName,
                companyName = Application.companyName,
                productName = Application.productName,
                unityVersion = Application.unityVersion,
                platform = Application.platform.ToString(),
                appVersion = Application.version,
                buildType = BuildTypeLabel(),
                startedUtc = DateTime.UtcNow.ToString("O"),
                endedUtc = string.Empty,
                sessionFolderName = sessionFolderName,
                sessionFolderPath = sessionFolderPath,
                eventsFilePath = _eventsFilePath,
                summaryFilePath = _summaryFilePath
            };

            File.WriteAllText(Path.Combine(sessionFolderPath, "session.json"), JsonUtility.ToJson(_sessionInfo, true), Encoding.UTF8);
            File.WriteAllText(_eventsFilePath, string.Empty, Encoding.UTF8);
            File.WriteAllText(Path.Combine(root, "latest-session-path.txt"), sessionFolderPath, Encoding.UTF8);

            Emit(DiagChannel.General, DiagSeverity.Info, "SessionStarted", "Diagnostics session started.", string.Empty, string.Empty, this);
        }

        private void CloseSession()
        {
            if (!_initialized || _sessionClosed)
            {
                return;
            }

            _sessionClosed = true;
            _sessionInfo.endedUtc = DateTime.UtcNow.ToString("O");
            File.WriteAllText(Path.Combine(_sessionFolderPath, "session.json"), JsonUtility.ToJson(_sessionInfo, true), Encoding.UTF8);
            File.WriteAllText(_summaryFilePath, BuildSummaryText(), Encoding.UTF8);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Emit(DiagChannel.General, DiagSeverity.Info, "SceneLoaded", $"Scene loaded: {scene.name}", string.Empty, string.Empty, this);
        }

        private void OnUnityLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (!_initialized || !captureUnityLogs)
            {
                return;
            }

            if (!string.IsNullOrEmpty(condition) && condition.StartsWith("[DIAG]"))
            {
                return;
            }

            DiagSeverity severity = DiagSeverity.Info;
            if (type == LogType.Warning)
            {
                severity = DiagSeverity.Warning;
            }
            else if (type == LogType.Assert || type == LogType.Error || type == LogType.Exception)
            {
                severity = DiagSeverity.Error;
            }

            string safeCondition = condition ?? string.Empty;
            string safeStack = stackTrace ?? string.Empty;

            DiagRecord record = new DiagRecord
            {
                sessionId = _sessionInfo.sessionId,
                projectName = Application.productName,
                unityVersion = Application.unityVersion,
                platform = Application.platform.ToString(),
                buildType = BuildTypeLabel(),
                scene = SceneManager.GetActiveScene().name,
                utcUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                utcIso = DateTime.UtcNow.ToString("O"),
                realtimeSinceStartup = Time.realtimeSinceStartup,
                timeSinceLevelLoad = Time.timeSinceLevelLoad,
                frame = Time.frameCount,
                channel = DiagChannel.Unity.ToString(),
                severity = severity.ToString(),
                eventName = type.ToString(),
                message = safeCondition,
                dataJson = string.Empty,
                stackTrace = safeStack,
                hero = CloneSnapshot(_latestSnapshot, SceneManager.GetActiveScene().name)
            };

            string line = JsonUtility.ToJson(record);

            lock (_ioGate)
            {
                File.AppendAllText(_eventsFilePath, line + Environment.NewLine, Encoding.UTF8);

                string key = record.channel + "/" + record.eventName;
                if (_eventCounts.ContainsKey(key))
                {
                    _eventCounts[key]++;
                }
                else
                {
                    _eventCounts[key] = 1;
                }
            }
        }

        private string BuildSummaryText()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Miner Diagnostics Summary");
            sb.AppendLine("========================");
            sb.AppendLine($"SessionId: {_sessionInfo.sessionId}");
            sb.AppendLine($"Project: {_sessionInfo.projectName}");
            sb.AppendLine($"Unity: {_sessionInfo.unityVersion}");
            sb.AppendLine($"Platform: {_sessionInfo.platform}");
            sb.AppendLine($"BuildType: {_sessionInfo.buildType}");
            sb.AppendLine($"StartedUtc: {_sessionInfo.startedUtc}");
            sb.AppendLine($"EndedUtc: {_sessionInfo.endedUtc}");
            sb.AppendLine($"SessionFolder: {_sessionFolderPath}");
            sb.AppendLine($"EventsFile: {_eventsFilePath}");
            sb.AppendLine();

            sb.AppendLine("Latest Hero Snapshot");
            sb.AppendLine("--------------------");
            sb.AppendLine($"State: {_latestSnapshot.state}");
            sb.AppendLine($"Scene: {_latestSnapshot.scene}");
            sb.AppendLine($"Position: {_latestSnapshot.position}");
            sb.AppendLine($"Velocity: {_latestSnapshot.velocity}");
            sb.AppendLine($"Input: {_latestSnapshot.input}");
            sb.AppendLine($"Grounded: {_latestSnapshot.grounded}");
            sb.AppendLine($"TouchingLadder: {_latestSnapshot.touchingLadder}");
            sb.AppendLine($"Climbing: {_latestSnapshot.climbing}");
            sb.AppendLine($"RecentlyGrounded: {_latestSnapshot.recentlyGrounded}");
            sb.AppendLine($"RecentlyTouchedLadder: {_latestSnapshot.recentlyTouchedLadder}");
            sb.AppendLine($"TopExitLocked: {_latestSnapshot.topExitLocked}");
            sb.AppendLine($"ClimbStartLocked: {_latestSnapshot.climbStartLocked}");
            sb.AppendLine($"GravityScale: {_latestSnapshot.gravityScale}");
            sb.AppendLine();

            sb.AppendLine("Event Counts");
            sb.AppendLine("------------");
            foreach (KeyValuePair<string, int> pair in _eventCounts)
            {
                sb.AppendLine($"{pair.Key}: {pair.Value}");
            }

            sb.AppendLine();
            sb.AppendLine("How to share");
            sb.AppendLine("------------");
            sb.AppendLine("Zip the whole session folder and send it together with reproduction steps.");

            return sb.ToString();
        }

        private static HeroDiagSnapshot CloneSnapshot(HeroDiagSnapshot source, string fallbackScene)
        {
            if (source == null)
            {
                HeroDiagSnapshot empty = HeroDiagSnapshot.Empty();
                empty.scene = fallbackScene;
                return empty;
            }

            return new HeroDiagSnapshot
            {
                state = source.state,
                scene = string.IsNullOrWhiteSpace(source.scene) ? fallbackScene : source.scene,
                position = source.position,
                velocity = source.velocity,
                input = source.input,
                grounded = source.grounded,
                touchingLadder = source.touchingLadder,
                climbing = source.climbing,
                recentlyGrounded = source.recentlyGrounded,
                recentlyTouchedLadder = source.recentlyTouchedLadder,
                topExitLocked = source.topExitLocked,
                climbStartLocked = source.climbStartLocked,
                gravityScale = source.gravityScale
            };
        }

        private static string BuildTypeLabel()
        {
#if UNITY_EDITOR
            return "Editor";
#elif DEVELOPMENT_BUILD
            return "DevelopmentBuild";
#else
            return "ReleaseBuild";
#endif
        }
    }
}

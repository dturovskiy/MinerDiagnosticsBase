using System;

namespace Miner.Diagnostics
{
    [Serializable]
    public class DiagRecord
    {
        public string sessionId;
        public string projectName;
        public string unityVersion;
        public string platform;
        public string buildType;
        public string scene;

        public long utcUnixMs;
        public string utcIso;
        public float realtimeSinceStartup;
        public float timeSinceLevelLoad;
        public int frame;

        public string channel;
        public string severity;
        public string eventName;
        public string message;
        public string dataJson;
        public string stackTrace;

        public HeroDiagSnapshot hero;
    }
}

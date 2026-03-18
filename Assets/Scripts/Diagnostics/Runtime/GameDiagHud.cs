using UnityEngine;

namespace Miner.Diagnostics
{
    public class GameDiagHud : MonoBehaviour
    {
        [SerializeField] private Rect panelRect = new Rect(12f, 12f, 430f, 250f);

        private GUIStyle _labelStyle;
        private GUIStyle _boxStyle;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnGUI()
        {
            GameDiagManager manager = GameDiagManager.Instance;
            if (manager == null || !manager.IsHudVisible())
            {
                return;
            }

            EnsureStyles();

            GUILayout.BeginArea(panelRect, _boxStyle);
            GUILayout.Label("Miner Diagnostics HUD", _labelStyle);
            GUILayout.Space(4f);

            HeroDiagSnapshot hero = manager.LatestSnapshot;
            GUILayout.Label("F8 - show/hide HUD", _labelStyle);
            GUILayout.Label("Session: " + manager.SessionId, _labelStyle);
            GUILayout.Label("State: " + hero.state, _labelStyle);
            GUILayout.Label("Scene: " + hero.scene, _labelStyle);
            GUILayout.Label("Position: " + hero.position, _labelStyle);
            GUILayout.Label("Velocity: " + hero.velocity, _labelStyle);
            GUILayout.Label("Input: " + hero.input, _labelStyle);
            GUILayout.Label("Grounded: " + hero.grounded, _labelStyle);
            GUILayout.Label("TouchingLadder: " + hero.touchingLadder, _labelStyle);
            GUILayout.Label("Climbing: " + hero.climbing, _labelStyle);
            GUILayout.Label("RecentlyGrounded: " + hero.recentlyGrounded, _labelStyle);
            GUILayout.Label("RecentlyTouchedLadder: " + hero.recentlyTouchedLadder, _labelStyle);
            GUILayout.Label("TopExitLocked: " + hero.topExitLocked, _labelStyle);
            GUILayout.Label("ClimbStartLocked: " + hero.climbStartLocked, _labelStyle);
            GUILayout.Label("GravityScale: " + hero.gravityScale.ToString("0.###"), _labelStyle);

            GUILayout.EndArea();
        }

        private void EnsureStyles()
        {
            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 13,
                    wordWrap = false
                };
            }

            if (_boxStyle == null)
            {
                _boxStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(10, 10, 10, 10)
                };
            }
        }
    }
}

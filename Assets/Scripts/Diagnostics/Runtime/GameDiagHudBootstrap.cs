using UnityEngine;

namespace Miner.Diagnostics
{
    public static class GameDiagHudBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (Object.FindFirstObjectByType<GameDiagHud>() != null)
            {
                return;
            }

            GameObject go = new GameObject("GameDiagHud");
            Object.DontDestroyOnLoad(go);
            go.AddComponent<GameDiagHud>();
#endif
        }
    }
}

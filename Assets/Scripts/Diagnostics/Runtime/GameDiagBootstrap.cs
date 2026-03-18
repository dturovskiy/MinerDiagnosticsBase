using UnityEngine;

namespace Miner.Diagnostics
{
    public static class GameDiagBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            GameDiagManager.EnsureExists();
#endif
        }
    }
}

using UnityEngine;

namespace Miner.Diagnostics
{
    public class GameDiagAutoSnapshot : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour providerSource;

        private IGameDiagSnapshotProvider _provider;

        private void Awake()
        {
            if (providerSource != null)
            {
                _provider = providerSource as IGameDiagSnapshotProvider;
            }

            if (_provider == null)
            {
                _provider = GetComponent<IGameDiagSnapshotProvider>();
            }

            if (_provider == null)
            {
                Debug.LogWarning("[DIAG] GameDiagAutoSnapshot did not find IGameDiagSnapshotProvider on the same GameObject.", this);
            }
        }

        private void LateUpdate()
        {
            if (_provider == null)
            {
                return;
            }

            GameDiag.SetHeroSnapshot(_provider.BuildSnapshot());
        }
    }
}

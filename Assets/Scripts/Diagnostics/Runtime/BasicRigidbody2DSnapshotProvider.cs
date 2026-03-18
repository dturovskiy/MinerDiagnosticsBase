using UnityEngine;
using UnityEngine.SceneManagement;

namespace Miner.Diagnostics
{
    public class BasicRigidbody2DSnapshotProvider : MonoBehaviour, IGameDiagSnapshotProvider
    {
        [SerializeField] private Rigidbody2D targetRigidbody;
        [SerializeField] private string stateLabel = "Unknown";

        private void Reset()
        {
            targetRigidbody = GetComponent<Rigidbody2D>();
        }

        public void SetStateLabel(string newLabel)
        {
            stateLabel = string.IsNullOrWhiteSpace(newLabel) ? "Unknown" : newLabel;
        }

        public HeroDiagSnapshot BuildSnapshot()
        {
            Rigidbody2D rb = targetRigidbody != null ? targetRigidbody : GetComponent<Rigidbody2D>();

            return new HeroDiagSnapshot
            {
                state = stateLabel,
                scene = SceneManager.GetActiveScene().name,
                position = transform.position,
                velocity = rb != null ? rb.linearVelocity : Vector2.zero,
                input = Vector2.zero,
                grounded = false,
                touchingLadder = false,
                climbing = false,
                recentlyGrounded = false,
                recentlyTouchedLadder = false,
                topExitLocked = false,
                climbStartLocked = false,
                gravityScale = rb != null ? rb.gravityScale : 0f
            };
        }
    }
}

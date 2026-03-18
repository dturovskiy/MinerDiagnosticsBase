using System;
using UnityEngine;

namespace Miner.Diagnostics
{
    [Serializable]
    public class HeroDiagSnapshot
    {
        public string state;
        public string scene;
        public Vector2 position;
        public Vector2 velocity;
        public Vector2 input;
        public bool grounded;
        public bool touchingLadder;
        public bool climbing;
        public bool recentlyGrounded;
        public bool recentlyTouchedLadder;
        public bool topExitLocked;
        public bool climbStartLocked;
        public float gravityScale;

        public static HeroDiagSnapshot Empty()
        {
            return new HeroDiagSnapshot
            {
                state = string.Empty,
                scene = string.Empty,
                position = Vector2.zero,
                velocity = Vector2.zero,
                input = Vector2.zero,
                grounded = false,
                touchingLadder = false,
                climbing = false,
                recentlyGrounded = false,
                recentlyTouchedLadder = false,
                topExitLocked = false,
                climbStartLocked = false,
                gravityScale = 0f
            };
        }
    }
}

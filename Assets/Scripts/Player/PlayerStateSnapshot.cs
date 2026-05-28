using UnityEngine;

namespace CyberpixelOk.Player
{
    public struct PlayerStateSnapshot
    {
        public Vector2 MoveInput;
        public Vector2 LookInput;
        public bool IsGrounded;
        public bool IsJetpacking;
        public bool IsRunning;
        public bool IsDead;
        public bool IsHurt;
        public bool IsShooting;
        public bool FacingRight;
        public float HorizontalSpeed;
        public float VerticalSpeed;
    }
}

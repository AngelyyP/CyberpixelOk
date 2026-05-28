using UnityEngine;

namespace CyberpixelOk.Player
{
    [CreateAssetMenu(menuName = "CyberpixelOk/Player/Movement Settings", fileName = "PlayerMovementSettings")]
    public class PlayerMovementSettings : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField] private float maxMoveSpeed = 7f;
        [SerializeField] private float maxRunSpeed = 10f;
        [SerializeField] private float groundAcceleration = 70f;
        [SerializeField] private float airAcceleration = 35f;

        [Header("Jump")]
        [SerializeField] private float jumpForce = 14f;
        [SerializeField] private float coyoteTime = 0.12f;
        [SerializeField] private float jumpBufferTime = 0.12f;
        [SerializeField] private float fallGravityMultiplier = 2.5f;
        [SerializeField] private float maxFallSpeed = 22f;

        [Header("Ground Check")]
        [SerializeField] private float groundCheckRadius = 0.12f;

        public float MaxMoveSpeed => maxMoveSpeed;
        public float MaxRunSpeed => maxRunSpeed;
        public float GroundAcceleration => groundAcceleration;
        public float AirAcceleration => airAcceleration;
        public float JumpForce => jumpForce;
        public float CoyoteTime => coyoteTime;
        public float JumpBufferTime => jumpBufferTime;
        public float FallGravityMultiplier => fallGravityMultiplier;
        public float MaxFallSpeed => maxFallSpeed;
        public float GroundCheckRadius => groundCheckRadius;
    }
}

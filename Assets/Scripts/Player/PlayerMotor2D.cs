using System;
using UnityEngine;

namespace CyberpixelOk.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMotor2D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayers = ~0;
        [SerializeField] private PlayerMovementSettings movementSettings;
        [SerializeField] private PlayerJetpackSettings jetpackSettings;

        public event Action<bool> GroundedChanged;
        public event Action<bool> JetpackStateChanged;
        public event Action<float, float> FuelChanged;

        public bool IsGrounded { get; private set; }
        public bool IsJetpacking { get; private set; }
        public bool FacingRight { get; private set; } = true;
        public float CurrentFuel { get; private set; }
        public float MaxFuel => jetpackSettings != null ? jetpackSettings.MaxFuel : 0f;
        public float HorizontalSpeed => body != null ? body.linearVelocity.x : 0f;
        public float VerticalSpeed => body != null ? body.linearVelocity.y : 0f;

        private Vector2 moveInput;
        private bool jumpQueued;
        private bool jetpackHeld;
        private float coyoteTimer;
        private float jumpBufferTimer;

        private void Reset()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void Awake()
        {
            if (body == null)
            {
                body = GetComponent<Rigidbody2D>();
            }

            if (jetpackSettings != null && CurrentFuel <= 0f)
            {
                CurrentFuel = jetpackSettings.MaxFuel;
            }
        }

        private void Update()
        {
            if (movementSettings == null || body == null)
            {
                return;
            }

            UpdateGroundedState();
            UpdateFacingDirection();
            UpdateTimers(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (movementSettings == null || body == null)
            {
                return;
            }

            ApplyHorizontalMovement();
            ApplyVerticalMovement();
        }

        public void SetInput(Vector2 newMoveInput, bool jumpPressed, bool newJetpackHeld)
        {
            moveInput = newMoveInput;
            jetpackHeld = newJetpackHeld;

            if (jumpPressed)
            {
                jumpQueued = true;
                jumpBufferTimer = movementSettings != null ? movementSettings.JumpBufferTime : 0.12f;
            }
        }

        public void SetFuel(float fuelValue)
        {
            if (jetpackSettings == null)
            {
                CurrentFuel = Mathf.Max(0f, fuelValue);
            }
            else
            {
                CurrentFuel = Mathf.Clamp(fuelValue, 0f, jetpackSettings.MaxFuel);
            }

            FuelChanged?.Invoke(CurrentFuel, MaxFuel);
        }

        public void RestoreFuelToMax()
        {
            if (jetpackSettings == null)
            {
                CurrentFuel = 0f;
            }
            else
            {
                CurrentFuel = jetpackSettings.MaxFuel;
            }

            FuelChanged?.Invoke(CurrentFuel, MaxFuel);
        }

        private void UpdateGroundedState()
        {
            Vector2 origin = groundCheck != null ? groundCheck.position : transform.position;
            bool grounded = Physics2D.OverlapCircle(origin, movementSettings.GroundCheckRadius, groundLayers) != null;

            if (grounded != IsGrounded)
            {
                IsGrounded = grounded;
                GroundedChanged?.Invoke(IsGrounded);
            }

            if (IsGrounded)
            {
                coyoteTimer = movementSettings.CoyoteTime;
            }
        }

        private void UpdateFacingDirection()
        {
            if (Mathf.Abs(moveInput.x) < 0.01f)
            {
                return;
            }

            FacingRight = moveInput.x > 0f;
        }

        private void UpdateTimers(float deltaTime)
        {
            if (coyoteTimer > 0f)
            {
                coyoteTimer -= deltaTime;
            }

            if (jumpBufferTimer > 0f)
            {
                jumpBufferTimer -= deltaTime;
            }
        }

        private void ApplyHorizontalMovement()
        {
            float targetSpeed = moveInput.x * movementSettings.MaxMoveSpeed;
            float acceleration = IsGrounded ? movementSettings.GroundAcceleration : movementSettings.AirAcceleration;
            float newVelocityX = Mathf.MoveTowards(body.linearVelocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);

            body.linearVelocity = new Vector2(newVelocityX, body.linearVelocity.y);
        }

        private void ApplyVerticalMovement()
        {
            Vector2 velocity = body.linearVelocity;

            if (jumpQueued && coyoteTimer > 0f)
            {
                velocity.y = movementSettings.JumpForce;
                jumpQueued = false;
                jumpBufferTimer = 0f;
                coyoteTimer = 0f;
            }

            bool canUseJetpack = jetpackHeld && jetpackSettings != null && CurrentFuel > 0f;
            if (canUseJetpack)
            {
                if (!IsJetpacking)
                {
                    IsJetpacking = true;
                    JetpackStateChanged?.Invoke(true);
                }

                body.gravityScale = jetpackSettings.GravityScaleWhileJetpacking;
                velocity.y = Mathf.MoveTowards(velocity.y, jetpackSettings.MaxAscendSpeed, jetpackSettings.JetpackAcceleration * Time.fixedDeltaTime);
                CurrentFuel = Mathf.Max(0f, CurrentFuel - jetpackSettings.FuelDrainPerSecond * Time.fixedDeltaTime);
                FuelChanged?.Invoke(CurrentFuel, MaxFuel);
            }
            else
            {
                if (IsJetpacking)
                {
                    IsJetpacking = false;
                    JetpackStateChanged?.Invoke(false);
                }

                body.gravityScale = 1f;

                if (velocity.y < 0f)
                {
                    velocity.y += Physics2D.gravity.y * (movementSettings.FallGravityMultiplier - 1f) * Time.fixedDeltaTime;
                    velocity.y = Mathf.Max(velocity.y, -movementSettings.MaxFallSpeed);
                }

                if (jetpackSettings != null)
                {
                    bool canRecharge = !jetpackSettings.RechargeOnlyOnGround || IsGrounded;
                    if (canRecharge && CurrentFuel < jetpackSettings.MaxFuel)
                    {
                        CurrentFuel = Mathf.Min(jetpackSettings.MaxFuel, CurrentFuel + jetpackSettings.FuelRechargePerSecond * Time.fixedDeltaTime);
                        FuelChanged?.Invoke(CurrentFuel, MaxFuel);
                    }
                }
            }

            if (IsGrounded && velocity.y < 0f)
            {
                velocity.y = 0f;
            }

            body.linearVelocity = velocity;
        }
    }
}

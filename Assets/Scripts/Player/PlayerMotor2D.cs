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
        [SerializeField] private Collider2D[] playerColliders;
        [SerializeField] private LayerMask groundLayers = ~0;
        [SerializeField] private PlayerMovementSettings movementSettings;
        [SerializeField] private PlayerJetpackSettings jetpackSettings;

        [Header("Fallback Movement")]
        [SerializeField] private float fallbackMaxMoveSpeed = 7f;
        [SerializeField] private float fallbackMaxRunSpeed = 10f;
        [SerializeField] private float fallbackGroundAcceleration = 70f;
        [SerializeField] private float fallbackAirAcceleration = 35f;
        [SerializeField] private float fallbackJumpForce = 14f;
        [SerializeField] private float fallbackCoyoteTime = 0.12f;
        [SerializeField] private float fallbackJumpBufferTime = 0.12f;
        [SerializeField] private float fallbackFallGravityMultiplier = 2.5f;
        [SerializeField] private float fallbackMaxFallSpeed = 22f;
        [SerializeField] private float fallbackGroundCheckRadius = 0.12f;

        public event Action<bool> GroundedChanged;
        public event Action<bool> JetpackStateChanged;
        public event Action<float, float> FuelChanged;

        public bool IsGrounded { get; private set; }
        public bool IsJetpacking { get; private set; }
        public bool IsRunning { get; private set; }
        public bool FacingRight { get; private set; } = true;
        public float CurrentFuel { get; private set; }
        public float MaxFuel => jetpackSettings != null ? jetpackSettings.MaxFuel : 0f;
        public float HorizontalSpeed => body != null ? body.linearVelocity.x : 0f;
        public float VerticalSpeed => body != null ? body.linearVelocity.y : 0f;

        private Vector2 moveInput;
        private bool jumpQueued;
        private bool previousJumpHeld;
        private bool jetpackHeld;
        private bool runHeld;
        private float coyoteTimer;
        private float jumpBufferTimer;
        private PhysicsMaterial2D noFrictionMaterial;

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

            if (playerColliders == null || playerColliders.Length == 0)
            {
                playerColliders = GetComponentsInChildren<Collider2D>(true);
            }

            ConfigurePhysics();

            if (jetpackSettings != null && CurrentFuel <= 0f)
            {
                CurrentFuel = jetpackSettings.MaxFuel;
            }
        }

        private void Update()
        {
            if (body == null)
            {
                return;
            }

            UpdateGroundedState();
            UpdateFacingDirection();
            UpdateTimers(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (body == null)
            {
                return;
            }

            ApplyHorizontalMovement();
            ApplyVerticalMovement();
        }

        public void SetInput(Vector2 newMoveInput, bool jumpHeld, bool newJetpackHeld, bool newRunHeld)
        {
            moveInput = newMoveInput;
            jetpackHeld = newJetpackHeld;
            runHeld = newRunHeld;
            IsRunning = runHeld && Mathf.Abs(moveInput.x) > 0.01f;

            if (jumpHeld && !previousJumpHeld)
            {
                jumpQueued = true;
                jumpBufferTimer = GetJumpBufferTime();
            }

            previousJumpHeld = jumpHeld;
        }

        public void QueueJump()
        {
            jumpQueued = true;
            jumpBufferTimer = GetJumpBufferTime();
        }

        public void RequestJump()
        {
            if (body == null)
            {
                return;
            }

            if (IsGrounded || coyoteTimer > 0f)
            {
                PerformJump();
                return;
            }

            QueueJump();
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
            Vector2 origin = GetGroundCheckOrigin();
            float radius = GetGroundCheckRadius();
            Collider2D hit = Physics2D.OverlapCircle(origin, radius, groundLayers);
            bool grounded = hit != null && !IsSelfCollider(hit);

            if (grounded != IsGrounded)
            {
                IsGrounded = grounded;
                GroundedChanged?.Invoke(IsGrounded);
            }

            if (IsGrounded)
            {
                coyoteTimer = GetCoyoteTime();
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
            float maxSpeed = IsRunning ? GetMaxRunSpeed() : GetMaxMoveSpeed();
            float targetSpeed = moveInput.x * maxSpeed;
            float acceleration = IsGrounded ? GetGroundAcceleration() : GetAirAcceleration();
            float newVelocityX = Mathf.MoveTowards(body.linearVelocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);

            body.linearVelocity = new Vector2(newVelocityX, body.linearVelocity.y);
        }

        private void ApplyVerticalMovement()
        {
            Vector2 velocity = body.linearVelocity;

            if (jumpQueued && coyoteTimer > 0f)
            {
                PerformJump();
                velocity = body.linearVelocity;
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
                    velocity.y += Physics2D.gravity.y * (GetFallGravityMultiplier() - 1f) * Time.fixedDeltaTime;
                    velocity.y = Mathf.Max(velocity.y, -GetMaxFallSpeed());
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

        private void PerformJump()
        {
            if (body == null)
            {
                return;
            }

            Vector2 velocity = body.linearVelocity;
            velocity.y = GetJumpForce();
            body.linearVelocity = velocity;

            IsGrounded = false;
            jumpQueued = false;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            body.gravityScale = 1f;

            GroundedChanged?.Invoke(false);
        }

        private float GetMaxMoveSpeed()
        {
            return movementSettings != null ? movementSettings.MaxMoveSpeed : fallbackMaxMoveSpeed;
        }

        private float GetMaxRunSpeed()
        {
            return movementSettings != null ? movementSettings.MaxRunSpeed : fallbackMaxRunSpeed;
        }

        private float GetGroundAcceleration()
        {
            return movementSettings != null ? movementSettings.GroundAcceleration : fallbackGroundAcceleration;
        }

        private float GetAirAcceleration()
        {
            return movementSettings != null ? movementSettings.AirAcceleration : fallbackAirAcceleration;
        }

        private float GetJumpForce()
        {
            return movementSettings != null ? movementSettings.JumpForce : fallbackJumpForce;
        }

        private float GetCoyoteTime()
        {
            return movementSettings != null ? movementSettings.CoyoteTime : fallbackCoyoteTime;
        }

        private float GetJumpBufferTime()
        {
            return movementSettings != null ? movementSettings.JumpBufferTime : fallbackJumpBufferTime;
        }

        private float GetFallGravityMultiplier()
        {
            return movementSettings != null ? movementSettings.FallGravityMultiplier : fallbackFallGravityMultiplier;
        }

        private float GetMaxFallSpeed()
        {
            return movementSettings != null ? movementSettings.MaxFallSpeed : fallbackMaxFallSpeed;
        }

        private float GetGroundCheckRadius()
        {
            return movementSettings != null ? movementSettings.GroundCheckRadius : fallbackGroundCheckRadius;
        }

        private Vector2 GetGroundCheckOrigin()
        {
            if (groundCheck != null)
            {
                return groundCheck.position;
            }

            if (playerColliders != null)
            {
                bool hasBounds = false;
                Bounds combinedBounds = default;

                for (int index = 0; index < playerColliders.Length; index++)
                {
                    Collider2D collider = playerColliders[index];
                    if (collider == null || collider.isTrigger)
                    {
                        continue;
                    }

                    if (!hasBounds)
                    {
                        combinedBounds = collider.bounds;
                        hasBounds = true;
                    }
                    else
                    {
                        combinedBounds.Encapsulate(collider.bounds);
                    }
                }

                if (hasBounds)
                {
                    return new Vector2(combinedBounds.center.x, combinedBounds.min.y - 0.02f);
                }
            }

            return body != null ? body.position + Vector2.down * 0.5f : (Vector2)transform.position;
        }

        private bool IsSelfCollider(Collider2D collider)
        {
            if (collider == null || playerColliders == null)
            {
                return false;
            }

            for (int index = 0; index < playerColliders.Length; index++)
            {
                if (playerColliders[index] == collider)
                {
                    return true;
                }
            }

            return false;
        }

        private void ConfigurePhysics()
        {
            if (body == null)
            {
                return;
            }

            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;

            if (noFrictionMaterial == null)
            {
                noFrictionMaterial = new PhysicsMaterial2D("Player_NoFriction")
                {
                    friction = 0f,
                    bounciness = 0f
                };
            }

            if (playerColliders == null)
            {
                return;
            }

            for (int index = 0; index < playerColliders.Length; index++)
            {
                Collider2D collider = playerColliders[index];
                if (collider != null && !collider.isTrigger)
                {
                    collider.sharedMaterial = noFrictionMaterial;
                }
            }
        }
    }
}

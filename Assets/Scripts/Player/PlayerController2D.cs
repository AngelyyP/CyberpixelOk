using CyberpixelOk.Core;
using CyberpixelOk.Inventory;
using CyberpixelOk.Managers;
using CyberpixelOk.Systems;
using UnityEngine;

namespace CyberpixelOk.Player
{
    [DisallowMultipleComponent]
    public class PlayerController2D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private PlayerMotor2D motor;
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private Collider2D[] playerColliders;
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private WeaponInventory weaponInventory;

        [Header("Jump")]
        [SerializeField] private LayerMask groundLayers = ~0;
        [SerializeField] private float jumpForce = 14f;
        [SerializeField] private float coyoteTime = 0.12f;
        [SerializeField] private float jumpBufferTime = 0.12f;
        [SerializeField] private float groundProbeDistance = 0.2f;
        [SerializeField] private float groundProbeInset = 0.02f;

        public PlayerStateSnapshot CurrentSnapshot { get; private set; }

        private bool isGrounded;
        private bool jumpQueued;
        private float coyoteTimer;
        private float jumpBufferTimer;

        private void Reset()
        {
            motor = GetComponent<PlayerMotor2D>();
            body = GetComponent<Rigidbody2D>();
            playerColliders = GetComponentsInChildren<Collider2D>(true);
            healthComponent = GetComponent<HealthComponent>();
            weaponInventory = GetComponent<WeaponInventory>();
            inputReader = FindFirstObjectByType<GameInputReader>();
        }

        private void Awake()
        {
            if (motor == null)
            {
                motor = GetComponent<PlayerMotor2D>();
            }

            if (body == null)
            {
                body = GetComponent<Rigidbody2D>();
            }

            if (playerColliders == null || playerColliders.Length == 0)
            {
                playerColliders = GetComponentsInChildren<Collider2D>(true);
            }

            if (healthComponent == null)
            {
                healthComponent = GetComponent<HealthComponent>();
            }

            if (weaponInventory == null)
            {
                weaponInventory = GetComponent<WeaponInventory>();
            }

            if (inputReader == null)
            {
                inputReader = FindFirstObjectByType<GameInputReader>();
            }
        }

        private void OnEnable()
        {
            if (inputReader != null)
            {
                inputReader.JumpPressed += HandleJumpPressed;
            }

            if (healthComponent != null)
            {
                healthComponent.Died += HandleDeath;
                healthComponent.Damaged += HandleDamaged;
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.JumpPressed -= HandleJumpPressed;
            }

            if (healthComponent != null)
            {
                healthComponent.Died -= HandleDeath;
                healthComponent.Damaged -= HandleDamaged;
            }

            SaveToSession();
        }

        private void Start()
        {
            RestoreFromSession();
        }

        private void Update()
        {
            if (inputReader == null || motor == null || healthComponent == null || body == null)
            {
                return;
            }

            UpdateGroundState();
            UpdateJumpTimers(Time.deltaTime);

            motor.SetInput(inputReader.Move, false, inputReader.JetpackHeld, inputReader.RunHeld);

            CurrentSnapshot = new PlayerStateSnapshot
            {
                MoveInput = inputReader.Move,
                LookInput = inputReader.Look,
                IsGrounded = isGrounded,
                IsJetpacking = motor.IsJetpacking,
                IsRunning = motor.IsRunning,
                IsDead = healthComponent.IsDead,
                IsHurt = false,
                IsShooting = false,
                FacingRight = motor.FacingRight,
                HorizontalSpeed = motor.HorizontalSpeed,
                VerticalSpeed = motor.VerticalSpeed
            };
        }

        public void QueueJump()
        {
            if (body == null)
            {
                return;
            }

            jumpQueued = true;
            jumpBufferTimer = jumpBufferTime;
            TryPerformJump();
        }

        private void HandleJumpPressed()
        {
            if (body == null)
            {
                return;
            }

            jumpQueued = true;
            jumpBufferTimer = jumpBufferTime;
            TryPerformJump();
        }

        public void SetFuel(float fuelAmount)
        {
            motor?.SetFuel(fuelAmount);
        }

        private void HandleDamaged(DamageInfo damageInfo)
        {
            PlayerStateSnapshot snapshot = CurrentSnapshot;
            snapshot.IsHurt = true;
            CurrentSnapshot = snapshot;
        }

        private void HandleDeath()
        {
            PlayerStateSnapshot snapshot = CurrentSnapshot;
            snapshot.IsDead = true;
            CurrentSnapshot = snapshot;
        }

        private void UpdateGroundState()
        {
            bool grounded = CheckGrounded();

            if (grounded)
            {
                coyoteTimer = coyoteTime;
            }
            else if (coyoteTimer > 0f)
            {
                coyoteTimer = Mathf.Max(0f, coyoteTimer - Time.deltaTime);
            }

            isGrounded = grounded;
        }

        private void UpdateJumpTimers(float deltaTime)
        {
            if (jumpQueued && jumpBufferTimer > 0f)
            {
                jumpBufferTimer -= deltaTime;
                if (jumpBufferTimer <= 0f)
                {
                    jumpQueued = false;
                }
            }

            if (jumpQueued)
            {
                TryPerformJump();
            }
        }

        private void TryPerformJump()
        {
            if (!jumpQueued || body == null)
            {
                return;
            }

            if (!isGrounded && coyoteTimer <= 0f)
            {
                return;
            }

            Vector2 velocity = body.linearVelocity;
            velocity.y = jumpForce;
            body.linearVelocity = velocity;

            isGrounded = false;
            coyoteTimer = 0f;
            jumpQueued = false;
            jumpBufferTimer = 0f;
        }

        private bool CheckGrounded()
        {
            Vector2 origin = GetGroundProbeOrigin();
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, Vector2.down, groundProbeDistance, groundLayers);

            for (int index = 0; index < hits.Length; index++)
            {
                Collider2D hitCollider = hits[index].collider;
                if (hitCollider != null && !IsSelfCollider(hitCollider))
                {
                    return true;
                }
            }

            return false;
        }

        private Vector2 GetGroundProbeOrigin()
        {
            if (playerColliders != null)
            {
                bool hasBounds = false;
                Bounds combinedBounds = default;

                for (int index = 0; index < playerColliders.Length; index++)
                {
                    Collider2D collider = playerColliders[index];
                    if (collider == null)
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
                    return new Vector2(combinedBounds.center.x, combinedBounds.min.y + groundProbeInset);
                }
            }

            return body != null ? body.position : (Vector2)transform.position;
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

        private void RestoreFromSession()
        {
            if (GameSessionManager.Instance == null)
            {
                return;
            }

            if (GameSessionManager.Instance.HasSavedPlayerVitals)
            {
                if (healthComponent != null)
                {
                    healthComponent.SetHealth(GameSessionManager.Instance.SavedPlayerHealth);
                }

                motor?.SetFuel(GameSessionManager.Instance.SavedJetpackFuel);
            }

            if (weaponInventory != null)
            {
                weaponInventory.RestoreFromSession(GameSessionManager.Instance);
            }
        }

        private void SaveToSession()
        {
            if (GameSessionManager.Instance == null)
            {
                return;
            }

            if (healthComponent != null && motor != null)
            {
                GameSessionManager.Instance.SavePlayerVitals(healthComponent.CurrentHealth, healthComponent.MaxHealth, motor.CurrentFuel, motor.MaxFuel);
            }

            if (weaponInventory != null)
            {
                weaponInventory.SaveToSession(GameSessionManager.Instance);
            }
        }
    }
}

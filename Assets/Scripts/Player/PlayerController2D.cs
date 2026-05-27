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
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private WeaponInventory weaponInventory;

        public PlayerStateSnapshot CurrentSnapshot { get; private set; }

        private void Reset()
        {
            motor = GetComponent<PlayerMotor2D>();
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
            if (inputReader == null || motor == null || healthComponent == null)
            {
                return;
            }

            motor.SetInput(inputReader.Move, false, inputReader.JetpackHeld);

            CurrentSnapshot = new PlayerStateSnapshot
            {
                MoveInput = inputReader.Move,
                LookInput = inputReader.Look,
                IsGrounded = motor.IsGrounded,
                IsJetpacking = motor.IsJetpacking,
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
            if (motor == null || inputReader == null)
            {
                return;
            }

            motor.SetInput(inputReader.Move, true, inputReader.JetpackHeld);
        }

        private void HandleJumpPressed()
        {
            if (motor == null || inputReader == null)
            {
                return;
            }

            motor.SetInput(inputReader.Move, true, inputReader.JetpackHeld);
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

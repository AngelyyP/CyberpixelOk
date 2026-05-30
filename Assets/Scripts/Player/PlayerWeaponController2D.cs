using CyberpixelOk.Inventory;
using CyberpixelOk.Systems;
using CyberpixelOk.Weapons;
using UnityEngine;

namespace CyberpixelOk.Player
{
    [DisallowMultipleComponent]
    public class PlayerWeaponController2D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private PlayerMotor2D motor;
        [SerializeField] private WeaponInventory weaponInventory;
        [SerializeField] private Transform firePoint;
        [SerializeField] private ProjectilePool projectilePool;

        public event System.Action<WeaponContext> WeaponFired;

        private void Reset()
        {
            inputReader = FindFirstObjectByType<GameInputReader>();
            motor = GetComponent<PlayerMotor2D>();
            weaponInventory = GetComponent<WeaponInventory>();
            ResolveFirePoint();
        }

        private void Awake()
        {
            if (inputReader == null)
            {
                inputReader = FindFirstObjectByType<GameInputReader>();
            }

            if (motor == null)
            {
                motor = GetComponent<PlayerMotor2D>();
            }

            if (weaponInventory == null)
            {
                weaponInventory = GetComponent<WeaponInventory>();
            }

            ResolveFirePoint();
        }

        private void OnEnable()
        {
            if (inputReader != null)
            {
                inputReader.AttackPressed += HandleAttackPressed;
                inputReader.NextWeaponPressed += HandleNextWeaponPressed;
                inputReader.PreviousWeaponPressed += HandlePreviousWeaponPressed;
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.AttackPressed -= HandleAttackPressed;
                inputReader.NextWeaponPressed -= HandleNextWeaponPressed;
                inputReader.PreviousWeaponPressed -= HandlePreviousWeaponPressed;
            }
        }

        public void EquipWeapon(WeaponBase weapon)
        {
            if (weaponInventory == null)
            {
                return;
            }

            weaponInventory.AddWeapon(weapon);
        }

        private void HandleAttackPressed()
        {
            TryFireCurrentWeapon();
        }

        private void HandleNextWeaponPressed()
        {
            weaponInventory?.EquipNextWeapon();
        }

        private void HandlePreviousWeaponPressed()
        {
            weaponInventory?.EquipPreviousWeapon();
        }

        private void TryFireCurrentWeapon()
        {
            if (weaponInventory == null || motor == null)
            {
                return;
            }

            Transform resolvedFirePoint = firePoint != null ? firePoint : transform;

            WeaponBase currentWeapon = weaponInventory.CurrentWeapon;
            if (currentWeapon == null)
            {
                return;
            }

            Vector2 lookInput = inputReader != null ? inputReader.Look : Vector2.zero;
            WeaponContext weaponContext = new WeaponContext(
                gameObject,
                resolvedFirePoint,
                WeaponDirectionUtility.ResolveAimDirection(lookInput, motor.FacingRight),
                lookInput,
                motor.FacingRight,
                projectilePool);

            if (weaponInventory.TryFireCurrentWeapon(weaponContext))
            {
                WeaponFired?.Invoke(weaponContext);
            }
        }

        private void ResolveFirePoint()
        {
            if (firePoint != null)
            {
                return;
            }

            Transform[] childTransforms = GetComponentsInChildren<Transform>(true);
            for (int index = 0; index < childTransforms.Length; index++)
            {
                Transform childTransform = childTransforms[index];
                if (childTransform != null && childTransform != transform && childTransform.name == "FirePoint")
                {
                    firePoint = childTransform;
                    return;
                }
            }
        }
    }
}

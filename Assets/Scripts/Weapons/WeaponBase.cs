using CyberpixelOk.Inventory;
using UnityEngine;

namespace CyberpixelOk.Weapons
{
    public abstract class WeaponBase : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string weaponName = "Weapon";

        [Header("Combat")]
        [SerializeField] private int damage = 1;
        [SerializeField] private float cooldown = 0.2f;
        [SerializeField] private bool usesAmmo = true;
        [SerializeField] private int magazineSize = 12;
        [SerializeField] private int startingReserveAmmo = 24;

        [Header("Projectile")]
        [SerializeField] private ProjectileBase projectilePrefab;
        [SerializeField] private float projectileSpeed = 18f;
        [SerializeField] private float projectileLifetime = 3f;
        [SerializeField] private Vector3 localSpawnOffset = new Vector3(0.35f, 0f, 0f);

        public string WeaponName => weaponName;
        public int Damage => damage;
        public float Cooldown => cooldown;
        public bool UsesAmmo => usesAmmo;
        public int MagazineSize => magazineSize;
        public int StartingReserveAmmo => startingReserveAmmo;
        public ProjectileBase ProjectilePrefab => projectilePrefab;
        public float ProjectileSpeed => projectileSpeed;
        public float ProjectileLifetime => projectileLifetime;
        public Vector3 LocalSpawnOffset => localSpawnOffset;

        public WeaponRuntimeState CreateRuntimeState()
        {
            return new WeaponRuntimeState
            {
                CurrentAmmo = usesAmmo ? magazineSize : int.MaxValue,
                ReserveAmmo = usesAmmo ? startingReserveAmmo : int.MaxValue,
                NextAllowedFireTime = 0f
            };
        }

        public bool TryFire(WeaponContext context, WeaponRuntimeState runtimeState)
        {
            if (context.FirePoint == null || runtimeState == null)
            {
                return false;
            }

            if (Time.time < runtimeState.NextAllowedFireTime)
            {
                return false;
            }

            if (usesAmmo && runtimeState.CurrentAmmo <= 0)
            {
                return false;
            }

            runtimeState.NextAllowedFireTime = Time.time + cooldown;

            if (usesAmmo)
            {
                runtimeState.SpendAmmo(1);
            }

            Fire(context, runtimeState);
            return true;
        }

        public void AddAmmo(WeaponRuntimeState runtimeState, int amount)
        {
            if (!usesAmmo || runtimeState == null || amount <= 0)
            {
                return;
            }

            int missingAmmo = Mathf.Max(0, magazineSize - runtimeState.CurrentAmmo);
            int ammoToLoad = Mathf.Min(amount, missingAmmo);
            runtimeState.CurrentAmmo += ammoToLoad;
            runtimeState.ReserveAmmo += Mathf.Max(0, amount - ammoToLoad);
        }

        protected abstract void Fire(WeaponContext context, WeaponRuntimeState runtimeState);
    }
}

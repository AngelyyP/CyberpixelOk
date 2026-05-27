using UnityEngine;

namespace CyberpixelOk.Weapons
{
    public readonly struct WeaponContext
    {
        public readonly GameObject Owner;
        public readonly Transform FirePoint;
        public readonly WeaponAimDirection AimDirection;
        public readonly Vector2 AimVector;
        public readonly ProjectilePool Pool;

        public WeaponContext(GameObject owner, Transform firePoint, WeaponAimDirection aimDirection, Vector2 aimVector, ProjectilePool pool)
        {
            Owner = owner;
            FirePoint = firePoint;
            AimDirection = aimDirection;
            AimVector = aimVector;
            Pool = pool;
        }
    }
}

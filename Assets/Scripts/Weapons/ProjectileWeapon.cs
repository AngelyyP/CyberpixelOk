using UnityEngine;

namespace CyberpixelOk.Weapons
{
    [CreateAssetMenu(menuName = "CyberpixelOk/Weapons/Projectile Weapon", fileName = "ProjectileWeapon")]
    public class ProjectileWeapon : WeaponBase
    {
        protected override void Fire(WeaponContext context, Inventory.WeaponRuntimeState runtimeState)
        {
            if (ProjectilePrefab == null || context.FirePoint == null)
            {
                return;
            }

            bool facingRight = context.FacingRight;
            Vector2 fireDirection = WeaponDirectionUtility.ToVector(context.AimDirection, facingRight);
            Vector3 spawnPosition = context.FirePoint.TransformPoint(LocalSpawnOffset);
            Quaternion rotation = Quaternion.identity;

            ProjectilePool pool = context.Pool != null ? context.Pool : ProjectilePool.Instance;
            ProjectileBase projectile = pool != null ? pool.Spawn(ProjectilePrefab, spawnPosition, rotation) : Object.Instantiate(ProjectilePrefab, spawnPosition, rotation);
            projectile.Initialize(context.Owner, Damage, fireDirection, ProjectileSpeed, ProjectileLifetime, pool);
        }
    }
}

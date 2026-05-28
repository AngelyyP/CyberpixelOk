using CyberpixelOk.Core;
using CyberpixelOk.Weapons;
using UnityEngine;

namespace CyberpixelOk.Enemies
{
    public class RangedEnemy : EnemyBase
    {
        [SerializeField] private ProjectileBase projectilePrefab;
        [SerializeField] private float projectileSpeed = 12f;
        [SerializeField] private float projectileLifetime = 4f;
        [SerializeField] private int projectileDamage = 1;
        [SerializeField] private Transform firePoint;

        protected override void TryAttack()
        {
            if (!CanAttackNow() || projectilePrefab == null || Target == null)
            {
                return;
            }

            ProjectilePool pool = ProjectilePool.Instance;
            Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
            Vector2 direction = (Target.position - spawnPosition).normalized;
            ProjectileBase projectile = pool != null ? pool.Spawn(projectilePrefab, spawnPosition, Quaternion.identity) : Object.Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            projectile.Initialize(gameObject, projectileDamage, direction, projectileSpeed, projectileLifetime, pool);

            RegisterAttackCooldown();
        }
    }
}

using CyberpixelOk.Core;
using UnityEngine;

namespace CyberpixelOk.Enemies
{
    public class MeleeEnemy : EnemyBase
    {
        [SerializeField] private float meleeDamage = 1f;

        protected override void TryAttack()
        {
            if (!CanAttackNow())
            {
                return;
            }

            HealthComponent targetHealth = TargetHealth;
            if (targetHealth == null)
            {
                return;
            }

            targetHealth.TakeDamage(new DamageInfo
            {
                Amount = meleeDamage,
                Source = gameObject,
                HitPoint = transform.position,
                HitDirection = (Target.position - transform.position).normalized,
                DamageType = "Melee"
            });

            RegisterAttackCooldown();
        }
    }
}

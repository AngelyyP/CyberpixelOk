using CyberpixelOk.Core;
using UnityEngine;

namespace CyberpixelOk.Pickups
{
    public class HealthPickup : PickupBase
    {
        [SerializeField] private float healAmount = 25f;

        protected override bool OnCollect(GameObject collector)
        {
            HealthComponent healthComponent = collector.GetComponentInParent<HealthComponent>();
            if (healthComponent == null || healthComponent.IsDead)
            {
                return false;
            }

            healthComponent.Heal(healAmount);
            return true;
        }
    }
}

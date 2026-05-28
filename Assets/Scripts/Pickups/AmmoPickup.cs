using CyberpixelOk.Inventory;
using UnityEngine;

namespace CyberpixelOk.Pickups
{
    public class AmmoPickup : PickupBase
    {
        [SerializeField] private int ammoAmount = 12;

        protected override bool OnCollect(GameObject collector)
        {
            WeaponInventory inventory = collector.GetComponentInParent<WeaponInventory>();
            if (inventory == null)
            {
                return false;
            }

            inventory.AddAmmoToCurrentWeapon(ammoAmount);
            return true;
        }
    }
}

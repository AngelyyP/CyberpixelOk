using CyberpixelOk.Inventory;
using CyberpixelOk.Weapons;
using UnityEngine;

namespace CyberpixelOk.Pickups
{
    public class WeaponPickup : PickupBase
    {
        [SerializeField] private WeaponBase weaponToGrant;

        protected override bool OnCollect(GameObject collector)
        {
            if (weaponToGrant == null)
            {
                return false;
            }

            WeaponInventory inventory = collector.GetComponentInParent<WeaponInventory>();
            if (inventory == null)
            {
                return false;
            }

            return inventory.AddWeapon(weaponToGrant);
        }
    }
}

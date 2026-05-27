using System;
using CyberpixelOk.Weapons;

namespace CyberpixelOk.Inventory
{
    [Serializable]
    public class WeaponLoadoutEntry
    {
        public WeaponBase Weapon;
        public WeaponRuntimeState RuntimeState;

        public WeaponLoadoutEntry(WeaponBase weapon)
        {
            Weapon = weapon;
            RuntimeState = weapon != null ? weapon.CreateRuntimeState() : new WeaponRuntimeState();
        }
    }
}

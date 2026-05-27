using System;

namespace CyberpixelOk.Inventory
{
    [Serializable]
    public class WeaponRuntimeState
    {
        public float NextAllowedFireTime;
        public int CurrentAmmo;
        public int ReserveAmmo;

        public void SpendAmmo(int amount)
        {
            CurrentAmmo = Math.Max(0, CurrentAmmo - amount);
        }

        public void AddAmmo(int amount, int magazineSize)
        {
            if (amount <= 0)
            {
                return;
            }

            int ammoToFill = Math.Min(amount, Math.Max(0, magazineSize - CurrentAmmo));
            CurrentAmmo += ammoToFill;
            ReserveAmmo += Math.Max(0, amount - ammoToFill);
        }
    }
}

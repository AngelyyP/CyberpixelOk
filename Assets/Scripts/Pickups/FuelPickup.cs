using CyberpixelOk.Player;
using UnityEngine;

namespace CyberpixelOk.Pickups
{
    public class FuelPickup : PickupBase
    {
        [SerializeField] private float fuelAmount = 25f;

        protected override bool OnCollect(GameObject collector)
        {
            PlayerMotor2D motor = collector.GetComponentInParent<PlayerMotor2D>();
            if (motor == null)
            {
                return false;
            }

            motor.SetFuel(motor.CurrentFuel + fuelAmount);
            return true;
        }
    }
}

using UnityEngine;

namespace CyberpixelOk.Player
{
    [CreateAssetMenu(menuName = "CyberpixelOk/Player/Jetpack Settings", fileName = "PlayerJetpackSettings")]
    public class PlayerJetpackSettings : ScriptableObject
    {
        [SerializeField] private float maxFuel = 100f;
        [SerializeField] private float fuelDrainPerSecond = 25f;
        [SerializeField] private float fuelRechargePerSecond = 20f;
        [SerializeField] private float jetpackAcceleration = 30f;
        [SerializeField] private float maxAscendSpeed = 12f;
        [SerializeField] private float gravityScaleWhileJetpacking = 0.5f;
        [SerializeField] private bool rechargeOnlyOnGround = false;

        public float MaxFuel => maxFuel;
        public float FuelDrainPerSecond => fuelDrainPerSecond;
        public float FuelRechargePerSecond => fuelRechargePerSecond;
        public float JetpackAcceleration => jetpackAcceleration;
        public float MaxAscendSpeed => maxAscendSpeed;
        public float GravityScaleWhileJetpacking => gravityScaleWhileJetpacking;
        public bool RechargeOnlyOnGround => rechargeOnlyOnGround;
    }
}

using System;
using UnityEngine;

namespace CyberpixelOk.Core
{
    [Serializable]
    public struct DamageInfo
    {
        public float Amount;
        public Vector3 HitPoint;
        public Vector3 HitDirection;
        public GameObject Source;
        public bool IsCritical;
        public string DamageType;

        public static DamageInfo Create(float amount, GameObject source = null)
        {
            return new DamageInfo
            {
                Amount = amount,
                Source = source,
                HitPoint = source != null ? source.transform.position : Vector3.zero,
                HitDirection = Vector3.zero,
                IsCritical = false,
                DamageType = string.Empty
            };
        }
    }
}

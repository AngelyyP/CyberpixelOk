using CyberpixelOk.Core;
using UnityEngine;

namespace CyberpixelOk.Pickups
{
    [DisallowMultipleComponent]
    public abstract class PickupBase : MonoBehaviour
    {
        [SerializeField] private bool destroyAfterPickup = true;

        protected bool DestroyAfterPickup => destroyAfterPickup;

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryCollect(other != null ? other.gameObject : null);
        }

        private void OnTriggerEnter(Collider other)
        {
            TryCollect(other != null ? other.gameObject : null);
        }

        private void TryCollect(GameObject collector)
        {
            if (collector == null)
            {
                return;
            }

            if (OnCollect(collector) && destroyAfterPickup)
            {
                Destroy(gameObject);
            }
        }

        protected abstract bool OnCollect(GameObject collector);
    }
}

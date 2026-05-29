using CyberpixelOk.Managers;
using UnityEngine;
using UnityEngine.Events;

namespace CyberpixelOk.Interactions
{
    [DisallowMultipleComponent]
    public class CollectibleInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionLabel = "Recoger";
        [SerializeField] private bool removeAfterPickup = true;
        [SerializeField] private UnityEvent onCollected;

        private bool hasBeenCollected;

        public string InteractionLabel => interactionLabel;

        public bool CanInteract(InteractorContext context)
        {
            return !hasBeenCollected;
        }

        public void Interact(InteractorContext context)
        {
            if (hasBeenCollected)
            {
                return;
            }

            hasBeenCollected = true;

            GameSessionManager sessionManager = GameSessionManager.Instance != null ? GameSessionManager.Instance : FindFirstObjectByType<GameSessionManager>();
            sessionManager?.AddCollectedCollectible(1);

            onCollected?.Invoke();

            if (removeAfterPickup)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
using CyberpixelOk.Managers;
using UnityEngine;
using UnityEngine.Events;

namespace CyberpixelOk.Interactions
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public class CollectibleInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionLabel = "Recoger";
        [SerializeField] private bool removeAfterPickup = true;
        [SerializeField] private UnityEvent onCollected;

        private bool hasBeenCollected;
        private Collider2D collectibleCollider;

        public string InteractionLabel => interactionLabel;

        private void Awake()
        {
            collectibleCollider = GetComponent<Collider2D>();
            if (collectibleCollider != null)
            {
                collectibleCollider.isTrigger = true;
            }
        }

        private void Reset()
        {
            collectibleCollider = GetComponent<Collider2D>();
            if (collectibleCollider != null)
            {
                collectibleCollider.isTrigger = true;
            }
        }

        private void OnValidate()
        {
            collectibleCollider = GetComponent<Collider2D>();
            if (collectibleCollider != null)
            {
                collectibleCollider.isTrigger = true;
            }
        }

        public bool CanInteract(InteractorContext context)
        {
            return !hasBeenCollected && isActiveAndEnabled && gameObject.activeInHierarchy;
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
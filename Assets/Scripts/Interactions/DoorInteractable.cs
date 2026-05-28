using UnityEngine;

namespace CyberpixelOk.Interactions
{
    [DisallowMultipleComponent]
    public class DoorInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionLabel = "Open Door";
        [SerializeField] private Animator doorAnimator;
        [SerializeField] private string openTrigger = "Open";
        [SerializeField] private string closeTrigger = "Close";
        [SerializeField] private bool startsOpen;

        private bool isOpen;

        public string InteractionLabel => interactionLabel;

        private void Awake()
        {
            isOpen = startsOpen;
        }

        public bool CanInteract(InteractorContext context)
        {
            return true;
        }

        public void Interact(InteractorContext context)
        {
            isOpen = !isOpen;

            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger(isOpen ? openTrigger : closeTrigger);
            }
        }
    }
}

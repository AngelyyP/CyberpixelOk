using UnityEngine;
using UnityEngine.Events;

namespace CyberpixelOk.Interactions
{
    [DisallowMultipleComponent]
    public class TerminalInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionLabel = "Use Terminal";
        [SerializeField] private UnityEvent onActivated;

        public string InteractionLabel => interactionLabel;

        public bool CanInteract(InteractorContext context)
        {
            return true;
        }

        public void Interact(InteractorContext context)
        {
            onActivated?.Invoke();
        }
    }
}

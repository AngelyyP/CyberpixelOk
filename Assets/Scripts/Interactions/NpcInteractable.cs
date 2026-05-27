using UnityEngine;

namespace CyberpixelOk.Interactions
{
    [DisallowMultipleComponent]
    public class NpcInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionLabel = "Talk";
        [SerializeField] private DialogueData dialogueData;

        public string InteractionLabel => interactionLabel;

        public bool CanInteract(InteractorContext context)
        {
            return dialogueData != null && (DialogueController.Instance == null || !DialogueController.Instance.IsDialogueActive);
        }

        public void Interact(InteractorContext context)
        {
            if (DialogueController.Instance == null || dialogueData == null)
            {
                return;
            }

            DialogueController.Instance.StartDialogue(dialogueData);
        }
    }
}

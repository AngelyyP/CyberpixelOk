using CyberpixelOk.Managers;
using UnityEngine;

namespace CyberpixelOk.Interactions
{
    [DisallowMultipleComponent]
    public class QuestGatedNpcInteractable : MonoBehaviour, IInteractable
    {
        [Header("Label")]
        [SerializeField] private string interactionLabel = "Hablar";

        [Header("Dialogue")]
        [SerializeField] private DialogueData incompleteDialogue;
        [SerializeField] private DialogueData completeDialogue;

        [Header("Transition")]
        [SerializeField] private bool switchTo3DWhenComplete = true;

        private bool waitingForDialogueEnd;

        public string InteractionLabel => interactionLabel;

        public bool CanInteract(InteractorContext context)
        {
            return DialogueController.Instance == null || !DialogueController.Instance.IsDialogueActive;
        }

        public void Interact(InteractorContext context)
        {
            DialogueController dialogueController = DialogueController.Instance;
            if (dialogueController == null)
            {
                return;
            }

            GameSessionManager sessionManager = GameSessionManager.Instance != null ? GameSessionManager.Instance : FindFirstObjectByType<GameSessionManager>();
            bool hasCompletedCollectibles = sessionManager != null && sessionManager.HasCollectedRequiredCollectibles;

            DialogueData dialogueToPlay = hasCompletedCollectibles ? completeDialogue : incompleteDialogue;
            if (dialogueToPlay == null)
            {
                return;
            }

            if (hasCompletedCollectibles && switchTo3DWhenComplete)
            {
                if (!waitingForDialogueEnd)
                {
                    waitingForDialogueEnd = true;
                    dialogueController.DialogueEnded += HandleDialogueEnded;
                }
            }

            dialogueController.StartDialogue(dialogueToPlay);

            if (!hasCompletedCollectibles || !switchTo3DWhenComplete)
            {
                return;
            }

            if (!dialogueController.IsDialogueActive)
            {
                waitingForDialogueEnd = false;
                dialogueController.DialogueEnded -= HandleDialogueEnded;
            }
        }

        private void HandleDialogueEnded()
        {
            DialogueController dialogueController = DialogueController.Instance;
            if (dialogueController != null)
            {
                dialogueController.DialogueEnded -= HandleDialogueEnded;
            }

            waitingForDialogueEnd = false;

            if (switchTo3DWhenComplete)
            {
                GameFlowManager.Instance?.StartGameplay3D();
            }
        }
    }
}
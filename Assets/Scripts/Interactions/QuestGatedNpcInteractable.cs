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
        [SerializeField] private bool loadGameplay3DWhenComplete = true;

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
                Debug.LogWarning($"{nameof(QuestGatedNpcInteractable)} on '{name}' could not find a DialogueController.", this);
                return;
            }

            GameSessionManager sessionManager = GameSessionManager.Instance != null ? GameSessionManager.Instance : FindFirstObjectByType<GameSessionManager>();
            bool hasCompletedCollectibles = sessionManager != null && sessionManager.CollectibleRequirement > 0 && sessionManager.CollectedCollectibles >= sessionManager.CollectibleRequirement;

            DialogueData dialogueToPlay = hasCompletedCollectibles ? completeDialogue : incompleteDialogue;
            if (dialogueToPlay == null)
            {
                Debug.LogWarning($"{nameof(QuestGatedNpcInteractable)} on '{name}' has no dialogue assigned for the current collectible state.", this);
                return;
            }

            Debug.Log($"{nameof(QuestGatedNpcInteractable)} on '{name}' started dialogue. Collected {sessionManager?.CollectedCollectibles ?? 0}/{sessionManager?.CollectibleRequirement ?? 0}. Completed={hasCompletedCollectibles}.", this);

            dialogueController.StartDialogue(dialogueToPlay);

            if (hasCompletedCollectibles && loadGameplay3DWhenComplete)
            {
                if (!waitingForDialogueEnd)
                {
                    waitingForDialogueEnd = true;
                    dialogueController.DialogueEnded += HandleDialogueEnded;
                }
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

            if (!loadGameplay3DWhenComplete)
            {
                return;
            }

            Debug.Log($"{nameof(QuestGatedNpcInteractable)} on '{name}' finished complete dialogue. Requesting 3D scene load.", this);
            GameFlowManager.Instance?.StartGameplay3D();
        }
    }
}
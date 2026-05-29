using CyberpixelOk.Interactions;
using TMPro;
using UnityEngine;

namespace CyberpixelOk.UI
{
    [DisallowMultipleComponent]
    public class InteractionPromptView : MonoBehaviour
    {
        [SerializeField] private PlayerInteractionDetector detector;
        [SerializeField] private TMP_Text promptText;
        [SerializeField] private GameObject root;

        private void Reset()
        {
            detector = FindFirstObjectByType<PlayerInteractionDetector>();
            promptText = GetComponentInChildren<TMP_Text>(true);
        }

        private void Awake()
        {
            if (detector == null)
            {
                detector = FindFirstObjectByType<PlayerInteractionDetector>();
            }

            if (promptText == null)
            {
                promptText = GetComponentInChildren<TMP_Text>(true);
            }
        }

        private void OnEnable()
        {
            if (detector != null)
            {
                detector.FocusedInteractableChanged += HandleFocusedInteractableChanged;
                HandleFocusedInteractableChanged(detector.CurrentInteractable);
            }
        }

        private void OnDisable()
        {
            if (detector != null)
            {
                detector.FocusedInteractableChanged -= HandleFocusedInteractableChanged;
            }
        }

        private void HandleFocusedInteractableChanged(IInteractable interactable)
        {
            bool hasInteractable = interactable != null;

            if (root != null)
            {
                root.SetActive(hasInteractable);
            }

            if (promptText != null)
            {
                promptText.text = hasInteractable ? $"[E] {interactable.InteractionLabel}" : string.Empty;
            }
        }
    }
}

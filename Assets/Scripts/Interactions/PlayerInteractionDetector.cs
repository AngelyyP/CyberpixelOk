using System;
using CyberpixelOk.Core;
using CyberpixelOk.Systems;
using UnityEngine;

namespace CyberpixelOk.Interactions
{
    [DisallowMultipleComponent]
    public class PlayerInteractionDetector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private Transform detectionOrigin;

        [Header("Detection")]
        [SerializeField] private float detectionRadius = 1.5f;
        [SerializeField] private LayerMask interactableLayers = ~0;

        public event Action<IInteractable> FocusedInteractableChanged;

        public IInteractable CurrentInteractable { get; private set; }
        public string CurrentPrompt => CurrentInteractable != null ? CurrentInteractable.InteractionLabel : string.Empty;

        private readonly Collider2D[] collider2DBuffer = new Collider2D[16];
        private readonly Collider[] collider3DBuffer = new Collider[16];
        private ContactFilter2D contactFilter2D;

        private void Reset()
        {
            inputReader = FindFirstObjectByType<GameInputReader>();
        }

        private void Awake()
        {
            if (inputReader == null)
            {
                inputReader = FindFirstObjectByType<GameInputReader>();
            }

            contactFilter2D = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = interactableLayers,
                useTriggers = true
            };
        }

        private void OnEnable()
        {
            if (inputReader != null)
            {
                inputReader.InteractPressed += HandleInteractPressed;
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.InteractPressed -= HandleInteractPressed;
            }
        }

        private void Update()
        {
            RefreshCurrentInteractable();
        }

        private void RefreshCurrentInteractable()
        {
            InteractorContext context = BuildContext();
            IInteractable bestInteractable = null;
            float bestDistance = float.MaxValue;

            Vector2 origin2D = detectionOrigin != null ? detectionOrigin.position : transform.position;
            int hitCount2D = Physics2D.OverlapCircle(origin2D, detectionRadius, contactFilter2D, collider2DBuffer);
            for (int index = 0; index < hitCount2D; index++)
            {
                Collider2D hit = collider2DBuffer[index];
                if (hit == null || !hit.gameObject.TryGetInterfaceInSelfOrParents(out IInteractable interactable) || !interactable.CanInteract(context))
                {
                    continue;
                }

                float distance = Vector2.Distance(origin2D, hit.ClosestPoint(origin2D));
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestInteractable = interactable;
                }
            }

            Vector3 origin3D = detectionOrigin != null ? detectionOrigin.position : transform.position;
            int hitCount3D = Physics.OverlapSphereNonAlloc(origin3D, detectionRadius, collider3DBuffer, interactableLayers);
            for (int index = 0; index < hitCount3D; index++)
            {
                Collider hit = collider3DBuffer[index];
                if (hit == null || !hit.gameObject.TryGetInterfaceInSelfOrParents(out IInteractable interactable) || !interactable.CanInteract(context))
                {
                    continue;
                }

                float distance = Vector3.Distance(origin3D, hit.ClosestPoint(origin3D));
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestInteractable = interactable;
                }
            }

            if (CurrentInteractable != bestInteractable)
            {
                CurrentInteractable = bestInteractable;
                FocusedInteractableChanged?.Invoke(CurrentInteractable);
            }
        }

        private void HandleInteractPressed()
        {
            if (DialogueController.Instance != null && DialogueController.Instance.IsDialogueActive)
            {
                return;
            }

            if (CurrentInteractable == null)
            {
                return;
            }

            InteractorContext context = BuildContext();
            if (CurrentInteractable.CanInteract(context))
            {
                CurrentInteractable.Interact(context);
            }
        }

        private InteractorContext BuildContext()
        {
            return new InteractorContext(gameObject, transform, inputReader);
        }
    }
}

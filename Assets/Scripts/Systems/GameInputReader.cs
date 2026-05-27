using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CyberpixelOk.Systems
{
    [DisallowMultipleComponent]
    public class GameInputReader : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private string actionMapName = "Player";

        public event Action<Vector2> MoveChanged;
        public event Action<Vector2> LookChanged;
        public event Action JumpPressed;
        public event Action JumpReleased;
        public event Action AttackPressed;
        public event Action AttackReleased;
        public event Action InteractPressed;
        public event Action NextWeaponPressed;
        public event Action PreviousWeaponPressed;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool JumpHeld { get; private set; }
        public bool AttackHeld { get; private set; }
        public bool SprintHeld { get; private set; }
        public bool CrouchHeld { get; private set; }
        public bool JetpackHeld => SprintHeld || jetpackHeld;

        private InputActionMap actionMap;
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction jumpAction;
        private InputAction attackAction;
        private InputAction interactAction;
        private InputAction nextWeaponAction;
        private InputAction previousWeaponAction;
        private InputAction sprintAction;
        private InputAction crouchAction;
        private InputAction jetpackAction;

        private bool jetpackHeld;

        private void Awake()
        {
            CacheActions();
        }

        private void OnEnable()
        {
            EnableActions();
            SubscribeToActions();
        }

        private void OnDisable()
        {
            UnsubscribeFromActions();
            DisableActions();
        }

        private void CacheActions()
        {
            if (inputActions == null)
            {
                Debug.LogWarning($"{nameof(GameInputReader)} needs an InputActionAsset assigned.", this);
                return;
            }

            actionMap = inputActions.FindActionMap(actionMapName, false);
            if (actionMap == null)
            {
                Debug.LogError($"Input action map '{actionMapName}' was not found on asset '{inputActions.name}'.", this);
                return;
            }

            moveAction = ResolveAction("Move");
            lookAction = ResolveAction("Look");
            jumpAction = ResolveAction("Jump");
            attackAction = ResolveAction("Attack");
            interactAction = ResolveAction("Interact");
            nextWeaponAction = ResolveAction("Next");
            previousWeaponAction = ResolveAction("Previous");
            sprintAction = ResolveAction("Sprint");
            crouchAction = ResolveAction("Crouch");
            jetpackAction = ResolveAction("Jetpack", "Fly");
        }

        private InputAction ResolveAction(params string[] names)
        {
            if (actionMap == null)
            {
                return null;
            }

            for (int index = 0; index < names.Length; index++)
            {
                InputAction action = actionMap.FindAction(names[index], false);
                if (action != null)
                {
                    return action;
                }
            }

            return null;
        }

        private void EnableActions()
        {
            actionMap?.Enable();
        }

        private void DisableActions()
        {
            actionMap?.Disable();
        }

        private void SubscribeToActions()
        {
            SubscribeValueAction(moveAction, HandleMovePerformed, HandleMoveCanceled);
            SubscribeValueAction(lookAction, HandleLookPerformed, HandleLookCanceled);
            SubscribeButtonAction(jumpAction, HandleJumpPerformed, HandleJumpCanceled);
            SubscribeButtonAction(attackAction, HandleAttackPerformed, HandleAttackCanceled);
            SubscribeButtonAction(interactAction, HandleInteractPerformed, null);
            SubscribeButtonAction(nextWeaponAction, HandleNextWeaponPerformed, null);
            SubscribeButtonAction(previousWeaponAction, HandlePreviousWeaponPerformed, null);
            SubscribeButtonAction(sprintAction, HandleSprintPerformed, HandleSprintCanceled);
            SubscribeButtonAction(crouchAction, HandleCrouchPerformed, HandleCrouchCanceled);
            SubscribeButtonAction(jetpackAction, HandleJetpackPerformed, HandleJetpackCanceled);
        }

        private void UnsubscribeFromActions()
        {
            UnsubscribeAction(moveAction, HandleMovePerformed, HandleMoveCanceled);
            UnsubscribeAction(lookAction, HandleLookPerformed, HandleLookCanceled);
            UnsubscribeAction(jumpAction, HandleJumpPerformed, HandleJumpCanceled);
            UnsubscribeAction(attackAction, HandleAttackPerformed, HandleAttackCanceled);
            UnsubscribeAction(interactAction, HandleInteractPerformed, null);
            UnsubscribeAction(nextWeaponAction, HandleNextWeaponPerformed, null);
            UnsubscribeAction(previousWeaponAction, HandlePreviousWeaponPerformed, null);
            UnsubscribeAction(sprintAction, HandleSprintPerformed, HandleSprintCanceled);
            UnsubscribeAction(crouchAction, HandleCrouchPerformed, HandleCrouchCanceled);
            UnsubscribeAction(jetpackAction, HandleJetpackPerformed, HandleJetpackCanceled);
        }

        private static void SubscribeValueAction(InputAction action, Action<InputAction.CallbackContext> performed, Action<InputAction.CallbackContext> canceled)
        {
            if (action == null)
            {
                return;
            }

            if (performed != null)
            {
                action.performed += performed;
            }

            if (canceled != null)
            {
                action.canceled += canceled;
            }
        }

        private static void SubscribeButtonAction(InputAction action, Action<InputAction.CallbackContext> performed, Action<InputAction.CallbackContext> canceled)
        {
            if (action == null)
            {
                return;
            }

            if (performed != null)
            {
                action.performed += performed;
            }

            if (canceled != null)
            {
                action.canceled += canceled;
            }
        }

        private static void UnsubscribeAction(InputAction action, Action<InputAction.CallbackContext> performed, Action<InputAction.CallbackContext> canceled)
        {
            if (action == null)
            {
                return;
            }

            if (performed != null)
            {
                action.performed -= performed;
            }

            if (canceled != null)
            {
                action.canceled -= canceled;
            }
        }

        private void HandleMovePerformed(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
            MoveChanged?.Invoke(Move);
        }

        private void HandleMoveCanceled(InputAction.CallbackContext context)
        {
            Move = Vector2.zero;
            MoveChanged?.Invoke(Move);
        }

        private void HandleLookPerformed(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
            LookChanged?.Invoke(Look);
        }

        private void HandleLookCanceled(InputAction.CallbackContext context)
        {
            Look = Vector2.zero;
            LookChanged?.Invoke(Look);
        }

        private void HandleJumpPerformed(InputAction.CallbackContext context)
        {
            JumpHeld = true;
            JumpPressed?.Invoke();
        }

        private void HandleJumpCanceled(InputAction.CallbackContext context)
        {
            JumpHeld = false;
            JumpReleased?.Invoke();
        }

        private void HandleAttackPerformed(InputAction.CallbackContext context)
        {
            AttackHeld = true;
            AttackPressed?.Invoke();
        }

        private void HandleAttackCanceled(InputAction.CallbackContext context)
        {
            AttackHeld = false;
            AttackReleased?.Invoke();
        }

        private void HandleInteractPerformed(InputAction.CallbackContext context)
        {
            InteractPressed?.Invoke();
        }

        private void HandleNextWeaponPerformed(InputAction.CallbackContext context)
        {
            NextWeaponPressed?.Invoke();
        }

        private void HandlePreviousWeaponPerformed(InputAction.CallbackContext context)
        {
            PreviousWeaponPressed?.Invoke();
        }

        private void HandleSprintPerformed(InputAction.CallbackContext context)
        {
            SprintHeld = true;
        }

        private void HandleSprintCanceled(InputAction.CallbackContext context)
        {
            SprintHeld = false;
        }

        private void HandleCrouchPerformed(InputAction.CallbackContext context)
        {
            CrouchHeld = true;
        }

        private void HandleCrouchCanceled(InputAction.CallbackContext context)
        {
            CrouchHeld = false;
        }

        private void HandleJetpackPerformed(InputAction.CallbackContext context)
        {
            jetpackHeld = true;
        }

        private void HandleJetpackCanceled(InputAction.CallbackContext context)
        {
            jetpackHeld = false;
        }
    }
}

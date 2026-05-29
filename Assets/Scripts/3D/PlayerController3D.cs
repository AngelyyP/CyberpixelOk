using CyberpixelOk.Interactions;
using CyberpixelOk.Systems;
using UnityEngine;

namespace CyberpixelOk.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController3D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private Rigidbody playerRigidbody;
        [SerializeField] private CapsuleCollider playerCollider;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private PlayerInteractionDetector interactionDetector;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpHeight = 1.25f;
        [SerializeField] private float lookSensitivity = 3f;
        [SerializeField] private float minPitch = -60f;
        [SerializeField] private float maxPitch = 75f;
        [SerializeField] private float groundCheckDistance = 0.2f;
        [SerializeField] private LayerMask groundLayers = ~0;
        [SerializeField] private bool lockCursorOnPlay = true;

        private Vector2 moveInput;
        private Vector2 lookInput;
        private float yaw;
        private float pitch;
        private bool jumpQueued;

        private void Reset()
        {
            playerRigidbody = GetComponent<Rigidbody>();
            playerCollider = GetComponent<CapsuleCollider>();
            inputReader = FindFirstObjectByType<GameInputReader>();

            interactionDetector = GetComponent<PlayerInteractionDetector>();

            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>(true);
            }

            if (playerCamera != null && cameraPivot == null)
            {
                cameraPivot = playerCamera.transform.parent != null ? playerCamera.transform.parent : playerCamera.transform;
            }
        }

        private void Awake()
        {
            if (playerRigidbody == null)
            {
                playerRigidbody = GetComponent<Rigidbody>();
            }

            if (playerCollider == null)
            {
                playerCollider = GetComponent<CapsuleCollider>();
            }

            if (inputReader == null)
            {
                inputReader = FindFirstObjectByType<GameInputReader>();
            }

            if (interactionDetector == null)
            {
                interactionDetector = GetComponent<PlayerInteractionDetector>();
            }

            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>(true);
            }

            if (playerCamera != null)
            {
                if (cameraPivot == null || cameraPivot == transform || !cameraPivot.IsChildOf(transform))
                {
                    cameraPivot = playerCamera.transform;
                }
            }

            if (playerRigidbody != null)
            {
                playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                playerRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            }

            yaw = transform.eulerAngles.y;
        }

        private void OnEnable()
        {
            if (inputReader != null)
            {
                inputReader.JumpPressed += HandleJumpPressed;
                moveInput = inputReader.Move;
                lookInput = inputReader.Look;
            }

            ApplyCursorState(true);
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.JumpPressed -= HandleJumpPressed;
            }

            ApplyCursorState(false);
        }

        private void Update()
        {
            if (playerRigidbody == null || inputReader == null)
            {
                return;
            }

            moveInput = inputReader.Move;
            lookInput = inputReader.Look;

            yaw += lookInput.x * lookSensitivity;
            pitch = Mathf.Clamp(pitch - lookInput.y * lookSensitivity, minPitch, maxPitch);

            if (cameraPivot != null)
            {
                cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            }
        }

        private void FixedUpdate()
        {
            if (playerRigidbody == null)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.Euler(0f, yaw, 0f);
            playerRigidbody.MoveRotation(targetRotation);

            Transform movementReference = playerCamera != null ? playerCamera.transform : transform;
            Vector3 cameraForward = Vector3.ProjectOnPlane(movementReference.forward, Vector3.up);
            Vector3 cameraRight = Vector3.ProjectOnPlane(movementReference.right, Vector3.up);

            if (cameraForward.sqrMagnitude > 0f)
            {
                cameraForward.Normalize();
            }

            if (cameraRight.sqrMagnitude > 0f)
            {
                cameraRight.Normalize();
            }

            Vector3 planarDirection = cameraRight * moveInput.x + cameraForward * moveInput.y;

            if (planarDirection.sqrMagnitude > 1f)
            {
                planarDirection.Normalize();
            }

            Vector3 velocity = playerRigidbody.linearVelocity;
            velocity.x = planarDirection.x * moveSpeed;
            velocity.z = planarDirection.z * moveSpeed;

            bool isGrounded = IsGrounded();
            if (isGrounded && velocity.y < 0f)
            {
                velocity.y = -2f;
            }

            if (jumpQueued && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
                jumpQueued = false;
            }

            playerRigidbody.linearVelocity = velocity;
        }

        private bool IsGrounded()
        {
            if (playerCollider == null)
            {
                return false;
            }

            Vector3 worldCenter = transform.TransformPoint(playerCollider.center);
            float radius = Mathf.Max(0.01f, playerCollider.radius * 0.95f);
            float halfHeight = Mathf.Max(playerCollider.height * 0.5f - playerCollider.radius, 0f);
            Vector3 castOrigin = worldCenter + Vector3.up * 0.05f;
            float castDistance = halfHeight + groundCheckDistance;

            return Physics.SphereCast(castOrigin, radius, Vector3.down, out _, castDistance, groundLayers, QueryTriggerInteraction.Ignore);
        }

        private void HandleJumpPressed()
        {
            jumpQueued = true;
        }

        private void ApplyCursorState(bool active)
        {
            if (!lockCursorOnPlay)
            {
                return;
            }

            if (active)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}

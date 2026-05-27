using CyberpixelOk.Systems;
using UnityEngine;

namespace CyberpixelOk.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController3D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Camera playerCamera;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpHeight = 1.25f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float lookSensitivity = 3f;
        [SerializeField] private float minPitch = -60f;
        [SerializeField] private float maxPitch = 75f;

        private Vector3 velocity;
        private float pitch;
        private bool jumpQueued;

        private void Reset()
        {
            characterController = GetComponent<CharacterController>();
            inputReader = FindFirstObjectByType<GameInputReader>();
        }

        private void Awake()
        {
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }

            if (inputReader == null)
            {
                inputReader = FindFirstObjectByType<GameInputReader>();
            }
        }

        private void OnEnable()
        {
            if (inputReader != null)
            {
                inputReader.JumpPressed += HandleJumpPressed;
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
            {
                inputReader.JumpPressed -= HandleJumpPressed;
            }
        }

        private void Update()
        {
            if (characterController == null || inputReader == null)
            {
                return;
            }

            Vector2 moveInput = inputReader.Move;
            Vector2 lookInput = inputReader.Look;

            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            characterController.Move(move * moveSpeed * Time.deltaTime);

            if (characterController.isGrounded && velocity.y < 0f)
            {
                velocity.y = -2f;
            }

            if (jumpQueued && characterController.isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpQueued = false;
            }

            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);

            transform.Rotate(Vector3.up * (lookInput.x * lookSensitivity));

            if (cameraPivot != null)
            {
                pitch = Mathf.Clamp(pitch - lookInput.y * lookSensitivity, minPitch, maxPitch);
                cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            }
        }

        private void HandleJumpPressed()
        {
            jumpQueued = true;
        }
    }
}

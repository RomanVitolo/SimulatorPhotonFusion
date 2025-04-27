using Core;
using Fusion;
using InputSystem;
using Network;
using Unity.Cinemachine;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkPlayer))]
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerController : NetworkBehaviour
    {
        [Header("Settings")]
        [SerializeField] private InputSettings inputSettings;

        [SerializeField] private CinemachineCamera playerCamera;

        private CharacterController characterController;
        private PlayerInputHandler inputHandler;

        private Vector3 velocity;
        private bool isGrounded;

        private Vector3 moveDirection = Vector3.zero;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            inputHandler = GetComponent<PlayerInputHandler>();
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                playerCamera.Follow = transform;
                playerCamera.LookAt = transform;
            }
            else
            {
                if (playerCamera != null)
                    playerCamera.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (!Object.HasInputAuthority) return;

            HandleMovement();
            HandleRotation();
        }

        private void HandleMovement()
        {
            Vector2 input = inputHandler.GetMovementInput();
            bool isSprinting = inputHandler.GetSprintInput();
            float targetSpeed = isSprinting ? inputSettings.sprintSpeed : inputSettings.walkSpeed;

            Vector3 horizontalMove = (transform.right * input.x + transform.forward * input.y).normalized * targetSpeed;

            isGrounded = characterController.isGrounded;

            if (isGrounded && velocity.y < 0)
                velocity.y = -2f;

            if (isGrounded && inputHandler.ConsumeJumpInput())
            {
                velocity.y = Mathf.Sqrt(inputSettings.jumpForce * -2f * inputSettings.gravity);
            }

            velocity.y += inputSettings.gravity * Time.deltaTime;

            // Combinar movimiento horizontal y vertical en un solo Vector3
            moveDirection = new Vector3(horizontalMove.x, velocity.y, horizontalMove.z);

            characterController.Move(moveDirection * Time.deltaTime);
        }

        private void HandleRotation()
        {
            if (playerCamera == null) return;

            Vector3 camForward = playerCamera.transform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            if (camForward.sqrMagnitude > 0.001f)
            {
                transform.forward = camForward;
            }
        }
    }
}

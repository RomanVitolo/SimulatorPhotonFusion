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
    [RequireComponent(typeof(NetworkTransform))]
    public class PlayerController : NetworkBehaviour
    {
        [Header("Settings")]
        [SerializeField] private InputSettings inputSettings;

        [SerializeField] private CinemachineCamera playerCamera;

        private CharacterController characterController;
        private PlayerInputHandler inputHandler;
        private NetworkPlayer networkPlayer;

        private Vector3 velocity;
        private bool isGrounded;

        private Vector3 moveDirection = Vector3.zero;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            inputHandler = GetComponent<PlayerInputHandler>();
            networkPlayer = GetComponent<NetworkPlayer>();
        }

        public override void FixedUpdateNetwork() => HandleMovement();

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

            moveDirection = new Vector3(horizontalMove.x, velocity.y, horizontalMove.z);

            characterController.Move(moveDirection * Time.deltaTime);

            SyncPosition();
        }

        private void SyncPosition() => networkPlayer?.SyncMovement(transform.position);
    }
}

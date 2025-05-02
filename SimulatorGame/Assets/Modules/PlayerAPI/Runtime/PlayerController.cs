using Fusion;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : NetworkBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Core.InputSettings inputSettings;

        private CharacterController characterController;

        private Vector3 velocity;
        private bool isGrounded;

        private Vector3 moveDirection = Vector3.zero;

        private void Awake() => characterController = GetComponent<CharacterController>();

        public override void Spawned() => characterController.enabled = HasInputAuthority;

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority) return;

            if (GetInput(out NetworkInputData data)) HandleMovement(data);
        }

        private void HandleMovement(NetworkInputData data)
        {
            float targetSpeed = data.sprintPressed ? inputSettings.sprintSpeed : inputSettings.walkSpeed;

            Vector3 horizontalMove = (transform.right * data.movementInput.x + transform.forward * data.movementInput.y).normalized * targetSpeed;

            isGrounded = characterController.isGrounded;

            if (isGrounded && velocity.y < 0)
                velocity.y = -2f;

            if (isGrounded && data.jumpPressed)
                velocity.y = Mathf.Sqrt(inputSettings.jumpForce * -2f * inputSettings.gravity);

            velocity.y += inputSettings.gravity * Runner.DeltaTime;
            moveDirection = new Vector3(horizontalMove.x, velocity.y, horizontalMove.z);
            characterController.Move(moveDirection * Runner.DeltaTime);
        }
    }
}

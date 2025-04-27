using UnityEngine;
using UnityEngine.InputSystem;

namespace InputSystem
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputHandler : MonoBehaviour
    {
        private Vector2 movementInput;
        private bool jumpPressed;
        private bool sprintPressed;

        private PlayerInput playerInput;

        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction sprintAction;

        public bool ConsumeJumpInput()
        {
            if (jumpPressed)
            {
                jumpPressed = false;
                return true;
            }
            return false;
        }

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();

            moveAction = playerInput.actions["Move"];
            jumpAction = playerInput.actions["Jump"];
            sprintAction = playerInput.actions["Sprint"];

            jumpAction.performed += ctx => OnJump();
        }

        private void OnDestroy() => jumpAction.performed -= ctx => OnJump();

        private void OnJump() => jumpPressed = true;

        private void Update()
        {
            movementInput = moveAction.ReadValue<Vector2>();
            sprintPressed = sprintAction.IsPressed();
        }

        public Vector2 GetMovementInput() => movementInput;
        public bool GetSprintInput() => sprintPressed;
    }
}
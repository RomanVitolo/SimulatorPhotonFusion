using UnityEngine;

namespace InputSystem
{
    public class LocalPlayerInput : IPlayerInput
    {
        public Vector2 GetMovement() => new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        public bool JumpPressed() => Input.GetKeyDown(KeyCode.Space);

        public bool SprintHeld() => Input.GetKey(KeyCode.LeftShift);
    }
}

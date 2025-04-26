using UnityEngine;

namespace InputSystem
{
    public interface IPlayerInput
    {
        Vector2 GetMovement();
        bool JumpPressed();
        bool SprintHeld();
    }
}

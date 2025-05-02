using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector2 movementInput;
    public bool sprintPressed;
    public bool jumpPressed;
}

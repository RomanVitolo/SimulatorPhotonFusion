using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public Vector3 movementInput;
    public bool sprintPressed;
    public bool jumpPressed;
}

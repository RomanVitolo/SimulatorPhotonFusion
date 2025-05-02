using Fusion;

public class Player : NetworkBehaviour
{
    private NetworkCharacterController _cc;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.movementInput.Normalize();
            _cc.Move(5 * Runner.DeltaTime * data.movementInput);
        }
    }
}
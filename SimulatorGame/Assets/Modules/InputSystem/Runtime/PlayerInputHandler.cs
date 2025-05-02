using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputSystem
{
    public class PlayerInputHandler : NetworkBehaviour, INetworkRunnerCallbacks
    {
        private Vector2 movementInput;
        private bool jumpPressed;
        private bool sprintPressed;

        private PlayerInput playerInput;

        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction sprintAction;

        private Action<InputAction.CallbackContext> jumpCallback;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();

            moveAction = playerInput.actions["Move"];
            jumpAction = playerInput.actions["Jump"];
            sprintAction = playerInput.actions["Sprint"];

            jumpCallback = ctx => OnJump();
            jumpAction.performed += jumpCallback;
        }

        private void OnDestroy()
        {
            if (jumpCallback != null)
                jumpAction.performed -= jumpCallback;
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority) Runner.AddCallbacks(this);
            enabled = Object.HasInputAuthority;
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (!Object.HasInputAuthority) return;

            var move = GetMovementInput();
            var sprint = GetSprintInput();
            var jump = ConsumeJumpInput();

            input.Set(new NetworkInputData
            {
                movementInput = move,
                sprintPressed = sprint,
                jumpPressed = jump
            });
        }

        private void OnJump() => jumpPressed = true;

        private void Update()
        {
            movementInput = moveAction.ReadValue<Vector2>();
            sprintPressed = sprintAction.IsPressed();
        }

        public Vector2 GetMovementInput() => movementInput;
        public bool GetSprintInput() => sprintPressed;

        public bool ConsumeJumpInput()
        {
            if (jumpPressed)
            {
                jumpPressed = false;
                return true;
            }
            return false;
        }

        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    }
}
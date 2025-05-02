using Fusion;
using Fusion.Sockets;
using InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Network
{
    public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static NetworkManager Instance;

        [SerializeField] private NetworkPrefabRef playerPrefabRef;
        [SerializeField] private InputActionAsset defaultInputActions;

        private NetworkRunner runner;

        private readonly Dictionary<PlayerRef, NetworkObject> playerObjects = new();

        private string status = "Idle";
        private readonly List<string> statusMessages = new();
        private readonly float messageDisplayTime = 3f;
        private readonly Dictionary<string, float> statusTimers = new();

        private PlayerInputHandler _inputHandler;

        private void Awake() => Instance = this;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 200, 300));

            if (runner == null)
            {
                if (GUILayout.Button("Host"))
                {
                    StartGame(GameMode.Host);
                }

                if (GUILayout.Button("Join"))
                {
                    StartGame(GameMode.Client);
                }
            }

            if (runner != null && GUILayout.Button("Leave Game"))
            {
                runner.Shutdown();
            }

            GUILayout.Label("Status: " + status);

            for (int i = statusMessages.Count - 1; i >= 0; i--)
            {
                string msg = statusMessages[i];
                GUILayout.Label(msg);

                if (Time.time > statusTimers[msg])
                {
                    statusMessages.RemoveAt(i);
                    statusTimers.Remove(msg);
                }
            }

            GUILayout.EndArea();
        }

        public void RegisterInputHandler(PlayerInputHandler handler) => _inputHandler = handler;

        private async void StartGame(GameMode mode)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
            runner.ProvideInput = true;

            status = "Starting as " + mode;

            runner.name = "NetworkRunner";
            runner.ProvideInput = true;
            runner.AddCallbacks(this);

            var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
            var sceneInfo = new NetworkSceneInfo();
            if (scene.IsValid)
                sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);

            var result = await runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "TestRoom",
                Scene = scene,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });

            status = result.Ok ? "Connected as " + mode : "Failed to start: " + result.ShutdownReason;

            Debug.Log($"[StartGame] ProvideInput={runner.ProvideInput}");
        }

        private string GetFriendlyShutdownMessage(ShutdownReason reason)
        {
            return reason switch
            {
                ShutdownReason.Ok or ShutdownReason.GameClosed or ShutdownReason.DisconnectedByPluginLogic or ShutdownReason.Error => "The host ended the session.",
                ShutdownReason.IncompatibleConfiguration => "Incompatible client configuration.",
                ShutdownReason.ConnectionRefused => "Connection was refused.",
                _ => $"Disconnected: {reason}",
            };
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (!runner.IsServer) return;

            Vector3 pos = new(UnityEngine.Random.Range(-5f, 5f), 1f, UnityEngine.Random.Range(-5f, 5f));

            NetworkObject obj = runner.Spawn(playerPrefabRef, pos, Quaternion.identity, player);

            runner.SetPlayerObject(player, obj);

            playerObjects[player] = obj;
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (playerObjects.TryGetValue(player, out NetworkObject obj))
            {
                runner.Despawn(obj);
                playerObjects.Remove(player);
            }

            string message = $"Player {player.PlayerId} left the game";
            statusMessages.Add(message);
            statusTimers[message] = Time.time + messageDisplayTime;
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log("Disconnected: " + shutdownReason);

            status = GetFriendlyShutdownMessage(shutdownReason);

            Destroy(runner.gameObject);
            this.runner = null;
            playerObjects.Clear();
        }

        public void OnInput(NetworkRunner runner, NetworkInput input){ }

        // Required Fusion callbacks (even if unused)
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
    }
}
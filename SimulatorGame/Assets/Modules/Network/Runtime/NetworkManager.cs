using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private GameObject playerPrefab;

        private NetworkRunner runner;

        private readonly Dictionary<PlayerRef, NetworkObject> playerObjects = new();

        private string status = "Idle";
        private readonly List<string> statusMessages = new();
        private readonly float messageDisplayTime = 3f;
        private readonly Dictionary<string, float> statusTimers = new();

        private NetworkObject localPlayerObject;

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

            // Mostrar mensajes de estado (join/leave)
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


        private async void StartGame(GameMode mode)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
            runner.ProvideInput = true;

            status = "Starting as " + mode;

            runner.name = "NetworkRunner";
            runner.ProvideInput = true;
            runner.AddCallbacks(this);

            // Create the NetworkSceneInfo from the current scene
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
            if (runner.IsServer)
            {
                Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(-3f, 3f), 0f, UnityEngine.Random.Range(-3f, 3f));
                NetworkObject obj = runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player);
                playerObjects[player] = obj;

                if (runner.LocalPlayer == player)
                {
                    localPlayerObject = obj;
                }
            }

            string message = $"Player {player.PlayerId} joined the game";
            statusMessages.Add(message);
            statusTimers[message] = Time.time + messageDisplayTime;
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

            // Desactivar controles si el jugador sigue activo
            if (localPlayerObject != null)
            {
                if (localPlayerObject.TryGetComponent<PlayerAPI.PlayerController>(out var controller))
                    controller.enabled = false;

                if (localPlayerObject.TryGetComponent<Core.FirstPersonCameraController>(out var cameraControl))
                    cameraControl.enabled = false;
            }

            Destroy(runner.gameObject);
            this.runner = null;
            playerObjects.Clear();
            localPlayerObject = null;
        }

        // Required Fusion callbacks (even if unused)
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
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
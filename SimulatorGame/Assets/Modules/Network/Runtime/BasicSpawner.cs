using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;

    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private readonly Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    private string _status = "Idle";
    private readonly List<string> _statusMessages = new();
    private readonly float _messageDisplayTime = 3f;
    private readonly Dictionary<string, float> _statusTimers = new();

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 200, 300));

        if (_runner == null)
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

        if (_runner != null && GUILayout.Button("Leave Game"))
        {
            _runner.Shutdown();
            _runner = null;
        }

        GUILayout.Label("Status: " + _status);

        for (int i = _statusMessages.Count - 1; i >= 0; i--)
        {
            string msg = _statusMessages[i];
            GUILayout.Label(msg);

            if (Time.time > _statusTimers[msg])
            {
                _statusMessages.RemoveAt(i);
                _statusTimers.Remove(msg);
            }
        }

        GUILayout.EndArea();
    }

    private async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        _status = "Starting as " + mode;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        _status = result.Ok ? "Connected as " + mode : "Failed to start: " + result.ShutdownReason;

        Debug.Log($"[StartGame] ProvideInput={_runner.ProvideInput}");
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector3 spawnPosition = new(player.RawEncoded % runner.Config.Simulation.PlayerCount * 3, 0, -10);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            _spawnedCharacters.Add(player, networkPlayerObject);
        }

        string message = $"Player {player.PlayerId} joined the game";
        _statusMessages.Add(message);
        _statusTimers[message] = Time.time + _messageDisplayTime;
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }

        string message = $"Player {player.PlayerId} left the game";
        _statusMessages.Add(message);
        _statusTimers[message] = Time.time + _messageDisplayTime;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}
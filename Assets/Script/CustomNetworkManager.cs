using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    public static CustomNetworkManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("CustomNetworkManager singleton initialized.");
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("Duplicate CustomNetworkManager instance destroyed.");
        }
    }

    [SerializeField] private PlayerObjectController GamePlayerPrefab;
    public List<PlayerObjectController> GamePlayers { get; } = new List<PlayerObjectController>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            PlayerObjectController GamePlayerInstance = Instantiate(GamePlayerPrefab);
            GamePlayerInstance.connectionID = conn.connectionId;
            GamePlayerInstance.PlayerIdNumber = (GamePlayers.Count + 1).ToString();

            // Ensure we have valid SteamLobby.Instance and currentLobbyID
            if (SteamLobby.Instance != null)
            {
                GamePlayerInstance.PlayerSteamID = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.currentLobbyID, GamePlayers.Count);
            }
            else
            {
                Debug.LogError("SteamLobby.Instance is not initialized.");
                return;
            }

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);
        }
    }

    public void StartGame(string sceneName)
    {
        ServerChangeScene(sceneName);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance { get; private set; }

    public Text LobbyNameText;
    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    public ulong currentLobbyID;
    public bool PlayerItemCreated = false;
    private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();
    public PlayerObjectController LocalPlayerController;

    public Button startButton;
    public Text ReadyButtonText;

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ReadyPlayer()
    {
        if (LocalPlayerController != null)
        {
            LocalPlayerController.ChangeReady();
        }
    }

    public void UpdateButton()
    {
        if (LocalPlayerController != null)
        {
            ReadyButtonText.text = LocalPlayerController.Ready ? "Unready" : "Ready";
        }
    }

    public void CheckIfAllReady()
{
    bool allReady = Manager.GamePlayers.All(player => player.Ready);

    startButton.interactable = allReady && LocalPlayerController != null && int.TryParse(LocalPlayerController.PlayerIdNumber, out int playerId) && playerId == 1;
}


    public void UpdateLobbyName()
    {
        if (Manager != null && Manager.GetComponent<SteamLobby>() != null)
        {
            currentLobbyID = Manager.GetComponent<SteamLobby>().currentLobbyID;
            LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(currentLobbyID), "name");
        }
        else
        {
            Debug.LogWarning("SteamLobby component or Manager is null.");
        }
    }

    public void UpdatePlayerList()
    {
        if (!PlayerItemCreated)
        {
            CreateHostPlayerItem();
        }

        if (PlayerListItems.Count < Manager.GamePlayers.Count)
        {
            CreateClientPlayerItem();
        }

        if (PlayerListItems.Count > Manager.GamePlayers.Count)
        {
            RemovePlayerItem();
        }

        if (PlayerListItems.Count == Manager.GamePlayers.Count)
        {
            UpdatePlayerItem();
        }
    }

    public void OnPlayerJoined(PlayerObjectController player)
    {
        Debug.Log("Player joined: " + player.playerName);
        UpdatePlayerList();
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalPlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }

    public void CreateHostPlayerItem()
    {
        if (PlayerItemCreated)
            return;

        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab);

            if (NewPlayerItem != null)
            {
                PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

                if (NewPlayerItemScript.PlayerNameText == null)
                {
                    Debug.LogError("PlayerNameText tidak terassign di prefab PlayerListItem!");
                    return;
                }

                NewPlayerItemScript.PlayerNameText.text = player.playerName;
                NewPlayerItemScript.PlayerName = player.playerName;
                NewPlayerItemScript.ConnectionID = player.connectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.Ready = player.Ready;
                NewPlayerItemScript.SetPlayerValues();

                NewPlayerItem.transform.SetParent(PlayerListViewContent.transform, false);
                PlayerListItems.Add(NewPlayerItemScript);
            }
            else
            {
                Debug.LogError("Prefab gagal dibuat.");
            }
        }

        PlayerItemCreated = true;
    }

    public void CreateClientPlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            if (!PlayerListItems.Any(b => b.ConnectionID == player.connectionID))
            {
                GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab);
                PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();
                
                if (NewPlayerItemScript.PlayerNameText != null)
                {
                    NewPlayerItemScript.PlayerNameText.text = player.playerName;
                }
                else
                {
                    Debug.LogError("PlayerNameText tidak ditemukan di prefab PlayerListItem!");
                    continue;
                }
                
                NewPlayerItemScript.PlayerName = player.playerName;
                NewPlayerItemScript.ConnectionID = player.connectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.Ready = player.Ready;
                NewPlayerItemScript.SetPlayerValues();

                NewPlayerItem.transform.SetParent(PlayerListViewContent.transform, false);
                PlayerListItems.Add(NewPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (PlayerObjectController player in Manager.GamePlayers)
        {
            foreach (PlayerListItem PlayerListItemScript in PlayerListItems)
            {
                if (PlayerListItemScript.ConnectionID == player.connectionID)
                {
                    PlayerListItemScript.PlayerName = player.playerName;
                    PlayerListItemScript.Ready = player.Ready;
                    PlayerListItemScript.SetPlayerValues();

                    if (player == LocalPlayerController)
                    {
                        UpdateButton();
                    }
                }
            }
        }
        CheckIfAllReady();
    }

    public void RemovePlayerItem()
    {
        var PlayerListItemsToRemove = PlayerListItems.Where(playerListItem =>
            !Manager.GamePlayers.Any(b => b.connectionID == playerListItem.ConnectionID)).ToList();

        foreach (PlayerListItem playerListItemToRemove in PlayerListItemsToRemove)
        {
            PlayerListItems.Remove(playerListItemToRemove);
            Destroy(playerListItemToRemove.gameObject);
        }
    }

    public void StartGamne(string sceneName)
    {
        LocalPlayerController.CanStartGame(sceneName);
    
    }
}

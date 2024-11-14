using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance { get; private set; }

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;
    protected Callback<LobbyMatchList_t> LobbyList;
    protected Callback<LobbyDataUpdate_t> LobbyDataUpdate;

    public List<CSteamID> LobbyIDs = new List<CSteamID>();

    public ulong currentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private CustomNetworkManager Manager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure singleton persists across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }
    }

    private void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("SteamManager is not initialized.");
            return;
        }

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);

        // Ensure the CustomNetworkManager singleton is initialized
        if (Manager == null)
        {
            Manager = CustomNetworkManager.Instance;
            if (Manager == null)
            {
                Debug.LogError("CustomNetworkManager singleton is not set.");
                return;
            }
        }
    }

    public void HostLobby()
    {
        if (Manager == null)
        {
            Manager = CustomNetworkManager.Instance; // Ensure Manager is assigned
            if (Manager == null)
            {
                Debug.LogError("CustomNetworkManager singleton is not set.");
                return;
            }
        }

        // Create a public lobby and specify the maximum number of connections
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, Manager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult == EResult.k_EResultOK)
        {
            Debug.Log("Lobby created successfully.");
            Manager.StartHost();
            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "Name", SteamFriends.GetPersonaName() + "'s Lobby");
        }
        else
        {
            Debug.LogWarning("Failed to create lobby. Result: " + callback.m_eResult);
        }
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request to join lobby received.");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        currentLobbyID = callback.m_ulSteamIDLobby;
        Debug.Log("Lobby entered: " + currentLobbyID);

        // Clients should try to connect to the lobby
        if (NetworkServer.active)
        {
            Debug.Log("Server joined lobby");
            return;
        }

        // Ensure Manager and currentLobbyID are properly set
        if (Manager != null)
        {
            Manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
            Debug.Log("Joining lobby with network address: " + Manager.networkAddress);
            Manager.StartClient();
        }
        else
        {
            Debug.LogError("Manager is not set when entering lobby.");
        }
    }

    public void GetLobbyList()
    {
        LobbyIDs.Clear();
        SteamMatchmaking.AddRequestLobbyListResultCountFilter(60);
        SteamMatchmaking.RequestLobbyList();
    }

    public void JoinLobby(CSteamID lobbyID)
    {
        SteamMatchmaking.JoinLobby(lobbyID);
    }

    private void OnGetLobbyList(LobbyMatchList_t result)
    {
        if (LobbyListManager.Instance != null)
        {
            LobbyListManager.Instance.DestroyLobby(); // Clear previous list

            for (int i = 0; i < result.m_nLobbiesMatching; i++)
            {
                CSteamID LobbyID = SteamMatchmaking.GetLobbyByIndex(i);
                LobbyIDs.Add(LobbyID);
                SteamMatchmaking.RequestLobbyData(LobbyID);
            }
        }
        else
        {
            Debug.LogWarning("LobbyListManager.Instance is null in OnGetLobbyList.");
        }
    }
}

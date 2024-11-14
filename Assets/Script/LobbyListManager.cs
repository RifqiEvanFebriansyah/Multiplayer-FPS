using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Edgegap;

public class LobbyListManager : MonoBehaviour
{
    public static LobbyListManager Instance;

    public GameObject LobbyMenu;
    public GameObject LobbyDataItemPrefab;
    public GameObject LobbyListContent;
    public GameObject LobbyButton;
    public GameObject HostButton;

    public List<GameObject> listOfLobby = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("LobbyListManager instance created.");
            DontDestroyOnLoad(gameObject); // Uncomment if you want to persist this object across scenes
        }
        else
        {
            Destroy(gameObject); // Avoid duplicate
        }
    }

    public void GetListOfLobby()
    {
        if (LobbyButton != null) LobbyButton.SetActive(false);
        if (HostButton != null) HostButton.SetActive(false);
        if (LobbyMenu != null) LobbyMenu.SetActive(true);

        if (SteamLobby.Instance != null)
        {
            SteamLobby.Instance.GetLobbyList();
        }
        else
        {
            Debug.LogWarning("SteamLobby.Instance is null. Make sure SteamLobby is initialized correctly.");
        }
    }

    public void DisplayLobby(List<CSteamID> LobbyIDs, LobbyDataUpdate_t result)
    {
        if (LobbyDataItemPrefab == null || LobbyListContent == null)
        {
            Debug.LogWarning("LobbyDataItemPrefab or LobbyListContent is not assigned in the inspector.");
            return;
        }

        // Clear existing lobby items to avoid duplication
        DestroyLobby();

        for (int i = 0; i < LobbyIDs.Count; i++)
        {
            if (LobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                GameObject createdItem = Instantiate(LobbyDataItemPrefab, LobbyListContent.transform);
                createdItem.transform.localScale = Vector3.one;

                LobbyDataEntry lobbyDataEntry = createdItem.GetComponent<LobbyDataEntry>();
                if (lobbyDataEntry != null)
                {
                    lobbyDataEntry.LobbyID = (CSteamID)LobbyIDs[i].m_SteamID;
                    lobbyDataEntry.LobbyName = SteamMatchmaking.GetLobbyData((CSteamID)LobbyIDs[i].m_SteamID, "Name");
                    if (string.IsNullOrEmpty(lobbyDataEntry.LobbyName))
                    {
                        lobbyDataEntry.LobbyName = "Empty";
                    }
                    lobbyDataEntry.SetLobbyData();
                }
                else
                {
                    Debug.LogWarning("LobbyDataEntry component not found on LobbyDataItemPrefab.");
                }

                listOfLobby.Add(createdItem);
            }
        }
    }

    public void DestroyLobby()
    {
        foreach (GameObject lobbyItem in listOfLobby)
        {
            if (lobbyItem != null)
            {
                Destroy(lobbyItem);
            }
        }
        listOfLobby.Clear();
    }
}

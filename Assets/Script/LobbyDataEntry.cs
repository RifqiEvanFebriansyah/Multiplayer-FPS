using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using Edgegap;

public class LobbyDataEntry : MonoBehaviour
{
    public CSteamID LobbyID;
    public string LobbyName;
    public Text LobbyNameText;


    public void SetLobbyData()
    {
        if(LobbyNameText != null)
        {
            LobbyNameText.text = LobbyName;
        }
        else 
        {
            Debug.LogWarning("LobbyNameText is not assigned in the inspector.");
        }
    }

    public void JoinLobby()
    {
        SteamLobby.Instance.JoinLobby(LobbyID);
    }
}

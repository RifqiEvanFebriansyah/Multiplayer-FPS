using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using Edgegap;
using Unity.VisualScripting;

public class PlayerObjectController : NetworkBehaviour
{
        // Player Data
        [SyncVar] public int connectionID;
        [SyncVar] public string PlayerIdNumber;
        [SyncVar] public ulong PlayerSteamID;
        [SyncVar(hook = nameof(PlayerNameUpdate))] public string playerName;
        [SyncVar(hook = nameof(PlayerNameUpdate))]
        public string PlayerName;
        [SyncVar(hook = nameof(PlayerReadyUpdate))]
        public bool Ready;

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

      private void PlayerReadyUpdate(bool oldValue, bool newValue)
        {
            if (isServer)
            {
            this.Ready = newValue;
            }
            if (isClient)
            {
            LobbyController.Instance.UpdatePlayerList();
            }
        }

         private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
            var rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        [Command]
        private void CmdSetPlayerReady()
        {
            this.PlayerReadyUpdate(this.Ready, !this.Ready);
        }

        public void ChangeReady()
        {
            if (isOwned)
            {
            CmdSetPlayerReady();
            }
        
        }
    

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName());
        gameObject.name = "LocalGamePlayer";
        
        if (LobbyController.Instance != null)
        {
            LobbyController.Instance.FindLocalPlayer();  
            LobbyController.Instance.UpdateLobbyName();
        }
    }

    public override void OnStartClient()
    {
        Manager.GamePlayers.Add(this);

        if (LobbyController.Instance != null)
        {
        LobbyController.Instance.UpdateLobbyName();
        LobbyController.Instance.UpdatePlayerList();
        }
    }


    public override void OnStopClient()
    {
        Manager.GamePlayers.Remove(this);

        if (LobbyController.Instance != null)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    [Command]
    public void CmdSetPlayerName(string playerName)
    {
        // Directly set the player name; the hook will automatically be called
        this.playerName = playerName;
    } 

    private void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer)
        {
            playerName = newValue;
        }
        if (isClient && LobbyController.Instance != null)
        {
            LobbyController.Instance.UpdatePlayerList();
        }
    }

    public void CanStartGame(string SceneName)
    {
        if (isOwned)
        {
            CmdCanStartGame(SceneName);
        }
        
    }
    
    [Command]
    public void CmdCanStartGame(string SceneName)
    {
        manager.StartGame(SceneName);
    }


}


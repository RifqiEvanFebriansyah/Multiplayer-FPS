using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class PlayerListItem : MonoBehaviour
{
   public string PlayerName;
   public int ConnectionID;
   public ulong PlayerSteamID;
   private bool AvatarReceived;

   public Text PlayerNameText;
   public RawImage PlayerIcon;
   public Text PlayerReadyText;
   public bool Ready;

   protected Callback<AvatarImageLoaded_t> ImageLoaded;

      public void ChangePlayerReadyStatus()
    {
        if (Ready)
        {
            PlayerReadyText.text = "Ready";
            PlayerReadyText.color = Color.green;
        }
        else
        {
            PlayerReadyText.text = "Unready";
            PlayerReadyText.color = Color.red;
        }
    }

   private void Start()
   {
       // Initialize Steam callback for avatar loading
       if (SteamManager.Initialized)
       {
           ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
       }
       else
       {
           Debug.LogError("Steam is not initialized.");
       }
   }

   public void SetPlayerValues()
   {
        ChangePlayerReadyStatus();
       // Ensure PlayerNameText is updated with PlayerName
       if (PlayerNameText != null)
       {
           PlayerNameText.text = string.IsNullOrEmpty(PlayerName) ? "Unknown Player" : PlayerName;
           Debug.Log("Player name == " + PlayerName);
       }
       else
       {
           Debug.LogError("PlayerNameText is not assigned in the prefab!");
       }

       // Check if avatar has already been loaded
       if (!AvatarReceived)
       {
           GetPlayerIcon();
       }
   }

   void GetPlayerIcon()
   {
       // Attempt to get avatar image ID
       int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamID);
       if (ImageID == -1)
       {
           Debug.LogWarning("Avatar not found for Steam ID: " + PlayerSteamID);
           return; // Avatar not found, exit method
       }

       // Load the avatar texture
       Texture2D avatarTexture = GetSteamImageAsTexture(ImageID);
       if (avatarTexture != null)
       {
           PlayerIcon.texture = avatarTexture;
           AvatarReceived = true; // Mark avatar as received
       }
       else
       {
           Debug.LogWarning("Failed to load avatar texture for Steam ID: " + PlayerSteamID);
       }
   }

   private Texture2D GetSteamImageAsTexture(int iImage)
   {
       Texture2D texture = null;

       // Get image size from Steam API
       bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
       if (isValid)
       {
           byte[] image = new byte[width * height * 4];
            // byte[] image = new byte[4 * width * height];
            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

           if (isValid)
           {
               texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
               texture.LoadRawTextureData(image);
               texture.Apply();
           }
           else
           {
               Debug.LogError("Failed to retrieve image data for avatar.");
           }
       }
       else
       {
           Debug.LogError("Failed to get image size for avatar.");
       }

       return texture;
   }

   private void OnImageLoaded(AvatarImageLoaded_t callback)
   {
       // Ensure that the callback matches the player's SteamID
       if (callback.m_steamID.m_SteamID == PlayerSteamID && callback.m_iImage != 0)
       {
           // Ensure this is running on the main thread to update UI
           if (PlayerIcon != null)
           {
               Texture2D avatarTexture = GetSteamImageAsTexture(callback.m_iImage);
               if (avatarTexture != null)
               {
                   PlayerIcon.texture = avatarTexture;
                   AvatarReceived = true; // Mark avatar as received
               }
               else
               {
                   Debug.LogWarning("Failed to load avatar texture from callback.");
               }
           }
           else
           {
               Debug.LogError("PlayerIcon is not assigned in the prefab!");
           }
       }
   }
}

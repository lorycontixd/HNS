using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class UserMenu : LobbyMenu, IDbListener
{
    public User user;

    // In the future all variables will be read from the user class
    [Header("User Info")]
    [Tooltip("User's username")] public TextMeshProUGUI usernameText;
    [Tooltip("User's level value")] public TextMeshProUGUI userLevelValueText;
    [Tooltip("User profile picture")] public Image userProfileImage;

    [Header("User currencies")]
    [Tooltip("User gold")] public TextMeshProUGUI coinsValue;
    [Tooltip("User gems")] public TextMeshProUGUI gemsValue;

    [Header("UI")]
    public Button playButton;
    public Button addFriendButton;
    // Group buttons
    public Button shopButton;
    public Button itemsButton;
    public Button messagesButton;
    public Button missionsButton;
    public Button rankingButton;
    public Button settingsButton;

    [Header("Friends")]
    public AddFriendTab addFriendTab;

    public override void Start()
    {
        base.Start();
    }

    public override bool OnOpen()
    {
        bool parentSuccess = base.OnOpen();
        UpdateUI();
        LoadMenu();
        return true;
    }

    public override bool OnClose(bool leaveOpen = false)
    {
        bool parentSuccess = base.OnClose(leaveOpen);
        return parentSuccess;
    }

    public override void UpdateUI()
    {
    }

    private void SetupPlayer(User user, string adminCode = "")
    {
        PhotonNetwork.LocalPlayer.NickName = user.username;
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add("NickName", user.username);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    private void LoadMenu()
    {
        if (LobbyNetworkManager.Instance.GetSessionUser().IsNull())
        {
            throw new UnityException("Entered user profile, but session user is null");
        }
        User user = LobbyNetworkManager.Instance.GetSessionUser();
        usernameText.text = user.username;
        userLevelValueText.text = "0";
        coinsValue.text = "0";
        gemsValue.text = "0";
        playButton.interactable = true;

        addFriendTab.Close();

        SetupPlayer(user);
    }

    // Photon Callbacks
    #region Photon Callbacks
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.InRoom) // Must obviously be true
        {
            LobbyNetworkManager.Instance.LoadMainRoom();
        }
    }
    #endregion


    // Menu buttons
    #region Menu Buttons
    public void ButtonShop()
    {
        SpawnError("Not implemented", "This menu is not implemented yet");
    }
    public void ButtonFriends()
    {
        FriendsManager.Instance.friendlistPanel.Open();
    }
    public void ButtonItems()
    {
        SpawnError("Not implemented", "This menu is not implemented yet");
    }
    public void ButtonMessages()
    {
        SpawnError("Not implemented", "This menu is not implemented yet");
    }
    public void ButtonMissions()
    {
        SpawnError("Not implemented", "This menu is not implemented yet");
    }
    public void ButtonRanking()
    {
        SpawnError("Not implemented", "This menu is not implemented yet");
    }

    public void ButtonPlay()
    {
        playButton.interactable = false;
        MatchmakingManager.Instance.QuickMatch();
    }
    #endregion

}

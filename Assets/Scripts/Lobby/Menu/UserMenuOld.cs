using Michsky.MUIP;
using Michsky.UI.MTP;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserMenuOld : LobbyMenu
{
    public TextMeshProUGUI titleText;
    public ButtonManager enterWorldButton;
    public ButtonManager createPartyButton;
    public CustomDropdown userChangeAvatarDropdown;
    public TMP_InputField adminCodeInput;
    public CustomDropdown userChangeNameColourDropdown;
    public StyleManager enteringWorldStyle;

    private int selectedAvatar = -1;
    private string currentSelectedNameColor = "Red";

    [Header("Settings")]
    public bool debugMode;

    public override void Start()
    {
        base.Start();
        inputFields.Add(adminCodeInput);

        // Listen to name colour change event
        userChangeNameColourDropdown.onValueChanged.AddListener(OnNameColorChange);

        // Setup enter animation
        enteringWorldStyle.playOnEnable = false;
        enteringWorldStyle.showFor = 3f;
    }

    public override bool OnOpen()
    {
        bool parentSuccess =  base.OnOpen();
        UpdateUI();
        LoadMenu();
        return true;
    }
    public override bool OnClose(bool leaveOpen = false)
    {
        bool parentSuccess = base.OnClose(leaveOpen);
        return true;
    }

    public override void UpdateUI()
    {
        Debug.Log("UPDATING USERMENU UI");
        titleText.text = $"Welcome back, {LobbyNetworkManager.Instance.GetSessionUser().username}";
    }


    private void LoadMenu()
    {

        FriendsManager.Instance.FetchFriends(LobbyMenuController.In.GetSessionUser()); // (( move to friends script? ))

        // Hide new friends alert, will be activated on friends load.

        // Set buttons to non-interactable until everything else is loaded (rooms, friends, etc)
        enterWorldButton.Interactable(false);
        createPartyButton.Interactable(false);
        enterWorldButton.UpdateUI();
        StartCoroutine(LoadUserPanelButtons());

        // Set the selected avatar to the last avatar if it was previously selected, otherwise set the default one
        selectedAvatar = PlayerPrefs.GetInt("SelectedCharacter");
        if (selectedAvatar != -1)
        {
            userChangeAvatarDropdown.selectedItemIndex = selectedAvatar;
            userChangeAvatarDropdown.UpdateItemLayout();
        }
        else
        {
            userChangeAvatarDropdown.selectedItemIndex = 0;
            userChangeAvatarDropdown.UpdateItemLayout();
        }
    }

    
    /// <summary>
    ///  Hide the UI components below the dropdown to give it space.
    /// </summary>
    public void CharacterDropdownButton()
    {
        userChangeNameColourDropdown.gameObject.SetActive(!userChangeNameColourDropdown.gameObject.activeSelf);
    }

    /// <summary>
    /// Waits until all rooms are fetched before allowing to enter the game.
    /// </summary>
    /// <returns>Coroutine</returns>
    private IEnumerator LoadUserPanelButtons()
    {
        yield return new WaitUntil(() => MatchmakingManager.Instance.roomsInitialSet);
        enterWorldButton.Interactable(true);
        createPartyButton.Interactable(true);
        enterWorldButton.UpdateUI();
    }

    /// <summary>
    /// Callback for new avatar selection
    /// </summary>
    /// <param name="index">Index of the selected avatar.</param>
    public void OnCharacterSelect(int index)
    {
        selectedAvatar = index;
        PlayerPrefs.SetInt("SelectedCharacter", selectedAvatar);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Callback for name tag colour change
    /// </summary>
    /// <param name="index">Index of the selected colour</param>
    public void OnNameColorChange(int index)
    {
        currentSelectedNameColor = userChangeNameColourDropdown.items[index].itemName.Capitalize();
    }


    private void SetupPlayer(User user, string adminCode = "")
    {
        PhotonNetwork.LocalPlayer.NickName = user.username;
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add("NickName", user.username);
        hash.Add("AdminCode", adminCode);
        hash.Add("Character", selectedAvatar);
        hash.Add("NameColour", currentSelectedNameColor);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    // Photon Callbacks
    #region Photon Callbacks
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.InRoom) // Must obviously be true
        {
            if (PhotonNetwork.CurrentRoom.Name.Contains("Server"))
            {
                LobbyNetworkManager.Instance.LoadMainRoom();
            }
        }
    }
    #endregion

    // Button Callbacks
    #region Button Callbacks
    public void EnterWorldButton()
    {
        string adminCode = adminCodeInput.text;
        selectedAvatar = PlayerPrefs.GetInt("SelectedCharacter");

        // Deactivate all buttons during loading
        enterWorldButton.Interactable(false);
        enterWorldButton.UpdateUI();
        // Play entering world animation
        enteringWorldStyle.gameObject.SetActive(true);
        enteringWorldStyle.forceUpdate = false;
        enteringWorldStyle.Play();
        // Look for the first room available

        //MatchmakingManager.Instance.FirstRoomAvailable();
    }
    public void CreatePartyButton()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            string roomname = "Party_" + LobbyNetworkManager.Instance.GetSessionUser().username;
            RoomOptions opts = new RoomOptions() { MaxPlayers = (byte)32, IsOpen = true, IsVisible = true, PublishUserId = true };
            string password = string.Empty;
            StartCoroutine(DatabaseManager.In.AddParty(roomname, opts, password, LobbyNetworkManager.Instance.GetSessionUser()));
        }
    }
    #endregion



}

using Michsky.MUIP;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartyMenu : LobbyMenu
{
    public User lobbyOwner;

    [Header("UI")]
    public ButtonManager enterWorldButton;
    public GameObject friendlistButton;
    public Transform playerItemHolder;
    public Transform chatItemHolder;
    public TextMeshProUGUI lobbyTitle;

    [Header("Prefabs")]
    public GameObject lobbyPlayerItem;
    public GameObject lobbyChatItem;

    [Header("Settings")]
    public bool debugMode;

    private Dictionary<int, Player> players = new Dictionary<int, Player>();


    public override void Start()
    {
        base.Start();
    }

    public override bool OnOpen()
    {
        bool baseSuccess = base.OnOpen();
        UpdateUI();
        return true;
    }

    public override bool OnClose(bool leaveOpen = false)
    {
        bool baseSuccess = base.OnClose(leaveOpen);
        return true;
    }

    public override void UpdateUI()
    {
        lobbyTitle.text = $"{lobbyOwner.username}'s Party";
    }


    public void SetOwner(User user)
    {
        lobbyOwner = user;
    }


    // Photon Callbacks
    #region Photon Callbacks
    public override void OnJoinedRoom()
    {
        // This if statement prevents the callback to activate on room creation too -> only joining
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            // Seek the party leader user
            players = PhotonNetwork.CurrentRoom.Players;
            DisplayPlayers();
        }
    }
    public override void OnCreatedRoom()
    {
        if (PhotonNetwork.CurrentRoom.Name.Contains("Party"))
        {
            // This user has created a group/party
            ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable();
            roomProps["OwnerID"] = LobbyNetworkManager.Instance.GetSessionUserID();
            DisplayPlayers();
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        players = PhotonNetwork.CurrentRoom.Players;
        DisplayPlayers();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        players = PhotonNetwork.CurrentRoom.Players;
        DisplayPlayers();
    }
    #endregion

    // Button Callbacks
    #region Button Callbacks
    public void EnterWorldButton()
    {

    }
    public void CloseGroupButton()
    {

    }
    #endregion



    #region UI
    public void ClearPlayerList()
    {
        for (int i=0; i<playerItemHolder.childCount; i++)
        {
            Destroy(playerItemHolder.GetChild(i).gameObject);
        }
    }

    public void DisplayPlayers()
    {
        ClearPlayerList();
        foreach(KeyValuePair<int, Player> kvp in PhotonNetwork.CurrentRoom.Players)
        {
            GameObject clone = Instantiate(lobbyPlayerItem, parent: playerItemHolder);
            PartyPlayerItem item = clone.GetComponent<PartyPlayerItem>();
            item.SetPlayer(kvp.Value);
            item.UpdateUI();
        }
    }
    #endregion
}

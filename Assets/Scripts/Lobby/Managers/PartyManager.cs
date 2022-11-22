using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PartyManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    private static PartyManager _instance;
    public static PartyManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        // Makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    #endregion

    public Party party;

    private bool inparty = false; 
    public bool InParty { get { return inparty; } }

    public UnityEvent<Party> onPartyCreated;
    public UnityEvent<Party> onPartyJoined;

    [Header("Settings")]
    public float partyRefreshRate = 5;

    private float timestamp;

    private void Start()
    {
       DatabaseManager.In.onQuery.AddListener(OnQuery);
    }

    private void Update()
    {
        if (!party.IsNull())
        {
            if (timestamp <= Time.time)
            {
            }
        }
    }


    public void CreateParty()
    {
        string name = $"{LobbyNetworkManager.Instance.GetSessionUser().username}_party";
        RoomOptions opts = new RoomOptions();
        opts.MaxPlayers = (byte)MatchmakingManager.Instance.maxPlayers;
        opts.IsOpen = true;
        opts.IsVisible = true;
        opts.PublishUserId = true;
        opts.SuppressPlayerInfo = false;
        StartCoroutine(DatabaseManager.In.AddParty(name, opts, "", LobbyNetworkManager.Instance.GetSessionUser()));
    }

    public void OnQuery(ResultType type, QueryData data)
    {
        if (data.queryType == QueryType.ADDPARTY)
        {
            MyAddPartyData partydata = (MyAddPartyData)data;
            if (type == ResultType.SUCCESS)
            {
                party = partydata.party;
                bool success = PhotonNetwork.CreateRoom(party.partyname, party.roomopts);
            }
        }
    }

    // pun callbacks
    #region Pun Callbacks
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.CurrentRoom.Name.Contains("party"))
            {
                // Created a party
                if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
                {
                    Debug.Log("[PartyManager] Created party");
                    onPartyCreated?.Invoke(party);
                    onPartyJoined?.Invoke(party);
                }
                else
                {
                    Debug.Log("[PartyManager] Joined party");
                    onPartyJoined?.Invoke(party);
                }
            }
        }
    }
    #endregion
}
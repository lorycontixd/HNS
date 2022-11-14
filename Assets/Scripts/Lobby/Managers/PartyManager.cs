using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
       DatabaseManager.In.onQuery.AddListener(OnQuery);
    }

    private void Update()
    {
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
                StartCoroutine(DatabaseManager.In.SearchUserByIDCoroutine(party.creatorid, "create_party"));
            }
        }
        if (data.queryType == QueryType.SEARCHUSER)
        {
            MySearchUserData userdata = (MySearchUserData)data;
            if (type == ResultType.SUCCESS)
            {
                if (userdata.extraInfo == "create_party")
                {
                    if (party != null)
                    {
                        User creator = userdata.user;
                        LobbyMenuController.In.SwitchMenu(LobbyMenuType.PARTY);
                        PartyMenu partymenu = (PartyMenu)LobbyMenuController.In.GetMenuByType(LobbyMenuType.PARTY);
                        partymenu.SetOwner(creator);
                        partymenu.UpdateUI();
                    }
                }
            }
        }
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvitationManager : Singleton<InvitationManager>, IDbListener
{
    public InvitationNotification invitationNotification;

    private Party pendingParty = null;
    private User pendingPartyCreator = null;

    private void Start()
    {
        invitationNotification.gameObject.SetActive(false);
        DatabaseManager.In.onQuery.AddListener(OnQuery);
    }


    public void InvitePlayerToParty(User user)
    {
        if (!PartyManager.Instance.party.IsNull())
        {
            if (FriendsManager.Instance.IsFriendOnline(user.username))
            {
                ChatManager.Instance.InvitePlayerToParty(user, PartyManager.Instance.party);
            }
            else
            {
                LobbyMenuController.In.activeMenu.SpawnError("Friend not online", "Cannot invite this player to your party");
            }
        }
        else
        {
            LobbyMenuController.In.activeMenu.SpawnError("Error", "You must be in a party");
        }
    }

    public void OnInvitationReceived(string partytoken)
    {
        
        StartCoroutine(DatabaseManager.In.SearchParty(partytoken));
        //invitationNotification.UpdateUI(user.username);
    }

    public void OnInvitationAccepted()
    {
        // Log

        // Go to party
        PartyManager.Instance.party = pendingParty;
        StartCoroutine(DatabaseManager.In.EnterPartyCoroutine(LobbyNetworkManager.Instance.GetSessionUser(), pendingParty.token));
        
    }

    public void OnInvitationDeclined()
    {
        // Log

        // Close notification
        invitationNotification.Reset();
        invitationNotification.gameObject.SetActive(false);
        // Reset
        pendingParty = null; 
        pendingPartyCreator = null;
    }


    public void OnQuery(ResultType type, QueryData data)
    {
        if (data.queryType == QueryType.SEARCHPARTY)
        {
            MySearchPartyData partydata = (MySearchPartyData)data;
            if (type == ResultType.SUCCESS)
            {
                pendingParty = partydata.party;
                StartCoroutine(DatabaseManager.In.SearchUserByIDCoroutine(pendingParty.creatorid, "search_party"));
                return;
            }
        }
        if (data.queryType == QueryType.SEARCHUSER)
        {
            MySearchUserData userdata = (MySearchUserData)data;
            if (type == ResultType.SUCCESS)
            {
                if (userdata.extraInfo == "search_party")
                {
                    pendingPartyCreator = userdata.user;
                    // Turn notification on
                    invitationNotification.gameObject.SetActive(true);
                    invitationNotification.UpdateUI(pendingPartyCreator.username);
                    return;
                }
            }
        }
        if (data.queryType == QueryType.ENTERPARTY)
        {
            MyEnterPartyData partydata = (MyEnterPartyData)data;
            if (type == ResultType.SUCCESS)
            {
                PhotonNetwork.JoinRoom(PartyManager.Instance.party.partyname);
                LobbyMenuController.In.SwitchMenu(LobbyMenuType.PARTY);
                PartyMenu pmenu = (PartyMenu)LobbyMenuController.In.GetMenuByType(LobbyMenuType.PARTY);
                pmenu.SetOwner(pendingPartyCreator);
                pmenu.UpdateUI();
                invitationNotification.gameObject.SetActive(false);
                pendingParty = null;
                pendingPartyCreator = null;
            }
        }
    }
}

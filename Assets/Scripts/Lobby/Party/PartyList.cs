using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyList : MonoBehaviourPunCallbacks, IDbListener
{
    private Party party;
    private User leader;
    public GameObject mainPanel;
    public Transform partyPlayersHolder;

    [Header("UI")]
    public TextMeshProUGUI partyNameText;
    public Button createPartyButton;
    public Button partyListButton;
    public Button leavePartyButton;

    [Header("Prefabs")]
    public GameObject partyPlayerItem;

    private void Start()
    {
        DatabaseManager.In.onQuery.AddListener(OnQuery);
        PartyManager.Instance.onPartyJoined.AddListener(OnPartyJoined);

        leavePartyButton.gameObject.SetActive(false);
        partyListButton.gameObject.SetActive(false);
    }

    public void UpdateUI()
    {
        partyNameText.text = $"{leader.username}'s party";
    }

    public void Open()
    {
        Debug.Log("Opening");
        mainPanel.SetActive(true);
    }
    public void Close()
    {
        mainPanel.SetActive(false);
    }


    public void SetParty(Party party)
    {
        this.party = party;
    }
    public Party GetParty()
    {
        return this.party;
    }
    public void SetLeader(User user)
    {
        this.leader = user;
    }

    public void OnPartyJoined(Party party)
    {
        Debug.Log("[PartyList] Joined party!");
        StartCoroutine(DatabaseManager.In.SearchUserByIDCoroutine(party.creatorid, "join_party"));
        SetParty(party);
        createPartyButton.gameObject.SetActive(false);
        partyListButton.gameObject.SetActive(true);
        leavePartyButton.gameObject.SetActive(true);
    }
    public void OnPartyLeft()
    {
        createPartyButton.gameObject.SetActive(true);
        leavePartyButton.gameObject.SetActive(false);
        partyListButton.gameObject.SetActive(false);
    }



    #region UI
    public void ButtonClose()
    {
        Close();
    }
    public void ButtonOpen()
    {
        Open();
    }

    public void ClearPartyPlayers()
    {
        for (int i=0; i<partyPlayersHolder.childCount; i++)
        {
            Destroy(partyPlayersHolder.GetChild(i).gameObject);
        }
    }
    public void DisplayPartyPlayers()
    {
        ClearPartyPlayers();
        foreach( KeyValuePair<int, Player> kvp in PhotonNetwork.CurrentRoom.Players)
        {
            GameObject clone = Instantiate(partyPlayerItem, partyPlayersHolder.transform);
            clone.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = kvp.Value.UserId;
        }
    }
    #endregion

    #region Pun Callbacks
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        DisplayPartyPlayers();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        DisplayPartyPlayers();
    }
    #endregion

    public void OnQuery(ResultType type, QueryData data)
    {
        if (data.queryType == QueryType.SEARCHUSER)
        {
            MySearchUserData userdata = (MySearchUserData)data;
            if (type == ResultType.SUCCESS)
            {
                User partycreator = userdata.user;
                if (data.extraInfo == "join_party")
                {
                    SetLeader(partycreator);
                    UpdateUI();
                    Open();
                    DisplayPartyPlayers();
                }
            }
        }
    }
}

using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchmakingManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    private static MatchmakingManager _instance;
    public static MatchmakingManager Instance { get { return _instance; } }

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
    }
    #endregion

    [Header("Settings")]
    public int maxPlayers = 8;

    public bool roomsInitialSet;
    public List<RoomInfo> rooms = new List<RoomInfo>();

    private void Start()
    {
    }


    public void QuickMatch()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (!PhotonNetwork.InRoom)
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }
    }

    public void JoinMatch(string name)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.JoinRoom(name);
            }
        }
    }

    public void CreateMatch(User user)
    {

    }

    public void CreateGroupMatch(Party party)
    {

    }


    // Pun callbacks
    #region Pun Callbacks
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        rooms.Clear();
        rooms = roomList;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom("Room" + UnityEngine.Random.Range(0, 9999));
    }
    #endregion
}

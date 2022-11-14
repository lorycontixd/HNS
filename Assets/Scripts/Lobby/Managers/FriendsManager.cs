using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

/// <summary>
/// Friends manager is a mixture of connections using Azure SQL database and Photon friend list.
/// The SQL database keeps persistent data of friendships, while photon keeps track of online/offline friends.
///
/// </summary>
public class FriendsManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    private static FriendsManager _instance;
    public static FriendsManager Instance { get { return _instance; } }

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

    public string appAddress;
    //public FriendListTab friendListTab;
    public FriendlistPanel friendlistPanel;


    [Header("Settings")]
    public float refreshRate = 3f;
    public bool canSearch = false;

    private float refreshRateTimestamp = 0f;
    private int searches = 0;

    public UnityEvent onRefresh;

    private List<User> friends = new List<User>();
    private List<User> unacceptedFriends = new List<User>();
    private List<User> pendingFriends = new List<User>();

    private List<User> lastFriends = new List<User>();
    private List<User> lastUnacceptedFriends = new List<User>();
    private List<User> lastPendingFriends = new List<User>();

    private List<FriendInfo> lastFriendlistUpdate = new List<FriendInfo>();
    public bool friendshipsUpdatedThisFrame = false;

    public UnityEvent<List<FriendInfo>> onFriendStatusUpdate;

    private void Start()
    {
        friendlistPanel.Close();

        DatabaseManager.In.onQuery.AddListener(OnQuery);
    }

    private void Update()
    {
        canSearch = LobbyNetworkManager.Instance.GetSessionUser() != null;
        if (canSearch && LobbyMenuController.In.activeMenu.hasFriendlist && PhotonNetwork.IsConnectedAndReady)
        {
            if (refreshRateTimestamp <= Time.time)
            {
                refreshRateTimestamp = Time.time + refreshRate;
                GetFriends();
                bool success = GetFriendsStatus(BuildFriends());
                searches += 1;
                onRefresh?.Invoke();
            }
        }
    }


    public void GetFriends()
    {
        int id = LobbyNetworkManager.Instance.GetSessionUserID();
        StartCoroutine(DatabaseManager.In.GetFriendships2(id));
    }

    /// <summary>
    /// Convert friendlist into array of string of usernames
    /// </summary>
    /// <returns></returns>
    public string[] BuildFriends()
    {
        string[] myfriends = new string[friends.Count];
        int i = 0;
        foreach (User friend in friends)
        {
            myfriends[i] = friend.username;
        }
        return myfriends;
    }

    public bool GetFriendsStatus(string[] friendsUserIds)
    {
        return PhotonNetwork.FindFriends(friendsUserIds);
    }

    public bool IsFriendOnline(string username)
    {
        if (lastFriendlistUpdate != null)
        {
            if (friends.Count > 0)
            {
                FriendInfo friendinfo = lastFriendlistUpdate.FirstOrDefault(f => f.UserId == username);
                return friendinfo.IsOnline;
            }
            else
            {
                return false;
            }
        }
        else
        {
            throw new UnityException("Searching online frined but friendlist has not been created");
        }
    }


    public void FetchFriends(User user)
    {
        if (user != null)
        {
            Debug.Log("Fetching friedns");
            StartCoroutine(DatabaseManager.In.GetFriendships2(user.id)); // goes to onquery->getfriendships
        }
    }

    private bool IsUserListUpdated(List<User> oldusers, List<User> newusers)
    {
        if (oldusers.Count != newusers.Count)
        {
            return true;
        }
        return !newusers.SequenceEqual(oldusers);
    }


    public override void OnFriendListUpdate(List<FriendInfo> friendsInfo)
    {
        foreach(FriendInfo info in friendsInfo)
        {
            Debug.Log($"[ONFRIENDLISTUPDATE] Friend {info.UserId} -> {info.IsOnline} -> {info.IsInRoom}");
        }
        lastFriendlistUpdate = friendsInfo;
        onFriendStatusUpdate?.Invoke(friendsInfo);
    }

    public void OnQuery(ResultType type, QueryData data)
    {
        if (data.queryType == QueryType.GETFRIENDSHIPS)
        {
            MyGetFriendshipsData fdata = (MyGetFriendshipsData)data;
            if (type == ResultType.SUCCESS)
            {
                ParseFriendshipsResult result = fdata.friends;
                friends = result.friends;
                unacceptedFriends = result.unacceptedFriends;
                pendingFriends = result.pendingFriends;

                //int unacceptedRequests = fdata.userfriendships.FindAll(f => f.isaccepted == false && f.receiverid == LobbyNetworkManager.Instance.GetSessionUserID()).Count;
                //friendListTab.SetNotification(unacceptedRequests);

                //StartCoroutine(friendListTab.DisplayFriends(currentFriendships));
                friendshipsUpdatedThisFrame = IsUserListUpdated(lastFriends, friends) && IsUserListUpdated(lastPendingFriends, pendingFriends) && IsUserListUpdated(lastUnacceptedFriends, unacceptedFriends);

                if (friendshipsUpdatedThisFrame || lastFriends.Count == 0)
                {
                    // Re-display
                    friendlistPanel.UpdateUI(friends, pendingFriends, unacceptedFriends);
                }

                // Update last
                lastFriends = friends;
                lastUnacceptedFriends = unacceptedFriends;
                lastPendingFriends = pendingFriends;
            }
        }
    }
}
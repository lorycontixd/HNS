using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FriendlistPanel : MonoBehaviour, IDbListener
{
    public GameObject mainPanel;
    public FriendInfoPanelNew friendInfoPanel;
    private List<FriendItem> friendItems = new List<FriendItem>();

    [Tooltip("Parent that only holds prefabs of accepted friendships")] public Transform friendsHolder;
    [Tooltip("Parent that holds unaccepted requests")] public Transform unacceptedFriendsHolder; // Pending requests go to friends or unaccepted??

    [Header("Prefabs")]
    public GameObject friendItem;
    public GameObject pendingFriendItem;
    public GameObject unacceptedFriendItem;

    private void Start()
    {
        friendInfoPanel.gameObject.SetActive(false);

        FriendsManager.Instance.onFriendStatusUpdate.AddListener(OnFriendStatusUpdate);
    }


    public void Open()
    {
        mainPanel.SetActive(true);
    }
    public void Close()
    {
        mainPanel.SetActive(false);
    }
    

    public void ClearFriendList()
    {
        for (int i=0; i<friendsHolder.childCount; i++)
        {
            Destroy(friendsHolder.GetChild(i).gameObject);
        }
    }

    public void ClearUnacceptedList()
    {
        for (int i = 0; i < unacceptedFriendsHolder.childCount; i++)
        {
            Destroy(unacceptedFriendsHolder.GetChild(i).gameObject);
        }
    }

    public void UpdateUI(List<User> friends, List<User> pendingFriends, List<User> unacceptedFriends)
    {
        Debug.Log("Updating ui");
        ClearFriendList();
        ClearUnacceptedList();
        foreach(User user in friends)
        {
            GameObject clone = Instantiate(friendItem, friendsHolder);
            FriendItem item = clone.GetComponent<FriendItem>();
            item.SetFriend(user);
            item.UpdateUI();
            friendItems.Add(item);
            item.onMoreInfo.AddListener(OnItemMoreInfo);
        }
        foreach(User user in pendingFriends)
        {
            GameObject clone = Instantiate(pendingFriendItem, unacceptedFriendsHolder);

        }
        foreach(User user in unacceptedFriends)
        {
            GameObject clone = Instantiate(unacceptedFriendItem, unacceptedFriendsHolder);
            FriendRequestItem item = clone.GetComponent<FriendRequestItem>();
            item.SetSender(user);
            item.UpdateUI();
            item.onAccept.AddListener(OnRequestAccepted);
            item.onDeny.AddListener(OnRequestDenied);
        }
    }

    public void OnRequestAccepted(Friendship friendship, User sender)
    {
        Debug.Log($"Friend request from {sender.username} accepted!!");
        StartCoroutine(DatabaseManager.In.AcceptFriendship(sender, LobbyNetworkManager.Instance.GetSessionUser()));
    }
    public void OnRequestDenied(Friendship friendship, User sender)
    {
        Debug.Log($"Friend request from {sender.username} denied :(");
    }


    public FriendItem FindFriend(string username)
    {
        FriendItem frienditem = friendItems.FirstOrDefault(i => i.friend.username == username);
        return frienditem; // handle exception
    }

    public void OnFriendStatusUpdate(List<FriendInfo> friendsInfo)
    {
        for (int i = 0; i < friendsInfo.Count; i++)
        {
            FriendInfo friend = friendsInfo[i];
            Debug.Log("Friend name: " + friend.UserId);

            FriendItem item = FindFriend(friend.UserId);
            item.SetStatus(friend.IsOnline);
            if (friendInfoPanel.gameObject.activeSelf && friendInfoPanel.friend.username == item.friend.username)
            {
                friendInfoPanel.SetOnline(friend.IsOnline);
            }
        }
    }

    public void OnItemMoreInfo(FriendItem item)
    {
        User friendClicked = item.friend;
        friendInfoPanel.SetFriend(friendClicked);
        friendInfoPanel.UpdateUI();
        friendInfoPanel.gameObject.SetActive(true);

    }

    public void OnQuery(ResultType type, QueryData data)
    {
    }
}

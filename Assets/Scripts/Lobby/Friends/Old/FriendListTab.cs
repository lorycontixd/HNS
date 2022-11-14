using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Michsky.MUIP;
using TMPro;
using System.Linq;

public struct FirstLastName
{
    public string firstname;
    public string lastname;
}

public class FriendListTab : MonoBehaviour
{
    public FriendInfoPanel friendInfoPanel;

    [Header("UI - Friendlist Button")]
    public GameObject friendlistPanelButton;
    public GameObject friendNotificationNumberParent;
    public TextMeshProUGUI friendNotificationNumberText;

    [Header("UI - Friendlist")]
    public GameObject friendlistMainPanel;
    // Usersearch
    public TMP_InputField userSearchInput;
    public ButtonManager userSearchButton;
    public GameObject userSearchResultPanel;
    public GameObject userSearchResultItemHolder;
    public GameObject userFriendRequestItemHolder;
    public GameObject userFriendItemHolder;

    [Header("Prefabs")]
    public GameObject userSearchResultItemPrefab;
    public GameObject userFriendRequestItemPrefab;
    public GameObject userFriendItemPrefab;
    public GameObject userFriendPendingItemPrefab;

    [Header("Settings")]
    public bool debugMode;

    private List<FriendRequestItem> friendRequests = new List<FriendRequestItem>();
    [HideInInspector] public List<FriendItem> friends = new List<FriendItem>();
    private List<FriendPendingItem> friendPendingRequests = new List<FriendPendingItem>();

    private User receivedUserForFriendship = null;
    public bool initialFriendListUpdated;
    private bool usersSeachPanelActive = false;

    private void Start()
    {
        userSearchResultPanel.SetActive(false);
        DatabaseManager.In.onQuery.AddListener(OnQuery);
        FriendsManager.Instance.onRefresh.AddListener(OnRefresh);

        if (friendInfoPanel != null)
        {
            friendInfoPanel.onUnfriend.AddListener(OnUnfriend);
            friendInfoPanel.gameObject.SetActive(false);
        }
        CloseFriendMainPanel();
        CloseFriendListPanelButton();
        CloseNewFriendsNotificationButton();
    }

    private void Update()
    {
        if (LobbyMenuController.In.activeMenu.hasFriendlist)
        {
            OpenFriendListPanelButton();
        }
    }


    // FriendsManager friends refresh listener
    public void OnRefresh()
    {

    }


    public void OpenFriendMainPanel()
    {
        friendlistMainPanel.SetActive(true);
    }
    public void CloseFriendMainPanel()
    {
        friendlistMainPanel.SetActive(false);
    }


    public void CloseFriendListPanelButton()
    {
        friendlistPanelButton.SetActive(false);
    }
    public void OpenFriendListPanelButton()
    {

        friendlistPanelButton.SetActive(true);
    }

    /// <summary>
    /// Closes the new friends notification number
    /// </summary>
    public void CloseNewFriendsNotificationButton()
    {
        friendNotificationNumberParent.SetActive(false);
    }

    /// <summary>
    /// Opens the new friends notification number
    /// </summary>
    public void OpenNewFriendsNotificationButton()
    {
        friendNotificationNumberParent.SetActive(true);
    }



    /// <summary>
    /// Sets the notification of unaccepted friend requests.
    /// If the number of unaccepted requests is 0, turns off the notification
    /// </summary>
    /// <param name="unaccepted">Number of unaccepted friend requests</param>
    public void SetNotification(int unaccepted)
    {
        OpenNewFriendsNotificationButton();
        if (unaccepted > 0)
        {
            friendNotificationNumberParent.SetActive(true);
            friendNotificationNumberText.text = unaccepted.ToString();
        }
        else
        {
            friendNotificationNumberParent.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ButtonFriendList()
    {
        friendlistMainPanel.SetActive(!friendlistMainPanel.activeSelf);
        if (friendlistMainPanel.activeSelf && debugMode)
        {
            userSearchInput.text = "Lorenzo Conti";
        }
    }
    


    /// <summary>
    ///  Parses 'Name Surname' with a space
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public FirstLastName ParseFirstLastName(string text)
    {
        string[] values = text.Split(' ');
        FirstLastName result = new FirstLastName();
        result.firstname = values[0];
        result.lastname = values[1];
        return result;
    }

    // User search
    #region User search

    /// <summary>
    /// Button callback for friends search
    /// </summary>
    public void ButtonSearch()
    {
        if (!usersSeachPanelActive)
        {
            if (userSearchInput.text == string.Empty)
            {
                return;
            }
            FirstLastName result = ParseFirstLastName(userSearchInput.text);
            StartCoroutine(DatabaseManager.In.SearchUsersCoroutine(result.firstname, result.lastname));
        }
        
    }

    /// <summary>
    /// Button callback to close the search panel
    /// </summary>
    public void ButtonCloseSearchPanel()
    {
        usersSeachPanelActive = false;
        userSearchResultPanel.SetActive(false);
    }

    /// <summary>
    /// Loads search results.
    /// <br>- Activates search results panel</br>
    /// <br>- Loads results</br>
    /// </summary>
    /// <param name="users"></param>
    public void LoadSearchResults(List<User> users)
    {
        usersSeachPanelActive = true;
        userSearchResultPanel.SetActive(true);
        ClearSearchResults();
        foreach (User user in users)
        {
            if (user.id != LobbyNetworkManager.Instance.GetSessionUserID())
            {
                GameObject clone = Instantiate(userSearchResultItemPrefab, userSearchResultItemHolder.transform);
                FriendlistUserSearchItem item = clone.GetComponent<FriendlistUserSearchItem>();
                item.SetUser(user);
                item.UpdateUI();
                item.onItemClick.AddListener(OnItemClick);
            }
        }
    }

    /// <summary>
    /// Clears all search results
    /// </summary>
    public void ClearSearchResults()
    {
        for (int i=0; i<userSearchResultItemHolder.transform.childCount; i++)
        {
            Destroy(userSearchResultItemHolder.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Callback for user search item click
    /// </summary>
    /// <param name="user">The user of the clicked item</param>
    public void OnItemClick(User user)
    {
        StartCoroutine(DatabaseManager.In.SendFriendship(LobbyNetworkManager.Instance.GetSessionUser(), user));
        userSearchResultPanel.gameObject.SetActive(false);

        AddPendingFriendUser(user);
    }
    #endregion


    /// <summary>
    /// Add a pending request when the user is already known
    /// </summary>
    /// <param name="user"></param>
    private void AddPendingFriendUser(User user)
    {
        GameObject clone = Instantiate(userFriendPendingItemPrefab, userFriendRequestItemHolder.transform);
        FriendPendingItem item = clone.GetComponent<FriendPendingItem>();
        item.SetFriend(user);
        item.UpdateUI();
        friendPendingRequests.Add(item);
    }

    /// <summary>
    ///  Add a pending request, but also request for the user by ID.
    /// </summary>
    /// <param name="userid"></param>
    /// <returns></returns>
    private IEnumerator AddPendingFriendID(int userid)
    {
        StartCoroutine(DatabaseManager.In.SearchUserByIDCoroutine(userid, "friendsdisplay"));
        yield return new WaitUntil(() => receivedUserForFriendship != null);
        GameObject clone = Instantiate(userFriendPendingItemPrefab, userFriendRequestItemHolder.transform);
        FriendPendingItem item = clone.GetComponent<FriendPendingItem>();
        item.SetFriend(receivedUserForFriendship);
        item.UpdateUI();
        friendPendingRequests.Add(item);
    }

    /// <summary>
    /// Clear friend request list - UI.
    /// </summary>
    public void ClearFriendRequests()
    {
        for (int i=0; i< userFriendRequestItemHolder.transform.childCount; i++)
        {
            Destroy(userFriendRequestItemHolder.transform.GetChild(i).gameObject);
        }
    }

    public void ClearFriendList()
    {
        for (int i = 0; i < userFriendItemHolder.transform.childCount; i++)
        {
            Destroy(userFriendItemHolder.transform.GetChild(i).gameObject);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="friendships"></param>
    /// <returns></returns>
    public IEnumerator DisplayFriends(List<Friendship> friendships)
    {
        ClearFriendRequests();
        foreach (Friendship friendship in friendships)
        {
            receivedUserForFriendship = null;
            if (!friendship.isaccepted)
            {
                if (friendship.receiverid == LobbyNetworkManager.Instance.GetSessionUserID())
                {
                    StartCoroutine(DatabaseManager.In.SearchUserByIDCoroutine(friendship.senderid, "friendsdisplay"));
                    yield return new WaitUntil(() => receivedUserForFriendship != null);
                    GameObject clone = Instantiate(userFriendRequestItemPrefab, userFriendRequestItemHolder.transform);
                    FriendRequestItem item = clone.GetComponent<FriendRequestItem>();
                    item.onAccept.AddListener(OnRequestAccepted);
                    item.onDeny.AddListener(OnRequestDenied);
                    item.SetFriendship(friendship);
                    item.SetSender(receivedUserForFriendship);
                    item.UpdateUI();
                    friendRequests.Add(item);
                }
                else
                {
                    StartCoroutine(AddPendingFriendID(friendship.receiverid));
                }
            }
            else
            {
                int senderid = friendship.senderid;
                int receiverid = friendship.receiverid;
                int friendid;
                if (receiverid == LobbyNetworkManager.Instance.GetSessionUserID())
                {
                    friendid = senderid;
                }
                else
                {
                    friendid = receiverid;
                }

                StartCoroutine(DatabaseManager.In.SearchUserByIDCoroutine(friendid, "friendsdisplay"));
                yield return new WaitUntil(() => receivedUserForFriendship != null);
                GameObject clone = Instantiate(userFriendItemPrefab, userFriendItemHolder.transform);
                FriendItem item = clone.GetComponent<FriendItem>();
                item.SetFriend(receivedUserForFriendship);
                item.UpdateUI();
                item.onMoreInfo.AddListener(OpenFriendInfo);
                friends.Add(item);
            }
        }
        receivedUserForFriendship = null;
    }

    public void OpenFriendInfo(FriendItem item)
    {
        User friend = item.friend;
        friendInfoPanel.gameObject.SetActive(true);
        friendInfoPanel.SetFriend(friend);
        friendInfoPanel.UpdateUI();
    }

    public void OnUnfriend(User friend)
    {
        StartCoroutine(DatabaseManager.In.RemoveFriendship(LobbyNetworkManager.Instance.GetSessionUser(), friend));
    }

    public FriendItem FindFriend(string username)
    {
        FriendItem frienditem = friends.FirstOrDefault(i => i.friend.username == username);
        return frienditem; // handle exception
    }

    // -------- Events --------------
    // On request accept
    public void OnRequestAccepted(Friendship friendship, User sender)
    {
        StartCoroutine(DatabaseManager.In.AcceptFriendship(sender, LobbyNetworkManager.Instance.GetSessionUser()));
    }
    public void OnRequestDenied(Friendship friendship, User sender)
    {

    }
    // Query callback
    public void OnQuery(ResultType type, QueryData data)
    {
        if (data.queryType == QueryType.SEARCHUSERS)
        {
            if (type == ResultType.SUCCESS)
            {
                MySearchUsersData usersdata = (MySearchUsersData)data;
                Debug.Log("Search users success, users: " +usersdata.users.Count);
                LoadSearchResults(usersdata.users);
            }
        }
        else if (data.queryType == QueryType.SEARCHUSER)
        {
            MySearchUserData userdata = (MySearchUserData)data;
            if (type == ResultType.SUCCESS)
            {
                if (data.extraInfo == "friendsdisplay")
                {
                    receivedUserForFriendship = userdata.user;
                }
            }
            else
            {
                Debug.Log("NONO:  " + userdata.errorMessage);   
            }
        }
        else if (data.queryType == QueryType.SENDFRIENDSHIP)
        {
            MyAddFriendshipData friendshipdata = (MyAddFriendshipData)data;
            if (type == ResultType.SUCCESS)
            {
                Debug.Log("Friendship added correctly: " + friendshipdata.receiver.username);
            }
            else
            {
                Debug.Log("Friendship add failed: " + friendshipdata.errorMessage);
            }
        }else if (data.queryType == QueryType.ACCEPTFRIENDSHIP)
        {
            MyAcceptFriendship accdata = (MyAcceptFriendship)data;
            if (type == ResultType.SUCCESS)
            {
                foreach(FriendRequestItem reqItem in friendRequests)
                {
                    if (friendRequests.Contains(reqItem))
                    {
                        friendRequests.Remove(reqItem);
                    }
                    Destroy(reqItem.gameObject);
                    GameObject clone = Instantiate(userFriendItemPrefab, userFriendItemHolder.transform);
                    FriendItem item = clone.GetComponent<FriendItem>();
                    item.SetFriend(reqItem.sender);
                    item.UpdateUI();
                    friends.Add(item);
                    break;
                }            
            }
        }
        else if (data.queryType == QueryType.REMOVEFRIENDSHIP)
        {
            MyRemoveFriendship remdata = (MyRemoveFriendship)data;
            foreach (FriendItem item in friends)
            {
                if (item.friend == remdata.receiver)
                {
                    Destroy(item);
                    break;
                }
            }
        }
    }
}

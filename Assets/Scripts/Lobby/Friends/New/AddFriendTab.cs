using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AddFriendTab : MonoBehaviour, IDbListener
{
    private User foundUser;
    public GameObject mainPanel;

    [Header("UI")]
    // Search
    public TMP_InputField usernameInput;
    public GameObject foundUserPanel;
    // Result
    public TextMeshProUGUI foundUserUsernameText;
    public TextMeshProUGUI foundUserLevelText;
    public TextMeshProUGUI foundUserScoreText;
    public Image foundUserImage;

    void Start()
    {
        DatabaseManager.In.onQuery.AddListener(OnQuery);   
    }

    public void Open()
    {
        mainPanel.SetActive(true);
        foundUserPanel.SetActive(false);
    }
    public void Close()
    {
        usernameInput.text = string.Empty;
        foundUser = null;
        foundUserUsernameText.text = string.Empty;
        foundUserLevelText.text = string.Empty;
        mainPanel.SetActive(false);
    }

    public void OnUserFound(User user)
    {
        foundUser = user;
        foundUserPanel.SetActive(true);
        foundUserUsernameText.text = foundUser.username;
        foundUserLevelText.text = "LV 1";
    }
    public void OnUserNotFound()
    {
        Debug.Log("User not found!");
        foundUserPanel.SetActive(false);
        // Spawn not found message
    }

    public void SearchButton()
    {
        if (usernameInput.text == string.Empty)
        {
            LobbyMenuController.In.activeMenu.SpawnError("Error searching friend", "You must enter a username");
            return;
        }
        if (usernameInput.text == LobbyNetworkManager.Instance.GetSessionUser().username)
        {
            LobbyMenuController.In.activeMenu.SpawnError("Error searching friend", "Cannot add yourself as a friend");
            return;
        }
        Search(usernameInput.text);
    }

    public void Search(string username)
    {
        StartCoroutine(DatabaseManager.In.SearchUserByUsernameCoroutine(username, "friend_add"));
    }

    public void FoundUserButton()
    {
        StartCoroutine(DatabaseManager.In.SendFriendship(LobbyNetworkManager.Instance.GetSessionUser(), foundUser));
    }

    public void OnQuery(ResultType type, QueryData data)
    {
        if (data.queryType == QueryType.SEARCHUSER)
        {
            MySearchUserData userdata = (MySearchUserData)data;
            Debug.Log("Received search user event");
            if (userdata.extraInfo == "friend_add")
            {
                Debug.Log("Received search user event with info friend_add");
                if (type == ResultType.SUCCESS)
                {
                    // Found user
                    Debug.Log("Found user!! : " + userdata.user.username);
                    OnUserFound(userdata.user);
                }
                else
                {
                    OnUserNotFound();
                }
            }
        }
    }
}

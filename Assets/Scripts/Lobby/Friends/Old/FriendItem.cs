using System.Collections;
using System.Collections.Generic;
using TMPro;
using Michsky.MUIP;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FriendItem : MonoBehaviour
{
    public User friend;

    [Header("UI")]
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI lastOnlineText;
    public Button moreButton;
    public GameObject onlineStateImage;
    public GameObject offlineStateImage;

    public UnityEvent<FriendItem> onMoreInfo;


    private void Start()
    {
        moreButton.onClick.AddListener(OnMoreInfo);
    }

    public void SetFriend(User friend)
    {
        this.friend = friend;
    }

    public void UpdateUI()
    {
        usernameText.text = friend.username;
        //lastOnlineText.text = "Offline";
    }

    public void SetStatus(bool isOnline)
    {
        if (isOnline)
        {
            onlineStateImage.SetActive(true);
            offlineStateImage.SetActive(false);
        }
        else
        {
            onlineStateImage.SetActive(false);
            offlineStateImage.SetActive(true);
        }
    }

    public void OnMoreInfo()
    {
        if (friend != null)
        {
            onMoreInfo?.Invoke(this);
        }
    }
}

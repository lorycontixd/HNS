using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendInfoPanelNew : MonoBehaviour
{
    public User friend;

    [Header("UI")]
    public TextMeshProUGUI usernameText;
    public Image iconImage;
    public TextMeshProUGUI statusText;
    public Button messageButton;
    public Button inviteButton;
    public Button unfriendButton;

    private void Start()
    {
    }

    public void SetFriend(User friend)
    {
        this.friend = friend;
    }

    public void UpdateUI()
    {
        usernameText.text = friend.username;
        SetOnline(FriendsManager.Instance.IsFriendOnline(friend.username));
    }

    public void SetOnline(bool isOnline)
    {
        statusText.text = isOnline ? "Online" : "Offline";
    }

    public void ButtonMessage()
    {
        if (friend != null)
        {
        }
    }
    public void ButtonInvite()
    {
        if (friend != null)
        {
            InvitationManager.In.InvitePlayerToParty(friend);
        }
    }
    public void ButtonUnfriend()
    {
        if (friend != null)
        {

        }
    }
}

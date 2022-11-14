using System.Collections;
using System.Collections.Generic;
using TMPro;
using Michsky.MUIP;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FriendInfoPanel : MonoBehaviour
{
    public User friend;

    [Header("UI")]
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI lastonlineText;
    public TextMeshProUGUI friendsSinceText;
    public ButtonManager messageButton;
    public ButtonManager unfriendButton;
    public ButtonManager closeButton;

    public UnityEvent<User> onUnfriend;


    public void SetFriend(User friend)
    {
        this.friend = friend;
    }

    public void UpdateUI()
    {
        usernameText.text = friend.username;
        lastonlineText.text = "Last online: TBD";
        friendsSinceText.text = "Friends since: TBD";
        unfriendButton.onClick.AddListener(OnUnfriend);
        closeButton.onClick.AddListener(OnClose);
    }

    public void OnInviteButton()
    {
        if (friend == null)
        {
            return;
        }
        InvitationManager.In.InvitePlayerToParty(friend);
        OnClose();
    }

    public void OnUnfriend()
    {
        if (friend != null)
        {
            onUnfriend?.Invoke(friend);
        }
    }

    public void OnClose()
    {
        friend = null;
        gameObject.SetActive(false);
    }
}
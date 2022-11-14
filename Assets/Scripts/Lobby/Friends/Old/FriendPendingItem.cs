using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FriendPendingItem : MonoBehaviour
{
    public User friend;

    [Header("UI")]
    public TextMeshProUGUI usernameText;

    public void SetFriend(User friend)
    {
        this.friend = friend;
    }

    public void UpdateUI()
    {
        usernameText.text = friend.username;
    }
}

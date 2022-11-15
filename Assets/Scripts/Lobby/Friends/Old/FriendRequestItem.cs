using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.MUIP;
using UnityEngine.Events;

public class FriendRequestItem : MonoBehaviour
{
    public Friendship friendship;
    public User sender = null;

    public TextMeshProUGUI usernameText;
    public Button acceptButton;
    public Button denyButton;

    public UnityEvent<Friendship, User> onAccept;
    public UnityEvent<Friendship, User> onDeny;

    public void SetFriendship(Friendship friendship, bool update = false)
    {
        this.friendship = friendship;
        if (update)
        {
            UpdateUI();
        }
    }
    public void SetSender(User sender)
    {
        this.sender = sender;
    }

    public void UpdateUI()
    {
        if (sender!= null)
        {
            usernameText.text = sender.username;
        }
    }

    public void ButtonAccept()
    {
        onAccept?.Invoke(null, sender);
    }
    public void ButtonDeny()
    {
        onAccept?.Invoke(null, sender);
    }
}

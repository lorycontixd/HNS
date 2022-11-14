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
    public ButtonManager acceptButton;
    public ButtonManager denyButton;

    public UnityEvent<Friendship, User> onAccept;
    public UnityEvent<Friendship, User> onDeny;

    private void Start()
    {
        acceptButton.onClick.AddListener(OnAccept);
        denyButton.onClick.AddListener(OnDeny);
    }

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

    public void OnAccept()
    {
        onAccept?.Invoke(friendship, sender);
    }
    public void OnDeny()
    {
        onDeny?.Invoke(friendship, sender);
    }
}

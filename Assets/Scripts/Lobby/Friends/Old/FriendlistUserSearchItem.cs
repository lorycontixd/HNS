using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class FriendlistUserSearchItem : MonoBehaviour
{
    public User user;
    public TextMeshProUGUI usernameText;
    public Button button;
    public UnityEvent<User> onItemClick;

    private void Start()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
        button.onClick.AddListener(OnButtonClick);
    }

    public void SetUser(User user, bool update = false)
    {
        this.user = user;
        if (update)
        {
            UpdateUI();
        }
    }
    public void UpdateUI()
    {
        usernameText.text = user.username;
    }
    public void OnButtonClick()
    {
        if (user != null) {
            onItemClick?.Invoke(user);
        }
    }
}

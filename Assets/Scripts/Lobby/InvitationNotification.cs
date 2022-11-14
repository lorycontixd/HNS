using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.MUIP;

public class InvitationNotification : MonoBehaviour
{
    public TextMeshProUGUI notificationText;
    public ButtonManager acceptButton;
    public ButtonManager denyButton;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Reset()
    {
        notificationText.text = string.Empty;
    }

    public void UpdateUI(string username)
    {
        notificationText.text = $"New invite from {username}";
    }

    public void AcceptButton()
    {
        InvitationManager.In.OnInvitationAccepted();
    }

    public void DenyButton()
    {
        InvitationManager.In.OnInvitationDeclined();
    }
}

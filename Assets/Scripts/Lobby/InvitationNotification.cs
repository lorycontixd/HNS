using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.MUIP;

public class InvitationNotification : MonoBehaviour
{
    public TextMeshProUGUI notificationTitle;
    public TextMeshProUGUI notificatoinSubtitle;
    public Button acceptButton;
    public Button denyButton;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Reset()
    {
        notificatoinSubtitle.text = string.Empty;
    }

    public void UpdateUI(string username)
    {
        notificatoinSubtitle.text = $"New invitation from {username}";
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

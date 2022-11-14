using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartyPlayerItem : MonoBehaviour
{
    public Player player;
    public User user = null;

    [Header("UI")]
    public TextMeshProUGUI usernameText;

    private void Start()
    {
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }
    public void UpdateUI()
    {
        usernameText.text = this.player.UserId;
    }
}

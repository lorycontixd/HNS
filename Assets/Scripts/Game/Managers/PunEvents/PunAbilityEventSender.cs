using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PunAbilityEventSender : MonoBehaviourPunCallbacks
{
    #region Singleton
    private static PunAbilityEventSender _instance;
    public static PunAbilityEventSender Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    public const byte AbilityInvisibilityCode = 10;

    public static void SendInvisibilityEvent(Player player, float duration)
    {
        object[] content = new object[] { player.UserId, duration };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(AbilityInvisibilityCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
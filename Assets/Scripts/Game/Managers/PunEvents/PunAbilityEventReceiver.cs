using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunAbilityEventReceiver : MonoBehaviourPunCallbacks
{
    #region Singleton
    private static PunAbilityEventReceiver _instance;
    public static PunAbilityEventReceiver Instance { get { return _instance; } }

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

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == PunAbilityEventSender.AbilityInvisibilityCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string player_userid = (string)data[0];
            Debug.Log("[PunEventReceiver->Invisibility] received user id: " + player_userid);
            float duration = (float)data[1];
            foreach (GameObject playerObj in GameNetworkManager.Instance.playerObjects)
            {
                Debug.Log("[PunEventReceiver->Invisibility] player: " + playerObj);
                PhotonView pv = playerObj.GetComponent<PhotonView>();
                Debug.Log("[PunEventReceiver->Invisibility] player userid: " + pv.Controller.UserId);
                Debug.Log("[PunEventReceiver->Invisibility] is user_id equal to received?: " + pv.Controller.UserId == player_userid);
                if (pv.Controller.UserId == player_userid)
                {
                    MyPlayer player = playerObj.GetComponent<MyPlayer>();
                    StartCoroutine(player.DeactivateRenderer(duration));
                    break;
                }
            }
        }
    }
}

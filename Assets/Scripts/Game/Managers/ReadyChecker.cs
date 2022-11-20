using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyChecker : MonoBehaviourPunCallbacks
{
    #region Singleton
    private static ReadyChecker _instance;
    public static ReadyChecker Instance { get { return _instance; } }

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

    public int readyPlayers;
    public List<Player> pregameReadyPlayers = new List<Player>();
    public bool allPlayersPregameReady = false;

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if ((bool)changedProps["IsPregameReady"])
        {
            pregameReadyPlayers.Add(targetPlayer);
            if (pregameReadyPlayers.Count == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                allPlayersPregameReady = true;
            }
        }
    }
}

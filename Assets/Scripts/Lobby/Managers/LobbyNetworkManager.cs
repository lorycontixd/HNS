using Michsky.LSS;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyNetworkManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    private static LobbyNetworkManager _instance;
    public static LobbyNetworkManager Instance { get { return _instance; } }

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

        // Makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    #endregion

    public User sessionUser = null; // Local player, set after login
    private bool isLoggedIn = false;

    public string gameVersion;
    public string appID;

    public LoadingScreenManager loadingManager;


    private void Update()
    {
        try
        {
            Debug.Log("Room: " + PhotonNetwork.CurrentRoom.Name);
        }
        catch
        {
            Debug.Log("Room: None");
        }
    }

    public void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {

            ConnectToRegion();
            PhotonNetwork.GameVersion = LobbyNetworkManager.Instance.gameVersion;
        }
    }
    public void ConnectToRegion()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connecting to region");
            AppSettings appSettings = new AppSettings();
            appSettings.FixedRegion = "eu";
            appSettings.AppIdRealtime = LobbyNetworkManager.Instance.appID;
            PhotonNetwork.ConnectUsingSettings(appSettings);
            PhotonNetwork.GameVersion = LobbyNetworkManager.Instance.gameVersion; ;
        }
    }

    public void LoadMainRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            loadingManager.LoadScene("Game");
            //PhotonNetwork.LoadLevel(1);
        }
    }

    // Session User
    #region Session User
    public User GetSessionUser()
    {
        return sessionUser;
    }
    public int GetSessionUserID()
    {
        return sessionUser.id;
    }
    public bool IsSessionUserNull()
    {
        return sessionUser == null;
    }
    public void SetLogin(bool isloggedin)
    {
        this.isLoggedIn = isloggedin;
    }
    public bool IsLoggedIn()
    {
        return isLoggedIn;
    }
    #endregion
}

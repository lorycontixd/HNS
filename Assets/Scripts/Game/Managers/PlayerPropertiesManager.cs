using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerPropertiesManager : MonoBehaviourPunCallbacks
{
    public Hashtable playerProps = new Hashtable();
    private bool initialized = false;

    private List<string> validProperties = new List<string>() { "Hunter", "FoundThisRound", "IsPregameReady"};

    public void Initialize()
    {
        playerProps = new Hashtable();
        playerProps.Add("Hunter", false);
        playerProps.Add("FoundThisRound", false);
        playerProps.Add("IsPregameReady", false);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
        initialized = true;
    }

    private void Start()
    {
    }

    public void SetRole(RoleType role)
    {
        if (initialized)
        {
            Initialize();
        }
        playerProps["Hunter"] = role == RoleType.HUNTER;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
    }

    public void PlayerFound()
    {
        if (!initialized)
        {
            Initialize();
        }
        playerProps["FoundThisRound"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
    }

    public void SetPlayerPregameReady(bool isready)
    {
        if (!initialized)
        {
            Initialize();
        }
        playerProps["IsPregameReady"] = isready;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
    }

    public void Reset()
    {
        Initialize();
    }
}

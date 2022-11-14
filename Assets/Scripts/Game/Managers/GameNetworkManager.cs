using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


public enum GameState
{
    WAITINGFORPLAYERS,
    PREGAME,
    COUNTPHASE,
    SEARCHPHASE,
    ENDPHASE
}


public class GameNetworkManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    private static GameNetworkManager _instance;
    public static GameNetworkManager Instance { get { return _instance; } }

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
        gameState = GameState.WAITINGFORPLAYERS;
    }
    #endregion

    public Camera standbyCamera;
    public GameObject playerPrefab;
    public List<Transform> spawnPoints = new List<Transform>();
    public MyPlayer myplayer;

    public GameState gameState;
    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    public Dictionary<int, PhotonView> playerViews = new Dictionary<int, PhotonView>();

    public Text debugText;
    [Header("Settings")]
    public bool debugMode;


    private IEnumerator Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            throw new UnityException("Joined game without being connected"); //Implement offline mode?
        }
        players = PhotonNetwork.CurrentRoom.Players;
        yield return new WaitUntil(() => players.Count > 1);
        SpawnPlayer();
        Debug.Log("CAN START!!");
    }
    private void Update()
    {
        if (PhotonNetwork.InRoom)
            debugText.text = PhotonNetwork.CurrentRoom.Name;
    }

    public void SpawnPlayer()
    {
        int sp = UnityEngine.Random.Range(0, spawnPoints.Count);
        int myid = players.FirstOrDefault(x => x.Value == PhotonNetwork.LocalPlayer).Key;
        GameObject clone = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[sp].position, spawnPoints[sp].localRotation);
        PhotonView pv = clone.GetComponent<PhotonView>();
        playerViews.Add(myid, pv);
        standbyCamera.gameObject.SetActive(false);
    }



    // Photon Callbacks
    #region Photon Callbacks
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        players = PhotonNetwork.CurrentRoom.Players;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        players = PhotonNetwork.CurrentRoom.Players;
    }
    #endregion

}

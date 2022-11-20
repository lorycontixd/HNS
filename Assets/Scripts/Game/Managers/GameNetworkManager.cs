using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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

    public Transform target;
    public GameObject scanArrow;

    public GameState gameState;
    private GameState lastGameState;

    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    public List<GameObject> playerObjects = new List<GameObject>();
    public Dictionary<int, PhotonView> playerViews = new Dictionary<int, PhotonView>();

    [Header("Game Variables")]
    private Player counterPlayer = null;

    [Header("Variables")]
    // Pregame
    public float pregameDuration = 15f;
    private float pregameTimestamp;
    public float countPhaseDuration = 20f;

    [Header("Events")]
    public UnityEvent<GameState, GameState> onGameStateChange; // <old, new>
    public UnityEvent<Player> onCounterPlayerSet;

    [Header("Settings")]
    public bool debugMode;


    private IEnumerator Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            throw new UnityException("Joined game without being connected"); //Implement offline mode?
        }
        players = PhotonNetwork.CurrentRoom.Players;
        yield return new WaitUntil(() => gameState == GameState.PREGAME);
        // Setup player
        SpawnPlayer();
        playerObjects = GameObject.FindGameObjectsWithTag("Player").ToList();
        myplayer = playerObjects.FirstOrDefault(obj => obj.GetComponent<PhotonView>().AmController).GetComponent<MyPlayer>();
        Debug.Log("MyPlayer: " + myplayer);
        SetupPhotonPlayer();
        myplayer.properties.SetPlayerPregameReady(true);

        // Start round
        onGameStateChange?.Invoke(GameState.WAITINGFORPLAYERS, GameState.PREGAME);
        UIManager.Instance.startGameButton.gameObject.SetActive(false);
        UIManager.Instance.waitingForHostTextParent.gameObject.SetActive(false);

        yield return new WaitUntil(() => ReadyChecker.Instance.allPlayersPregameReady);

        // Assign roles
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            Player player = MasterClientChooseCounter();
            photonView.RPC("SetHunter", RpcTarget.All, player);
        }
        // Assign abilities
        bool islocalplayerhunter = (bool)myplayer.properties.playerProps["Hunter"];
        Debug.Log("Is localplayer hunter: " + islocalplayerhunter);
        List<Ability> roleabilities = AbilityManager.Instance.GetRoleAbilities(islocalplayerhunter);
        AbilityController acontroller = myplayer.gameObject.GetComponent<AbilityController>();
        acontroller.AssignAbilities(roleabilities);
    }

    private void Update()
    {
        
        if (gameState == GameState.PREGAME)
        {
            if (lastGameState != GameState.PREGAME)
            {
                // Pregame started
                UIManager.Instance.gameTimerParent.gameObject.SetActive(true);
                UIManager.Instance.gameTimer.gameObject.SetActive(true);
                pregameTimestamp = pregameDuration;
                //StartCoroutine(SetCounterCoroutine());
            }
            else
            {
                pregameTimestamp -= Time.deltaTime;
                int timer = (int)pregameTimestamp;
                UIManager.Instance.gameTimer.text = timer.ToString();
                if (pregameTimestamp <= 0)
                {
                    gameState = GameState.COUNTPHASE;
                    onGameStateChange?.Invoke(GameState.PREGAME, GameState.COUNTPHASE);
                }
            }
        }

        if (gameState == GameState.COUNTPHASE)
        {
            if (lastGameState != GameState.COUNTPHASE) // Previous should be pregame phase
            {
                // First frame in count phase
                UIManager.Instance.gameTimerParent.gameObject.SetActive(true);
                UIManager.Instance.gameTimer.gameObject.SetActive(true);
                pregameTimestamp = countPhaseDuration;
            }
            else
            {
                pregameTimestamp -= Time.deltaTime;
                int timer = (int)pregameTimestamp;
                UIManager.Instance.gameTimer.text = timer.ToString();
                if (pregameTimestamp <= 0)
                {
                    gameState = GameState.SEARCHPHASE;
                    onGameStateChange?.Invoke(GameState.COUNTPHASE, GameState.SEARCHPHASE);
                }
            }
        }

        if (gameState == GameState.SEARCHPHASE)
        {
            if (lastGameState != GameState.SEARCHPHASE)
            {
                // First frame in search phase
                UIManager.Instance.gameTimerParent.gameObject.SetActive(false);
                UIManager.Instance.gameTimer.gameObject.SetActive(false);
            }
        }
        lastGameState = gameState;
    }

    public IEnumerator SetCounterCoroutine()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            yield return new WaitUntil(() => myplayer != null);
            Player player = MasterClientChooseCounter();
            photonView.RPC("SetCounterPlayer", RpcTarget.All, player);
        }
    }

    public void MasterClientStartGame()
    {
        photonView.RPC("SetGameState", RpcTarget.All, GameState.WAITINGFORPLAYERS, GameState.PREGAME);
    }


    /// <summary>
    /// 
    /// </summary>
    public void SpawnPlayer()
    {
        int sp = UnityEngine.Random.Range(0, spawnPoints.Count);
        int myid = players.FirstOrDefault(x => x.Value == PhotonNetwork.LocalPlayer).Key;
        GameObject clone = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[sp].position, spawnPoints[sp].localRotation);
        PhotonView pv = clone.GetComponent<PhotonView>();
        playerViews.Add(myid, pv);
        standbyCamera.gameObject.SetActive(false);
    }

    public void SetupPhotonPlayer()
    {
        myplayer.properties.Initialize();
    }


    public Player MasterClientChooseCounter()
    {
        int index = UnityEngine.Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount);
        Player counter = PhotonNetwork.CurrentRoom.Players.Values.ToList()[index];
        return counter;
        //return PhotonNetwork.LocalPlayer;
    }

    // Pun RPCs
    // SetGameState
    #region Pun RPCs
    [PunRPC]
    public void SetGameState(GameState oldState, GameState newState)
    {
        gameState = newState;
        onGameStateChange?.Invoke(oldState, newState);
    }

    [PunRPC]
    public void SetHunter(Player player, PhotonMessageInfo info)
    {
        counterPlayer = player;
        onCounterPlayerSet?.Invoke(player);

        Debug.LogFormat("Set counter player: {0} {1} {2} - {3}", info.Sender, info.photonView, info.SentServerTime, player.UserId);
        if (counterPlayer == PhotonNetwork.LocalPlayer)
        {
            //// I am the counter of this round
            // Update photon properties
            myplayer.properties.SetRole(RoleType.HUNTER);
        }
        else
        {
            myplayer.properties.SetRole(RoleType.RUNNER);
        }
    }
    #endregion

    

    public void StartGame()
    {

    }
    public IEnumerator StartGameCoroutine()
    {
        yield return null;
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
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
    }
    #endregion

}

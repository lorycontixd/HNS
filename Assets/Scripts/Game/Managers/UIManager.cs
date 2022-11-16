using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class UIManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    private static UIManager _instance;
    public static UIManager Instance { get { return _instance; } }

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

    [Header("Player Panel")]
    public Image playerImage;
    public TextMeshProUGUI roleText;
    public GameObject roleTextParent;

    [Header("Game Panel")]
    public TextMeshProUGUI gameStateText;
    public TextMeshProUGUI gameTimer;
    public GameObject gameTimerParent;

    [Header("Debug")]
    public Text roomDebugText;
    public TextMeshProUGUI debugText;

    [Header("Settings")]
    public bool debugMode;


    private void Start()
    {
        GameNetworkManager.Instance.onGameStateChange.AddListener(OnGameStateChanged);
        GameNetworkManager.Instance.onCounterPlayerSet.AddListener(OnCounterPlayerSet);

        roleTextParent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom)
            roomDebugText.text = PhotonNetwork.CurrentRoom.Name;

        SetGameStateText();
    }

    #region Game Panel
    public void SetGameStateText()
    {
        gameStateText.text = GameNetworkManager.Instance.gameState.ToString();
    }
    #endregion


    public void DebugText(string msg)
    {
        debugText.text = msg;
    }

    // Listeners
    // OnGameStateChanged, OnCounterPlayerSet
    #region Listeners
    public void OnGameStateChanged(GameState oldstate, GameState newstate){
        DebugText("State: " + newstate.ToString());
    }
    public void OnCounterPlayerSet(Player player)
    {
        bool isMe = player == PhotonNetwork.LocalPlayer;
        roleTextParent.gameObject.SetActive(true);
        if (isMe)
        {
            roleText.text = "Hunter";
        }
        else
        {
            roleText.text = "Runner";
        }
    }
    #endregion
}

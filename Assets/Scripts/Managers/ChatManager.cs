using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using UnityEngine;

public class ChatManager : MonoBehaviour, IChatClientListener, IDbListener
{
    #region Singleton
    private static ChatManager _instance;
    public static ChatManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    #endregion

    public string chatAppID;
    public string chatAppVersion;
    private ChatClient client;

    private bool isConnected = false;

    [Header("Channels")]
    public string masterChannel = "chat_channel_master";
    public string adminChannel = "chat_channel_admin";

    [Header("Settings")]
    public bool debugMode = false;
    public bool joinOnConnect = true;
    public bool sendMessageOnConnect = false;
    public int historyToFetchOnConnect = 5;


    private IEnumerator Start()
    {
        yield return new WaitUntil(() => LobbyNetworkManager.Instance.IsLoggedIn());
        Connect(LobbyNetworkManager.Instance.GetSessionUser().username);
        DatabaseManager.In.onQuery.AddListener(OnQuery);
    }

    private void Update()
    {
        if (this.client != null && isConnected)
        {
            this.client.Service();
        }
    }

    public void Connect(string authID)
    {
        Application.runInBackground = true;
        client = new ChatClient(this);

        client.ChatRegion = "eu"; // to be changed
        client.Connect(chatAppID, chatAppVersion, new AuthenticationValues(authID));
        isConnected = true;
    }

    // Testing
    #region Testing
    public void TestPrivateMessage(string message, string receiver)
    {
        if (!isConnected)
        {
            return;
        }
        this.client.SendPrivateMessage(receiver, message);
    }
    #endregion

    // Invitations
    #region Invitations
    public void InvitePlayerToParty(User player, Party party)
    {
        if (!isConnected)
        {
            Debug.LogError("Trying to send party invite message, but ChatClient is not connected");
            return;
        }
        this.client.SendPrivateMessage(player.username, $"party_invite_{party.token}");
    }
    #endregion


    // Chat callbacks
    #region Chat Callbacks
    public void OnConnected()
    {
        if (!isConnected)
        {
            return;
        }
        if (this.joinOnConnect)
        {
            this.client.Subscribe(masterChannel, historyToFetchOnConnect);
        }
        //this.client.AddFriends();
        this.client.SetOnlineStatus(ChatUserStatus.Online);
        TestPrivateMessage("Hello, this is a test", LobbyNetworkManager.Instance.GetSessionUser().username);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        if (!isConnected)
        {
            return;
        }
        if (debugMode)
        {
            Debug.Log($"[ChatManager:Debug] {message}");
        }
    }

    public void OnChatStateChange(ChatState state)
    {
        if (!isConnected)
        {
            return;
        }
    }

    public void OnDisconnected()
    {
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i=0; i < messages.Length; i++)
        {
            Debug.Log($"[ChatManager:NewMessage] [Channel:{channelName}] {senders[i]}: {messages[i]}");
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        if (!isConnected)
        {
            return;
        }
        if (sender != LobbyNetworkManager.Instance.GetSessionUser().username)
        {
            string msg = string.Empty;
            try
            {
                msg = message.ToString();
            }
            catch
            {
                Debug.LogError("Error converting private message received to string");
                return;
            }

            if (msg.Contains("party_invite_"))
            {
                string partytoken = msg.Substring(msg.IndexOf("party_invite_") + "party_invite_".Length);
                InvitationManager.In.OnInvitationReceived(partytoken);
            }
            else
            {
                LobbyMenuController.In.activeMenu.SpawnError($"New message from {sender}", msg);
            }
        }
    }


    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        // Receives status updates of friends
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        if (sendMessageOnConnect)
        {
            foreach(string channel in channels)
            {
                this.client.PublishMessage(channel, $"Hello, i just connected. I'm {LobbyNetworkManager.Instance.GetSessionUser().username}"); ;
            }
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
    }

    public void OnUserSubscribed(string channel, string user)
    {
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }
    #endregion


    // Handle disconnection
    #region Handle disconnection
    private void OnDestroy()
    {
        if (this.client != null)
        {
            // log

            // disconnect
            this.client.Disconnect();
        }
    }

    private void OnApplicationQuit()
    {
        if (this.client != null)
        {
            // log

            // disconnect
            this.client.Disconnect();
        }
    }

    #endregion

    public void OnQuery(ResultType type, QueryData data)
    {
    }
}

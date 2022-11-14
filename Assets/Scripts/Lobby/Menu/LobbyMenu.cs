using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Michsky.MUIP;


public enum LobbyMenuType
{
    NONE,
    LOGIN,
    REGISTER,
    USER,
    PARTY
}

public abstract class LobbyMenu : MonoBehaviourPunCallbacks
{
    [Header("Menu Info")]
    [HideInInspector] public int index;
    public LobbyMenuType type;
    public string menuName;
    public bool hasFriendlist;

    [Header("Components")]
    public GameObject mainPanel; // The panel where all menu components lie, in order to keep the script on a parent object always on.
    protected List<TMP_InputField> inputFields = new List<TMP_InputField>();
    private int selectedInput = 0;
    public NotificationManager menuNotification;


    public virtual void Start()
    {
        if (mainPanel == null)
        {
            Debug.LogError($"MainPanel not set for menu {menuName}");
        }
        index = (int)type;

        // Listen to database operations
        DatabaseManager.In.onQuery.AddListener(OnQuery);

        // Setup menu notification
        menuNotification.enableTimer = true;
        menuNotification.timer = 3f;
    }

    public virtual bool OnOpen()
    {
        mainPanel.SetActive(true);
        return true;
    }
    public virtual bool OnClose(bool leaveOpen = false)
    {
        mainPanel.SetActive(leaveOpen);
        return true;
    }

    public abstract void UpdateUI();

    public void NextInput()
    {
        if (inputFields.Count > 0)
        {
            // Deselect

            // Change index
            selectedInput = (selectedInput + 1) % inputFields.Count;

            // Select new
            inputFields[selectedInput].Select();
        }
    }

    public void SpawnError(string errorTitle, string errorDescription)
    {
        menuNotification.enabled = true;
        menuNotification.title = errorTitle;
        menuNotification.description = errorDescription;
        menuNotification.OpenNotification();
        menuNotification.UpdateUI();
    }

    public virtual void OnQuery(ResultType type, QueryData data)
    {
    }
}

using Michsky.MUIP;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Types;

public class LoginMenu : LobbyMenu
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Toggle rememberMeSwitch;
    public Button loginButton;
    public Button registerButton;
    public Button quitButton;



    [Header("Settings")]
    public bool debugMode;
    public string debugUsername = "Lore";
    public string debugPassword = "12345678";

    public override void Start()
    {
        base.Start();
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        inputFields.Add(usernameInput);
        inputFields.Add(passwordInput);
    }

    public override bool OnClose(bool leaveOpen = false)
    {
        base.OnClose(leaveOpen);
        return true;
    }

    public override bool OnOpen()
    {
        base.OnOpen();
        if (debugMode)
        {
            if (debugUsername.Length > 0 && debugPassword.Length > 0)
            {
                usernameInput.text = debugUsername;
                passwordInput.text = debugPassword;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public override void UpdateUI()
    {
    }


    // Photon Callbacks
    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }
    public override void OnJoinedLobby()
    {
        LobbyMenuController.In.SwitchMenu(LobbyMenuType.USER);
    }
    #endregion

    // Button Callbacks
    #region Button Callbacks
    public void LoginButton()
    {
        loginButton.interactable = false;
        DatabaseManager.In.CheckLogin(usernameInput.text, passwordInput.text);
    }
    public void RegisterButton()
    {
        LobbyMenuController.In.SwitchMenu(LobbyMenuType.REGISTER);
    }
    public void QuitButton()
    {
        // Pre-quit (e.g. log)

        // Quit
        Application.Quit();
    }
    #endregion

    public override void OnQuery(ResultType type, QueryData data)
    {
        base.OnQuery(type, data);
        if (data.queryType == QueryType.LOGIN)
        {
            MyLoginData logindata = (MyLoginData)data;
            if (type == ResultType.SUCCESS)
            {
                User user = logindata.user;
                AuthenticationValues auth = new AuthenticationValues();
                auth.UserId = user.username;
                PhotonNetwork.AuthValues = auth;
                LobbyNetworkManager.Instance.Connect();
                LobbyNetworkManager.Instance.sessionUser = user;
                LobbyNetworkManager.Instance.SetLogin(true);
            }
            else
            {
                Debug.Log(logindata.errorMessage);
                SpawnError("Login failed", logindata.errorMessage);
                loginButton.interactable = true;
            }
        }
    }
}

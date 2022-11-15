using Michsky.MUIP;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterMenu : LobbyMenu
{
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button registerButton;
    public Button backbutton;

    [Header("Settings")]
    public bool debugMode;
    public string debugUsername = "Lore";
    public string debugEmail = "loryconti1@gmail.com";
    public string debugPassword = "12345678";



    public override void Start()
    {
        base.Start();
        passwordInput.contentType = TMP_InputField.ContentType.Password;
        inputFields.Add(usernameInput);
        inputFields.Add(emailInput);
        inputFields.Add(passwordInput);
    }

    public override bool OnOpen()
    {
        bool baseSuccess = base.OnOpen();
        if (debugMode)
        {
            if (debugUsername.Length > 0 && debugPassword.Length > 0)
            {
                usernameInput.text = debugUsername;
                emailInput.text = debugEmail;
                passwordInput.text = debugPassword;
            }
            else
            {
                return false;
            }
        }
        return true;
    }
    public override bool OnClose(bool leaveOpen = false)
    {
        bool baseSuccess = base.OnClose(leaveOpen);
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
    public void RegisterButton()
    {
        string errorMessage = "";
        if (LoginValidator.ValidateRegistration(usernameInput.text, emailInput.text, passwordInput.text, out errorMessage))
        {
            DatabaseManager.In.RegisterUser(usernameInput.text, emailInput.text, passwordInput.text);
        }
        else
        {
            SpawnError("Invalid registration", errorMessage);
        }
    }
    public void BackButton()
    {
        LobbyMenuController.In.SwitchMenu(LobbyMenuType.LOGIN);
    }
    #endregion

    public override void OnQuery(ResultType type, QueryData data)
    {
        base.OnQuery(type, data);
        if (data.queryType == QueryType.REGISTER)
        {
            MyRegisterData registerdata = (MyRegisterData)data;
            if (type == ResultType.FAIL)
            {
                SpawnError("Registration failed", registerdata.errorMessage);
            }
            else
            {
                User user = registerdata.user;
                AuthenticationValues auth = new AuthenticationValues();
                auth.UserId = registerdata.user.username;
                PhotonNetwork.AuthValues = auth;
                LobbyNetworkManager.Instance.Connect();
                LobbyNetworkManager.Instance.sessionUser = user;
                LobbyNetworkManager.Instance.SetLogin(true);
                // Redirected to Photon callback -> OnConnectedToMaster
            }
        }
    }
}

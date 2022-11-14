using Photon.Realtime;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public struct ParseFriendshipsResult
{
    public List<User> friends;
    public List<User> unacceptedFriends;
    public List<User> pendingFriends;

    public ParseFriendshipsResult(List<User> friends, List<User> unacceptedFriends, List<User> pendingFriends)
    {
        this.friends = friends;
        this.unacceptedFriends = unacceptedFriends;
        this.pendingFriends = pendingFriends;
    }
}


public class DatabaseManager : Singleton<DatabaseManager>
{
    [Tooltip("Web app address")] public string appAddress;
    [Tooltip("Database address")] public string dbAddress;
    public UnityEvent<ResultType, QueryData> onQuery;

    public void RegisterUser(string username, string email, string password)
    {
        StartCoroutine(RegisterUserCoroutine(username, email, password));
    }

    private IEnumerator RegisterUserCoroutine(string username, string email, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("email", email);
        form.AddField("password", password);

        Debug.Log("Sending request with form");
        UnityWebRequest www = UnityWebRequest.Post(appAddress + "insertuser.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("1Register User text: " + www.error);
            onQuery?.Invoke(ResultType.FAIL, new MyRegisterData(www.error));
        }
        else
        {
            string text = www.downloadHandler.text;
            Debug.Log("2Register User text: " + text);
            if (text.Contains("Error"))
            {
                if (text == "Error0")
                {
                    onQuery?.Invoke(ResultType.FAIL, new MyRegisterData("User already exists"));
                }
                else
                {
                    onQuery?.Invoke(ResultType.FAIL, new MyRegisterData(text));
                }
            }
            else
            {

                User user = ParseUser(text);
                onQuery?.Invoke(ResultType.SUCCESS, new MyRegisterData(user));
            }
        }
    }

    public void CheckLogin(string username, string password)
    {
        StartCoroutine(CheckLoginCoroutine(username, password));
    }

    private IEnumerator CheckLoginCoroutine(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        UnityWebRequest www = UnityWebRequest.Post(appAddress + "checklogin.php", form);
        yield return www.SendWebRequest();

        //int randomNumber = Random.Range(0, 9999);
        //onQuery?.Invoke(ResultType.SUCCESS, new MyLoginData(new User(0, $"Player{randomNumber}", "Temporary", "Account", "email", "hashedpass")));

        if (www.result != UnityWebRequest.Result.Success)
        {
            onQuery?.Invoke(ResultType.FAIL, new MyLoginData("Unexpected error while connecting to server. Contact administration."));
        }
        else
        {
            string text = www.downloadHandler.text;
            if (!text.Contains("Error"))
            {
                User user = ParseUser(text);
                Debug.Log("Login successful");
                onQuery?.Invoke(ResultType.SUCCESS, new MyLoginData(user));
            }
            else
            {
                onQuery?.Invoke(ResultType.FAIL, new MyLoginData(text));
            }
        }
    }

    public IEnumerator UpdateOnlineStatus(bool isonline)
    {
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post(appAddress + "useronlineupdate.php", form);
        yield return www.SendWebRequest();

        //int randomNumber = Random.Range(0, 9999);
        //onQuery?.Invoke(ResultType.SUCCESS, new MyLoginData(new User(0, $"Player{randomNumber}", "Temporary", "Account", "email", "hashedpass")));

        if (www.result != UnityWebRequest.Result.Success)
        {
            onQuery?.Invoke(ResultType.FAIL, new MyLoginData("Unexpected error while connecting to server. Contact administration."));
        }
        else
        {
            string text = www.downloadHandler.text;
            if (!text.Contains("Error"))
            {
                User user = ParseUser(text);
                Debug.Log("Login successful");
                onQuery?.Invoke(ResultType.SUCCESS, new MyLoginData(user));
            }
            else
            {
                onQuery?.Invoke(ResultType.FAIL, new MyLoginData("User does not exist"));
            }
        }
    }

    public IEnumerator SearchUserByUsernameCoroutine(string username, string extrainfo = "")
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);

        UnityWebRequest www = UnityWebRequest.Post(appAddress + "searchuserbyusername.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            onQuery?.Invoke(ResultType.FAIL, new MySearchUserData("Unexpected error while connecting to server. Contact administration."));
        }
        else
        {
            string text = www.downloadHandler.text;
            if (!text.Contains("Error"))
            {
                User user = ParseUser(text);
                onQuery?.Invoke(ResultType.SUCCESS, new MySearchUserData(user, extrainfo));
            }
            else
            {
                onQuery?.Invoke(ResultType.FAIL, new MySearchUserData("User does not exist"));
            }
        }
    }
    public IEnumerator SearchUserByIDCoroutine(int id, string extrainfo = "")
    {
        WWWForm form = new WWWForm();
        form.AddField("userid", id);

        UnityWebRequest www = UnityWebRequest.Post("https://3fmetaverse.azurewebsites.net/php/searchuserbyid.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("searchuserbyid failed: " + www.downloadHandler.text);
            onQuery?.Invoke(ResultType.FAIL, new MySearchUserData("Unexpected error while connecting to server. Contact administration."));
        }
        else
        {
            string text = www.downloadHandler.text;
            if (!text.Contains("Error"))
            {
                User user = ParseUser(text);
                onQuery?.Invoke(ResultType.SUCCESS, new MySearchUserData(user, extrainfo));
            }
            else
            {
                onQuery?.Invoke(ResultType.FAIL, new MySearchUserData("User does not exist"));
            }
        }
    }


    public IEnumerator SearchUsersCoroutine(string firstname, string lastname)
    {
        WWWForm form = new WWWForm();
        form.AddField("firstname", firstname);
        form.AddField("lastname", lastname);

        Debug.Log("Sending request for search users to " + appAddress + "friendships/searchfriends.php");
        UnityWebRequest www = UnityWebRequest.Post(appAddress + "friendships/searchfriends.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            onQuery?.Invoke(ResultType.FAIL, new MySearchUserData("Unexpected error while connecting to server. Contact administration."));
        }
        else
        {
            string text = www.downloadHandler.text;
            if (!text.Contains("Error"))
            {
                Debug.Log("Search users co text: " + text);
                List<User> users = ParseMultipleUsers(text);
                onQuery?.Invoke(ResultType.SUCCESS, new MySearchUsersData(users));
            }
            else
            {
                onQuery?.Invoke(ResultType.FAIL, new MySearchUserData("User does not exist"));
            }
        }
    }

    #region Friendships
    public IEnumerator SendFriendship(User sender, User receiver)
    {
        WWWForm form = new WWWForm();
        form.AddField("senderid", sender.id);
        form.AddField("receiverid", receiver.id);

        UnityWebRequest www = UnityWebRequest.Post(appAddress + "friendships/addfriendship.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error friendship:  " + www.downloadHandler.text);
            onQuery?.Invoke(ResultType.FAIL, new MyAddFriendshipData("Unexpected error while connecting to server. Contact administration."));
        }
        else
        {
            string text = www.downloadHandler.text;
            if (!text.Contains("Error"))
            {
                onQuery?.Invoke(ResultType.SUCCESS, new MyAddFriendshipData(sender, receiver));
                Debug.Log("Friendship request success: " + www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Friendship request fail: " + www.downloadHandler.text);
                onQuery?.Invoke(ResultType.FAIL, new MyAddFriendshipData($"Error creating friendship between users {sender.username} and {receiver.username}"));
            }
        }
    }

    public IEnumerator AcceptFriendship(User sender, User receiver)
    {
        WWWForm form = new WWWForm();
        form.AddField("senderid", sender.id);
        form.AddField("receiverid", receiver.id);

        UnityWebRequest www = UnityWebRequest.Post(appAddress + "friendships/acceptfriendship.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error friendship:  " + www.downloadHandler.text);
            onQuery?.Invoke(ResultType.FAIL, new MyAcceptFriendship("Unexpected error while connecting to server. Contact administration."));
        }
        else
        {
            string text = www.downloadHandler.text;
            if (!text.Contains("Error"))
            {
                onQuery?.Invoke(ResultType.SUCCESS, new MyAcceptFriendship(sender, receiver));
                Debug.Log("Friendship request success: " + www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Friendship request fail: " + www.downloadHandler.text);
                onQuery?.Invoke(ResultType.FAIL, new MyAcceptFriendship($"Error creating friendship between users {sender.username} and {receiver.username}"));
            }
        }
    }

    public IEnumerator RemoveFriendship(User sender, User receiver)
    {
        WWWForm form = new WWWForm();
        form.AddField("senderid", sender.id);
        form.AddField("receiverid", receiver.id);

        UnityWebRequest www = UnityWebRequest.Post(appAddress + "friendships/removefriendship.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error friendship:  " + www.downloadHandler.text);
            onQuery?.Invoke(ResultType.FAIL, new MyRemoveFriendship("Unexpected error while connecting to server. Contact administration."));
        }
        else
        {
            string text = www.downloadHandler.text;
            if (!text.Contains("Error"))
            {
                onQuery?.Invoke(ResultType.SUCCESS, new MyRemoveFriendship(sender, receiver));
                Debug.Log("Friendship remove success: " + www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Friendship remove fail: " + www.downloadHandler.text);
                onQuery?.Invoke(ResultType.FAIL, new MyRemoveFriendship($"Error creating friendship between users {sender.username} and {receiver.username}"));
            }
        }
    }
    /*
    public IEnumerator GetFriendships(int userid)
    {
        WWWForm form = new WWWForm();
        form.AddField("userid", userid);

        UnityWebRequest www = UnityWebRequest.Post(appAddress + "friendships/getfriendships.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error friendship:  " + www.downloadHandler.text);
            onQuery?.Invoke(ResultType.FAIL, new MyGetFriendshipsData("Unexpected error while connecting to server. Contact administration."));
        }
        else
        {
            string text = www.downloadHandler.text;
            if (!text.Contains("Error"))
            {
                Debug.Log("friendship text: " + text);
                List<Friendship> friendships = ParseFriendships(text);
                Debug.Log("Friendship request success: " + www.downloadHandler.text);
                onQuery?.Invoke(ResultType.SUCCESS, new MyGetFriendshipsData(friendships));
            }
            else
            {
                Debug.Log("Friendship request fail: " + www.downloadHandler.text);
                onQuery?.Invoke(ResultType.FAIL, new MyGetFriendshipsData($"Error fetching friendship for user with ID " + userid));
            }
        }
    }*/

    public IEnumerator GetFriendships2(int userid)
    {
        WWWForm form = new WWWForm();
        form.AddField("userid", userid);

        UnityWebRequest www = UnityWebRequest.Post(appAddress + "friendships/getfriendships2.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            onQuery?.Invoke(ResultType.FAIL, new MyGetFriendshipsData("Unexpected error while connecting to server. Contact administration."));
        }
        else
        {
            string text = www.downloadHandler.text;
            if (!text.Contains("Error"))
            {
                ParseFriendshipsResult result = ParseFriendships2(text);
                onQuery?.Invoke(ResultType.SUCCESS, new MyGetFriendshipsData(result));
            }
            else
            {
                onQuery?.Invoke(ResultType.FAIL, new MyGetFriendshipsData($"Error fetching friendship for user with ID " + userid));
            }
        }
    }
    #endregion

    #region Party
    public IEnumerator AddParty(string roomname, RoomOptions createdroom, string password, int creatorid)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", roomname);
        form.AddField("password", password);
        form.AddField("leaderid", creatorid);
        form.AddField("isvisible", createdroom.IsVisible ? 1 : 0);
        form.AddField("isopen", createdroom.IsOpen ? 1 : 0);


        UnityWebRequest www = UnityWebRequest.Post(appAddress + "parties/addparty.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error party add:  " + www.downloadHandler.text);
            onQuery?.Invoke(ResultType.FAIL, new MyAddPartyData("Unexpected error while connecting to server. Contact administration."));
        }
        else
        {
            string text = www.downloadHandler.text;
            Debug.Log("Add party result text: " + text);
            if (!text.Contains("Error"))
            {
                Party party = ParseParty(text);
                onQuery?.Invoke(ResultType.SUCCESS, new MyAddPartyData(party));
                Debug.Log("add party request success: " + www.downloadHandler.text);
            }
            else
            {
                Debug.Log("add party request fail: " + www.downloadHandler.text);
                onQuery?.Invoke(ResultType.FAIL, new MyAddPartyData("Failed to create a new party"));
            }
        }
    }

    public IEnumerator SearchParty(string partytoken)
    {
        WWWForm form = new WWWForm();
        form.AddField("partytoken", partytoken);

        UnityWebRequest www = UnityWebRequest.Post(appAddress + "parties/searchparty.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error party search:  " + www.downloadHandler.text);
            onQuery?.Invoke(ResultType.FAIL, new MySearchPartyData("Unexpected error while connecting to server. Contact administration."));
        }
        else
        {
            string text = www.downloadHandler.text;
            Debug.Log("Seach party result text: " + text);
            if (!text.Contains("Error"))
            {
                Party party = ParseParty(text);
                onQuery?.Invoke(ResultType.SUCCESS, new MySearchPartyData(party));
                Debug.Log("Party search success: " + www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Party search fail: " + www.downloadHandler.text);
                onQuery?.Invoke(ResultType.FAIL, new MyAddPartyData("Failed to search party"));
            }
        }
    }
    #endregion


    private Party ParseParty(string text)
    {
        Debug.Log("Parsing party text: " + text);
        string[] values = text.Split('\t');
        if (values.Length != 8)
        {
            Debug.LogError("Error: party values must be 8");
        }
        int id = int.Parse(values[0]);
        string roomname = values[1];
        string partytoken = values[2];
        string hashedpassword = values[3];
        int leaderid = int.Parse(values[4]);
        int playercount = int.Parse(values[5]);
        bool isvisible = Convert.ToBoolean(int.Parse(values[6]));
        bool isopen = Convert.ToBoolean(int.Parse(values[7]));
        RoomOptions opts = new RoomOptions() { MaxPlayers = (byte)16, IsOpen = isopen, IsVisible = isvisible, PublishUserId = true };
        return new Party(id, roomname, opts, partytoken, hashedpassword, playercount, leaderid, DateTime.Now, DateTime.Now);
    }

    private User ParseUser(string text)
    {
        string[] values = text.Split('\t');
        if (values.Length != 8)
        {
            Debug.LogError("Error: user values must be 8");
        }
        int id = int.Parse(values[0]);
        int roleid = int.Parse(values[1]);
        string username = values[2];
        string email = values[3];
        string hashedpassword = values[4];
        bool isonline = Convert.ToBoolean(int.Parse(values[5]));
        bool isactive = Convert.ToBoolean(int.Parse(values[6]));
        int partyid = int.Parse(values[7]);
        User user = new User(id, roleid, username, email, hashedpassword, isonline, isactive, partyid);
        return user;
    }
    private List<User> ParseMultipleUsers(string text)
    {
        List<User> result = new List<User>();
        string[] users = text.Split(new string[] { "<br />" }, StringSplitOptions.None);
        foreach (string userstring in users)
        {
            if (userstring != string.Empty)
            {
                User user = ParseUser(userstring);
                result.Add(user);
            }
        }
        return result;
    }
    private List<Friendship> ParseFriendships(string text)
    {
        List<Friendship> result = new List<Friendship>();
        string[] friendships = text.Split(new string[] { "<br />" }, StringSplitOptions.None);
        foreach (string friendshipstring in friendships)
        {
            if (friendshipstring != string.Empty)
            {
                Friendship friendship = ParseFriendship(friendshipstring);
                result.Add(friendship);
            }
        }
        return result;
    }

    

    private ParseFriendshipsResult ParseFriendships2(string text)
    {
        List<User> friends = new List<User>();
        List<User> unacceptedFriends = new List<User>();
        List<User> pendingFriends = new List<User>();

        string[] users = text.Split(new string[] { "<br />" }, StringSplitOptions.None);
        users = users.Take(users.Length - 1).ToArray();
        foreach (string userstring in users)
        {
            Debug.Log("userstring: " + userstring);
            string[] result = userstring.Split(new string[] { "\t\t" }, StringSplitOptions.None);
            Debug.Log("results length: " + result.Length);
            if (result.Length != 2)
            {
                throw new UnityException("Error while splitting friendships: all values");
            }
            string usertext = result[0];
            string friendshiptext = result[1];
            string[] uservals = usertext.Split('\t');
            string[] friendsvals = friendshiptext.Split('\t');
            if (friendsvals.Length != 2)
            {
                Debug.LogError("Error while splitting friendships: friendship values");
            }
            User user = ParseUser(usertext);
            bool isaccepted = Convert.ToBoolean(int.Parse(friendsvals[0]));
            int senderid = int.Parse(friendsvals[1]);
            bool amisender = senderid == LobbyNetworkManager.Instance.GetSessionUserID();

            if (isaccepted)
            {
                friends.Add(user);
            }
            else
            {
                if (amisender)
                {
                    pendingFriends.Add(user);
                }
                else
                {
                    unacceptedFriends.Add(user);
                }
            }
        }
        return new ParseFriendshipsResult(friends, unacceptedFriends, pendingFriends);
    }

    private Friendship ParseFriendship(string friendshipstring)
    {
        string[] values = friendshipstring.Split('\t');
        if (values.Length != 4)
        {
            Debug.LogError("Error: friendship values must be 6");
        }
        Debug.Log("===> " + values[3]);
        int id = int.Parse(values[0]);
        int senderid = int.Parse(values[1]);
        int receiverid = int.Parse(values[2]);
        bool isaccepted = Convert.ToBoolean(int.Parse(values[3]));
        return new Friendship(id, senderid, receiverid, isaccepted, "", "");
    }
}
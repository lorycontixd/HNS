using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResultType
{
    FAIL,
    SUCCESS
}
public enum QueryType
{
    LOGIN,
    REGISTER,
    SEARCHUSER,
    SEARCHUSERS,
    SENDFRIENDSHIP,
    GETFRIENDSHIPS,
    ACCEPTFRIENDSHIP,
    DENYFRIENDSHIP,
    REMOVEFRIENDSHIP,
    ADDPARTY,
    SEARCHPARTY,
    REMOVEPARTY,
    ENTERPARTY,
    LEAVEPARTY
}

public abstract class QueryData
{
    public QueryType queryType;
    public string extraInfo;
    public string errorMessage;

    public bool IsSuccess { get { return errorMessage == string.Empty; } }
}

public class MyLoginData : QueryData
{
    public User user;

    public MyLoginData(User user, string extrainfo = "")
    {
        this.user = user;
        this.extraInfo = extrainfo;
        this.errorMessage = string.Empty;
        queryType = QueryType.LOGIN;
    }
    public MyLoginData(string errorMessage)
    {
        this.user = null;
        this.extraInfo = "error";
        this.errorMessage = errorMessage;
        queryType = QueryType.LOGIN;
    }
}
public class MyRegisterData : QueryData
{
    public User user;

    public MyRegisterData(User user, string extrainfo = "")
    {
        this.user = user;
        this.extraInfo = extrainfo;
        this.errorMessage = string.Empty;
        queryType = QueryType.REGISTER;
    }
    public MyRegisterData(string errorMessage)
    {
        this.user = null;
        this.errorMessage = errorMessage;
        queryType = QueryType.REGISTER;
    }
}

public class MySearchUserData : QueryData
{
    public User user;

    public MySearchUserData(User user, string extrainfo = "")
    {
        this.user = user;
        this.extraInfo = extrainfo;
        this.errorMessage = string.Empty;
        queryType = QueryType.SEARCHUSER;
    }
    public MySearchUserData(string errorMessage)
    {
        this.user = null;
        this.errorMessage = errorMessage;
        queryType = QueryType.SEARCHUSER;
    }
}

public class MySearchUsersData : QueryData
{
    public List<User> users;

    public MySearchUsersData(List<User> users, string extrainfo = "")
    {
        this.users = users;
        this.extraInfo = extrainfo;
        this.errorMessage = string.Empty;
        queryType = QueryType.SEARCHUSERS;
    }
    public MySearchUsersData(string errorMessage)
    {
        this.users = null;
        this.errorMessage = errorMessage;
        queryType = QueryType.SEARCHUSERS;
    }
}

public class MyAddFriendshipData : QueryData
{
    public User sender;
    public User receiver;

    public MyAddFriendshipData(User sender, User receiver, string extrainfo = "")
    {
        this.sender = sender;
        this.receiver = receiver;
        this.extraInfo = extrainfo;
        this.errorMessage = string.Empty;
        queryType = QueryType.SENDFRIENDSHIP;
    }
    public MyAddFriendshipData(string errorMessage)
    {
        this.sender = null;
        this.receiver = null;
        this.errorMessage = errorMessage;
        queryType = QueryType.SENDFRIENDSHIP;
    }
}

public class MyGetFriendshipsData : QueryData
{
    public ParseFriendshipsResult friends;

    public MyGetFriendshipsData(ParseFriendshipsResult userfriendships, string extrainfo = "")
    {
        this.friends = userfriendships;
        this.extraInfo = extrainfo;
        this.errorMessage = string.Empty;
        queryType = QueryType.GETFRIENDSHIPS;
    }
    public MyGetFriendshipsData(string errorMessage)
    {
        this.friends = new ParseFriendshipsResult();
        this.errorMessage = errorMessage;
        queryType = QueryType.GETFRIENDSHIPS;
    }
}

public class MyAcceptFriendship : QueryData
{
    public User sender;
    public User receiver;

    public MyAcceptFriendship(User sender, User receiver, string extrainfo = "")
    {
        this.sender = sender;
        this.receiver = receiver;
        this.extraInfo = extrainfo;
        this.errorMessage = string.Empty;
        queryType = QueryType.ACCEPTFRIENDSHIP;
    }
    public MyAcceptFriendship(string errorMessage)
    {
        this.sender = null;
        this.receiver = null;
        this.errorMessage = errorMessage;
        queryType = QueryType.ACCEPTFRIENDSHIP;
    }
}

public class MyRemoveFriendship : QueryData
{
    public User sender;
    public User receiver;

    public MyRemoveFriendship(User sender, User receiver, string extrainfo = "")
    {
        this.sender = sender;
        this.receiver = receiver;
        this.extraInfo = extrainfo;
        this.errorMessage = string.Empty;
        queryType = QueryType.REMOVEFRIENDSHIP;
    }
    public MyRemoveFriendship(string errorMessage)
    {
        this.sender = null;
        this.receiver = null;
        this.errorMessage = errorMessage;
        queryType = QueryType.REMOVEFRIENDSHIP;
    }
}


public class MyAddPartyData : QueryData
{
    public Party party;

    public MyAddPartyData(Party party, string extrainfo = "")
    {
        this.party = party;
        this.extraInfo = extrainfo;
        this.errorMessage = string.Empty;
        this.queryType = QueryType.ADDPARTY;
    }

    public MyAddPartyData(string errorMessage)
    {
        this.party = null;
        this.errorMessage = string.Empty;
        this.queryType = QueryType.ADDPARTY;
    }
}


public class MySearchPartyData : QueryData
{
    public Party party;

    public MySearchPartyData(Party party, string extrainfo = "")
    {
        this.party = party;
        this.extraInfo = extrainfo;
        this.errorMessage = string.Empty;
        this.queryType = QueryType.SEARCHPARTY;
    }
    public MySearchPartyData(string errorMessage)
    {
        this.party = null;
        this.errorMessage = string.Empty;
        this.queryType = QueryType.SEARCHPARTY;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class User
{
    public int id;
    public int roleid;
    public string username;
    public string email;
    public string password;
    public bool isOnline;
    public bool isActive;
    public int partyID;

    public UserFriends userFriends;

    public User(int id, int roleid, string username, string email, string password, bool isOnline, bool isActive, int partyID)
    {
        this.id = id;
        this.username = username;
        this.email = email;
        this.password = password;
        this.isOnline = isOnline;
        this.isActive = isActive;
        this.partyID = partyID;
    }

    public bool IsNull()
    {
        return id == 0 && username == string.Empty && email == string.Empty;
    }

    private bool IsEqual(User other)
    {
        return other is User
            && other.id.Equals(id)
            && other.username.Equals(username);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Friendship
{
    public int id;
    public int senderid;
    public int receiverid;
    public bool isaccepted;
    public string creationdate;
    public string lastupdate;

    public Friendship(int id, int senderid, int receiverid, bool isaccepted, string creationdate, string lastupdate)
    {
        this.id = id;
        this.senderid = senderid;
        this.receiverid = receiverid;
        this.isaccepted = isaccepted;
        this.creationdate = creationdate;
        this.lastupdate = lastupdate;
    }
}

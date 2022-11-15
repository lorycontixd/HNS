using Photon.Realtime;
using System;

[Serializable]
public class Party
{
    public string partyname;
    public int id;
    public RoomOptions roomopts;
    public string token;
    public string password;
    public int playercount;
    public int creatorid;
    public string creatorusername;
    public DateTime creationdate;
    public DateTime lastupdate;
    public bool IsVisible { get {return roomopts.IsVisible; } }
    public bool IsOpen { get {return roomopts.IsOpen; } }

    private string phpdateformat = "d-m-Y H:i:s";
    private string localdateformat = "dd-MM-yyyy HH:mm:ss";

    public Party(int id, string name, RoomOptions roomopts, string token, string password, int playercount, int creatorid, string creatorusername, DateTime creationdate, DateTime lastupdate)
    {
        this.creatorusername = creatorusername;
        this.partyname = name;
        this.roomopts = roomopts;
        this.token = token;
        this.password = password;
        this.playercount = playercount;
        this.creatorid = creatorid;
        this.creationdate = creationdate;
        this.lastupdate = lastupdate;
    }

    public bool IsNull()
    {
        return partyname == string.Empty && id == 0 && token == string.Empty;
    }

    /*public override string ToString()
    {
        return $"party_{token}_{creatorid}";
    }*/
}

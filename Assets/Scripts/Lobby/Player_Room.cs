using System.Collections;
using System.Collections.Generic;

public class Player_Room
{
    public int ID { get; }
    public string Username { get; }
    public bool ready { get; }

    public Player_Room(int iD, string username, bool ready)
    {
        ID = iD;
        Username = username;
        this.ready = ready;
    }


}

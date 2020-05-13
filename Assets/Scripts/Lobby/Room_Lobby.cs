using System.Collections;
using System.Collections.Generic;


public class Room_Lobby 
{
    public int ID { get; }
    public int map { get; }
    public int nowPlayerNum { get; }
    public int maxPlayerNum { get; }

    public Room_Lobby(int iD, int map, int nowPlayerNum, int maxPlayerNum)
    {
        ID = iD;
        this.map = map;
        this.nowPlayerNum = nowPlayerNum;
        this.maxPlayerNum = maxPlayerNum;
    }
}

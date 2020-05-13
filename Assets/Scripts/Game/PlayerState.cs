using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    public int playerID;
    public string username;
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 lookTarget;
    public int motion;
    public int move;
    public int attack;
    public int HP;
    public int curAmmo;
    public int bagAmmo;
    public int LV;
    public int EXP;

    public PlayerState()
    {

    }

    public PlayerState(int playerID,string username,Vector3 pos,Vector3 rot,Vector3 lookTarget,int motion,
                        int move,int attack,int HP,int curAmmo,int bagAmmo,int LV,int EXP)
    {
        this.playerID = playerID;
        this.username = username;
        this.pos = pos;
        this.rot = rot;
        this.lookTarget = lookTarget;
        this.motion = motion;
        this.move = move;
        this.attack = attack;
        this.HP = HP;
        this.curAmmo = curAmmo;
        this.bagAmmo = bagAmmo;
        this.LV = LV;
        this.EXP = EXP;
    }
}

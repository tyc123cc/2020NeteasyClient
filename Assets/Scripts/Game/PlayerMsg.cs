using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMsg
{
    public static List<PlayerMsg> players = new List<PlayerMsg>();

    /// <summary>
    /// 用户所在房间号
    /// </summary>
    public int roomID;

    /// <summary>
    /// 用户在本局游戏的ID
    /// </summary>
    public int playerID;

    /// <summary>
    /// 用户出生的位置
    /// </summary>
    public Vector3 initPos;

    /// <summary>
    /// 用户出生朝向
    /// </summary>
    public Vector3 initRot;

    /// <summary>
    /// 用户名
    /// </summary>
    public string username = "";

    /// <summary>
    /// 用户当前血量
    /// </summary>
    public int HP = 100;

    /// <summary>
    /// 用户弹匣子弹数
    /// </summary>
    public int curAmmo = 30;

    /// <summary>
    /// 用户背包子弹数
    /// </summary>
    public int bagAmmo = 70;

    /// <summary>
    /// 用户等级
    /// </summary>d
    public int LV = 1;

    /// <summary>
    /// 用户经验值
    /// </summary>
    public int EXP = 0;
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetTextParse : MonoBehaviour
{
    public static int MSG_UNMEANING = 0;
    public static int MSG_REGISTER = 1;
    public static int MSG_LOGIN = 2;
    public static int MSG_LOBBY = 3;
    public static int MSG_QUIT = 4;
    public static int MSG_RECONNECT = 5;
    public static int MSG_GAME = 6;
    public static int MSG_SYNCHRONIZATION = 7;

    public static int MSG_SYN_DELAY = 1;
    public static int MSG_SYN_TIME = 2;

    public static int MSG_LOBBY_UNMEANING = 0;
    public static int MSG_LOBBY_ROOM = 1;
    public static int MSG_LOBBY_JOIN = 2;
    public static int MSG_LOBBY_QUIT = 3;
    public static int MSG_LOBBY_GETLOBBYROOM = 4;
    public static int MSG_LOBBY_START = 5;

    public static int MSG_GAME_UNMEANING = 0;
    public static int MSG_GAME_START = 1;
    public static int MSG_GAME_UPDATE = 2;
    public static int MSG_GAME_LOAD = 3;
    public static int MSG_GAME_ATTACK = 4;
    public static int MSG_GAME_ENEMY = 5;
    public static int MSG_GAME_PROP = 6;
    public static int MSG_GAME_END = 7;
    public static int MSG_GAME_BACK = 8;

    public GameObject m_quitImage;

    public LoginSceneManager m_loginManager;
    public LobbySceneManager m_lobbyManager;
    public GameSceneManager m_gameManager;
    public LoadSceneManager m_loadManager;

    networkSocket m_socket;

    private void Start()
    {
        m_socket = GameObject.Find("Network").GetComponent<networkSocket>();
        m_quitImage = Resources.Load("Prefabs\\QuitImage") as GameObject;
        if (SceneManager.GetActiveScene().name == "Login") // 登录界面
        {
            m_loginManager = GameObject.Find("LoginSceneManager").GetComponent<LoginSceneManager>();
        }
        else if (SceneManager.GetActiveScene().name == "Lobby") // 大厅界面
        {
            m_lobbyManager = GameObject.Find("LobbySceneManager").GetComponent<LobbySceneManager>();
        }
        else if (SceneManager.GetActiveScene().name.Contains("Game"))// 游戏界面
        {
            m_gameManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
        }
        else if (SceneManager.GetActiveScene().name == "Load") // 加载界面
        {
            m_loadManager = GameObject.Find("LoadSceneManager").GetComponent<LoadSceneManager>();
        }
    }

    public void parse(string serverMsg)
    {
        if (serverMsg[0] != '@' || serverMsg[serverMsg.Length - 1] != '#') // 收到的信息不符合协议规范
        {
            return;
        }
        string[] msgs = simple(serverMsg);
        foreach (var m in msgs)
        {
            // 判断消息类型
            int type = parseType(m);
            // 去除类型前缀
            string msg = m.Remove(0, 1);
            if (type == MSG_UNMEANING)
            {
                return;
            }
            else if (type == MSG_REGISTER)
            {
                parseRegister(msg);
            }
            else if (type == MSG_LOGIN)
            {
                parseLogin(msg);
            }
            else if (type == MSG_LOBBY)
            {
                parseLobby(msg);
            }
            else if (type == MSG_QUIT)
            {
                parseQuit(msg);
            }
            else if (type == MSG_RECONNECT)
            {
                parseReconnect(msg);
            }
            else if (type == MSG_GAME)
            {
                parseGame(msg);
            }
            else if (type == MSG_SYNCHRONIZATION)
            {
                parseSynchroinzation(msg);
            }
        }

    }

    #region 同步相关解析

    private void parseSynchroinzation(string msg)
    {
        int type = parseSynType(msg);
        msg = msg.Remove(0, 1);
        if (type == MSG_SYN_DELAY)
        {
            if (!networkSocket.delay.ContainsKey(float.Parse(msg)))
            {
                networkSocket.delay.Add(float.Parse(msg), Time.time);
            }
        }
        else if (type == MSG_SYN_TIME)
        {
            networkSocket.timeDif = float.Parse(msg);
        }
    }

    private int parseSynType(string msg)
    {
        if (msg[0] == 'D')
        {
            return MSG_SYN_DELAY;
        }
        else if (msg[0] == 'T')
        {
            return MSG_SYN_TIME;
        }
        else
        {
            return MSG_UNMEANING;
        }
    }

    #endregion

    #region 游戏相关解析
    private void parseGame(string msg)
    {
        int type = parseGameType(msg);
        msg = msg.Remove(0, 1);
        if (type == MSG_GAME_START)
        {
            parseGameStart(msg);
        }
        else if (type == MSG_GAME_LOAD)
        {
            parseGameLoad(msg);
        }
        else if (type == MSG_GAME_UPDATE)
        {
            parseGameUpdate(msg);
        }
        else if (type == MSG_GAME_ATTACK)
        {
            parseGameAttack(msg);
        }
        else if (type == MSG_GAME_ENEMY)
        {
            parseGameEnemy(msg);
        }
        else if (type == MSG_GAME_PROP)
        {
            parseGameProp(msg);
        }
        else if(type == MSG_GAME_END)
        {
            parseGameEnd(msg);
        }
        else if(type == MSG_GAME_BACK)
        {
            parseGameBack(msg);
        }
    }

    private void parseGameBack(string msgs)
    {
        if(m_gameManager == null)
        {
            return;
        }
        string[] msg = msgs.Split(' ');
        int userID = int.Parse(msg[0]);
        string username = msg[1];
        int HP = int.Parse(msg[2]);
        int ammo = int.Parse(msg[3]);
        int LV = int.Parse(msg[4]);
        int EXP = int.Parse(msg[5]);
        m_gameManager.BackToLobby(userID,username,HP,ammo,LV,EXP);
    }

    private void parseGameEnd(string msg)
    {
        if(m_gameManager == null)
        {
            return;
        }
        string[] msgs = msg.Split(' ');
        int lv = int.Parse(msgs[0]);
        int exp = int.Parse(msgs[1]);
        m_gameManager.GameEnd(lv, exp);
    }

    private void parseGameProp(string msg)
    {
        if (m_gameManager == null)
        {
            return;
        }
        string[] msgs = msg.Split(' ');
        int num = int.Parse(msgs[0]);
        m_gameManager.InitProp();
        for(int i = 1;i < 1 + num * 3; i+=3)
        {
            int id = int.Parse(msgs[i]);
            float x = float.Parse(msgs[i + 1]);
            float z = float.Parse(msgs[i + 2]);
            m_gameManager.SetProp(id,x, z);
        }
        m_gameManager.DestroyProp();
    }


    private void parseGameEnemy(string msg)
    {
        if (m_gameManager == null)
        {
            return;
        }
        string[] msgs = msg.Split(' ');
        float serverTime = float.Parse(msgs[0]);
        for (int i = 1; i < 22; i += 8)
        {
            int enemyID = int.Parse(msgs[i]);
            int enemyHP = int.Parse(msgs[i + 1]);
            Vector2 enemyPos = new Vector2(float.Parse(msgs[i + 2]), float.Parse(msgs[i + 3]));
            Vector2 enemyNextPos = new Vector2(float.Parse(msgs[i + 4]), float.Parse(msgs[i + 5]));
            int enemyState = int.Parse(msgs[i + 6]);
            int attackTarget = int.Parse(msgs[i + 7]);
            //print("p" + enemyPos + "n" + enemyNextPos);
            m_gameManager.SetEnemyState(enemyID, enemyHP, enemyPos, enemyNextPos, enemyState, attackTarget);
        }
        int deathEnemy = int.Parse(msgs[25]);
        int maxEnemy = int.Parse(msgs[26]);
        m_gameManager.SetGameTarget(deathEnemy, maxEnemy);
    }

    private void parseGameAttack(string msg)
    {
        if (m_gameManager == null)
        {
            return;
        }
        string[] msgs = msg.Split(' ');
        int playerID = int.Parse(msgs[0]);
        Vector3 pos = new Vector3(float.Parse(msgs[1]), float.Parse(msgs[2]), float.Parse(msgs[3]));
        int hitTarget = int.Parse(msgs[4]);
        m_gameManager.SetHitEffect(playerID, pos, hitTarget);
    }

    private void parseGameUpdate(string msg)
    {
        if (m_gameManager == null)
        {
            return;
        }
        string[] msgs = msg.Split(' ');
        int playersNum = int.Parse(msgs[0]);
        float serverTime = float.Parse(msgs[1]);
        for (int i = 2; i < 2 + playersNum * 14; i += 14)
        {
            int playerID = int.Parse(msgs[i]);
            string username = msgs[i + 1];
            Vector2 pos = new Vector2(float.Parse(msgs[i + 2]), float.Parse(msgs[i + 3]));
            float rotY = float.Parse(msgs[i + 4]);
            float lookPosY = float.Parse(msgs[i + 5]);
            int motion = int.Parse(msgs[i + 6]);
            int move = int.Parse(msgs[i + 7]);
            int attack = int.Parse(msgs[i + 8]);
            int HP = int.Parse(msgs[i + 9]);
            int curAmmo = int.Parse(msgs[i + 10]);
            int bagAmmo = int.Parse(msgs[i + 11]);
            int LV = int.Parse(msgs[i + 12]);
            int EXP = int.Parse(msgs[i + 13]);
            m_gameManager.SetPlayerState(playerID, serverTime, username, pos, rotY, lookPosY, motion, move, attack, HP, curAmmo, bagAmmo, LV, EXP);
        }
    }

    private void parseGameLoad(string msg)
    {
        if (msg.CompareTo("READY") == 0)
        {
            if (m_loadManager)
            {
                m_loadManager.Ready();
            }
            return;
        }
        string[] msgs = msg.Split(' ');
        int playerNum = int.Parse(msgs[0]);
        //m_loadManager.SetUserNum(playerNum);
        for (int i = 1; i < 1 + playerNum * 3; i += 3)
        {
            int playerID = int.Parse(msgs[i]);
            string username = msgs[i + 1];
            float process = float.Parse(msgs[i + 2]);
            if (m_loadManager)
            {
                m_loadManager.SetProcess(playerID, username, process);
            }
        }
    }

    private void parseGameStart(string receiveMsg)
    {
        string[] msgs = receiveMsg.Split(' ');
        int roomID = int.Parse(msgs[0]);
        int map = int.Parse(msgs[1]);
        int playerNum = int.Parse(msgs[2]);
        List<PlayerMsg> players = new List<PlayerMsg>();
        for (int i = 3; i < 3 + 8 * playerNum; i += 8)
        {
            PlayerMsg player = new PlayerMsg();
            player.roomID = roomID;
            player.playerID = int.Parse(msgs[i]);
            player.username = msgs[i + 1];
            player.initPos = new Vector3(int.Parse(msgs[i + 2]), int.Parse(msgs[i + 3]), int.Parse(msgs[i + 4]));
            player.initRot = new Vector3(int.Parse(msgs[i + 5]), int.Parse(msgs[i + 6]), int.Parse(msgs[i + 7]));
            players.Add(player);
        }
        if (m_lobbyManager)
        {
            m_lobbyManager.Start(true, map, players);
        }
    }

    private int parseGameType(string msg)
    {
        if (msg[0] == 'U')
        {
            return MSG_GAME_UPDATE;
        }
        else if (msg[0] == 'S')
        {
            return MSG_GAME_START;
        }
        else if (msg[0] == 'L')
        {
            return MSG_GAME_LOAD;
        }
        else if (msg[0] == 'A')
        {
            return MSG_GAME_ATTACK;
        }
        else if (msg[0] == 'E')
        {
            return MSG_GAME_ENEMY;
        }
        else if (msg[0] == 'P')
        {
            return MSG_GAME_PROP;
        }
        else if(msg[0] == 'D')
        {
            return MSG_GAME_END;
        }
        else if(msg[0] == 'B')
        {
            return MSG_GAME_BACK;
        }
        else
        {
            return MSG_GAME_UNMEANING;
        }
    }

    #endregion


    #region 网络相关解析

    /// <summary>
    /// 服务端发送信息让用户退出游戏
    /// </summary>
    /// <param name="msg"></param>
    private void parseQuit(string msg)
    {
        GameObject quitImage = Instantiate(m_quitImage, GameObject.Find("Canvas").transform);
        Quit quit = quitImage.GetComponent<Quit>();
        if (msg[0] == 'A')
        {
            // 由于账号在其他客户端登录使用户退出游戏
            quit.SetText("账号在其他客户端登录");
        }

    }

    /// <summary>
    /// 服务器反馈重登信息
    /// </summary>
    /// <param name="msg"></param>
    private void parseReconnect(string msgs)
    {
        if (msgs[0] == 'R')
        {
            // 用户短线前未在游戏中，直接返回大厅界面
            msgs = msgs.Remove(0, 1);
            string[] msg = msgs.Split(' ');
            UserMessage.userID = int.Parse(msg[0]);
            UserMessage.username = msg[1];
            UserMessage.HP = int.Parse(msg[2]);
            UserMessage.ammo = int.Parse(msg[3]);
            UserMessage.LV = int.Parse(msg[4]);
            UserMessage.EXP = int.Parse(msg[5]);
            SceneManager.LoadScene("Lobby");
        }
        else if (msgs[0] == 'G')
        {
            // 用户断线前在游戏中，加载游戏界面
        }
    }

    #endregion

    #region 大厅界面相关解析
    private void parseLobby(string msg)
    {
        int type = parseLobbyMsgType(msg);
        msg = msg.Remove(0, 1);
        if (type == MSG_LOBBY_UNMEANING)
        {
            return;
        }
        else if (type == MSG_LOBBY_ROOM)
        {
            // 房间命令，返回的是某房间的全部信息
            parseLobbyMsg_Room(msg);
        }
        else if (type == MSG_LOBBY_JOIN)
        {
            // 加入房间命令
            parseLobbyMsg_Join(msg);
        }
        else if (type == MSG_LOBBY_QUIT)
        {
            // 退出房间命令
            parseLobbyMsg_Quit(msg);
        }
        else if (type == MSG_LOBBY_GETLOBBYROOM)
        {
            // 获得大厅内的所有房间信息
            parseLobbyMsg_GetLoobyRoomMsg(msg);
        }
        else if (type == MSG_LOBBY_START)
        {
            // 开始游戏信息
            parseLobbyMsg_StartGame(msg);
        }
    }

    private void parseLobbyMsg_StartGame(string msg)
    {
        if (msg == "FAIL")
        {
            m_lobbyManager.Start(false, 0, null);
        }
    }

    private void parseLobbyMsg_Room(string msg)
    {
        string[] msgs = msg.Split(' ');
        int roomID = int.Parse(msgs[0]);
        int map = int.Parse(msgs[1]);
        int nowPlayerNum = int.Parse(msgs[2]);
        int maxPlayerNum = int.Parse(msgs[3]);
        List<Player_Room> list = new List<Player_Room>();
        for (int i = 4; i < msgs.Length - 1; i += 3)
        {
            int userID = int.Parse(msgs[i]);
            string username = msgs[i + 1];
            bool ready = false;
            if (msgs[i + 2].CompareTo("True") == 0)
            {
                ready = true;
            }
            Player_Room player = new Player_Room(userID, username, ready);
            list.Add(player);
        }
        m_lobbyManager.RoomUpdate(roomID, map, nowPlayerNum, maxPlayerNum, list);
    }

    private void parseLobbyMsg_GetLoobyRoomMsg(string msg)
    {
        string[] rooms = msg.Split(' ');
        List<Room_Lobby> list = new List<Room_Lobby>();
        if (rooms.Length < 4)
        {
            m_lobbyManager.LobbyRoomUpdate(list);
            return;
        }
        for (int i = 0; i < rooms.Length - 1; i += 4)
        {
            int roomID = int.Parse(rooms[i]);
            int map = int.Parse(rooms[i + 1]);
            int nowUserNum = int.Parse(rooms[i + 2]);
            int maxUserNum = int.Parse(rooms[i + 3]);
            Room_Lobby room = new Room_Lobby(roomID, map, nowUserNum, maxUserNum);
            list.Add(room);
        }
        m_lobbyManager.LobbyRoomUpdate(list);
    }

    private void parseLobbyMsg_Quit(string msg)
    {
        if (msg.CompareTo("OK") == 0)
        {
            m_lobbyManager.Back();
        }
    }

    private void parseLobbyMsg_Join(string msg)
    {
        if (msg == "FAIL")
        {
            //加入房间失败，再次尝试拉取房间信息
            m_lobbyManager.m_hintText.SetText("加入房间失败");
            m_lobbyManager.JoinRoom();
        }
    }

    private int parseLobbyMsgType(string msg)
    {
        if (msg[0] == 'R')
        {
            return MSG_LOBBY_ROOM;
        }
        else if (msg[0] == 'J')
        {
            return MSG_LOBBY_JOIN;
        }
        else if (msg[0] == 'Q')
        {
            return MSG_LOBBY_QUIT;
        }
        else if (msg[0] == 'G')
        {
            return MSG_LOBBY_GETLOBBYROOM;
        }
        else if (msg[0] == 'S')
        {
            return MSG_LOBBY_START;
        }
        else
        {
            return MSG_LOBBY_UNMEANING;
        }
    }

    #endregion

    #region 登录界面相关解析

    private void parseLogin(string msg)
    {
        if (msg.Substring(0, 2) == "OK")
        {
            m_loginManager.showLoginRes(msg.Substring(3), true);
        }
        else if (msg == "FAIL")
        {
            m_loginManager.showLoginRes(msg, false);
        }
    }

    private void parseRegister(string msg)
    {
        if (msg == "OK")
        {
            m_loginManager.showRegisterRes(true);
        }
        else if (msg == "FAIL")
        {
            m_loginManager.showRegisterRes(false);
        }
    }

    #endregion

    private int parseType(string msg)
    {
        if (msg[0] == 'R')
        {
            return MSG_REGISTER;
        }
        else if (msg[0] == 'L')
        {
            return MSG_LOGIN;
        }
        else if (msg[0] == 'B')
        {
            return MSG_LOBBY;
        }
        else if (msg[0] == 'Q')
        {
            return MSG_QUIT;
        }
        else if (msg[0] == 'E')
        {
            return MSG_RECONNECT;
        }
        else if (msg[0] == 'G')
        {
            return MSG_GAME;
        }
        else if (msg[0] == 'S')
        {
            return MSG_SYNCHRONIZATION;
        }
        else
        {
            return MSG_UNMEANING;
        }
    }

    private string[] simple(string msg)
    {
        if (msg == " ")
        {
            return new string[] { "U" };
        }
        // 可能因为延迟收到多条信息 去除前后缀并将所有信息放入数组中
        string[] temp = msg.Trim().Split('@');
        List<string> res = new List<string>();
        foreach (var item in temp)
        {
            if (item.Length > 1)
            {
                res.Add(item.Remove(item.Length - 1));
            }
        }
        // 去除前缀和后缀
        return res.ToArray();
    }
}

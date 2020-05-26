using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbySceneManager : MonoBehaviour
{
    public TMP_Text m_usernameText;
    public TMP_Text m_HPText;
    public TMP_Text m_ammoText;
    public TMP_Text m_LVText;
    public TMP_Text m_EXPText;

    public HintText m_hintText;

    public TMP_Text m_room_RoomIDText;
    public TMP_Text m_room_MapText;
    public TMP_Text m_room_UserNumText;
    public TMP_Text m_room_readyText;
    public Transform m_room_Content;
    private GameObject m_room_UserMsgPerfab;

    public Transform m_join_Content;
    private GameObject m_join_RoomMsgPrefab;

    public Transform m_lobbyObj;
    public Transform m_roomObj;
    public Transform m_createRoomObj;
    public Transform m_joinRoomObj;

    public Transform m_lobbyStopPos;
    public Transform m_midPos;

    public float m_uiMovingSpeed = 500f;

    private bool m_removeCreateRoomUI;
    private bool m_showCreateRoomUI;

    private bool m_removeJoinRoomUI;
    private bool m_showJoinRoomUI;

    private bool m_removeRoomUI;
    private bool m_showRoomUI;

    private bool m_removeLobbyUI;
    private bool m_showLoobyUI;

    private int m_roomID;

    private Vector3 m_oriLobbyPos;
    private Vector3 m_oriCreteRoomPos;
    private Vector3 m_oriJoinRoomPos;
    private Vector3 m_oriRoomPos;

    public networkSocket m_socket;

    public bool m_ani = false;

    public bool m_owner = false;


    // Start is called before the first frame update
    void Start()
    {
        m_socket = GameObject.Find("Network").GetComponent<networkSocket>();
        m_usernameText.text = UserMessage.username;
        m_HPText.text = "生命值:" + UserMessage.HP.ToString();
        m_ammoText.text = "子弹数:" + UserMessage.ammo.ToString();
        m_LVText.text = "等级:" + UserMessage.LV.ToString();
        m_EXPText.text = "经验:" + UserMessage.EXP.ToString();

        m_oriLobbyPos = m_lobbyObj.position;
        m_oriJoinRoomPos = m_joinRoomObj.position;
        m_oriCreteRoomPos = m_createRoomObj.position;
        m_oriRoomPos = m_roomObj.position;

        m_room_UserMsgPerfab = Resources.Load("Prefabs\\UserMsg") as GameObject;
        m_join_RoomMsgPrefab = Resources.Load("Prefabs\\RoomMsg") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_removeLobbyUI)
        {
            // 移去大厅相关操作动画
            m_ani = true;
            m_lobbyObj.position = Vector2.MoveTowards(m_lobbyObj.position, m_lobbyStopPos.position, m_uiMovingSpeed * Time.deltaTime);
            if (Vector2.Distance(m_lobbyObj.position, m_lobbyStopPos.position) < 0.1f)
            {
                m_removeLobbyUI = false;
            }
        }
        else if (m_removeCreateRoomUI)
        {
            // 移去创建房间UI动画
            m_ani = true;
            m_createRoomObj.position = Vector2.MoveTowards(m_createRoomObj.position, m_oriCreteRoomPos, m_uiMovingSpeed * Time.deltaTime);
            if (Vector2.Distance(m_createRoomObj.position, m_oriCreteRoomPos) < 0.1f)
            {
                m_removeCreateRoomUI = false;
            }
        }
        else if (m_removeJoinRoomUI)
        {
            // 移去加入房间UI动画
            m_ani = true;
            m_joinRoomObj.position = Vector2.MoveTowards(m_joinRoomObj.position, m_oriJoinRoomPos, m_uiMovingSpeed * Time.deltaTime);
            if (Vector2.Distance(m_joinRoomObj.position, m_oriJoinRoomPos) < 0.1f)
            {
                m_removeJoinRoomUI = false;
            }
        }
        else if (m_removeRoomUI)
        {
            // 移去房间UI动画
            m_ani = true;
            m_roomObj.position = Vector2.MoveTowards(m_roomObj.position, m_oriRoomPos, m_uiMovingSpeed * Time.deltaTime);
            if (Vector2.Distance(m_roomObj.position, m_oriRoomPos) < 0.1f)
            {
                m_removeRoomUI = false;
            }
        }
        else if (m_showLoobyUI)
        {
            // 恢复大厅相关操作
            m_lobbyObj.position = Vector2.MoveTowards(m_lobbyObj.position, m_oriLobbyPos, m_uiMovingSpeed * Time.deltaTime);
            if (Vector2.Distance(m_lobbyObj.position, m_oriLobbyPos) < 0.1f)
            {
                m_showLoobyUI = false;
                m_ani = false;
            }
        }
        else if (m_showJoinRoomUI)
        {
            // 显示加入房间UI
            m_joinRoomObj.position = Vector2.MoveTowards(m_joinRoomObj.position, m_midPos.position, m_uiMovingSpeed * Time.deltaTime);
            if (Vector2.Distance(m_joinRoomObj.position, m_midPos.position) < 0.1f)
            {
                m_showJoinRoomUI = false;
                m_ani = false;
            }
        }
        else if (m_showCreateRoomUI)
        {
            // 显示加入房间UI
            m_createRoomObj.position = Vector2.MoveTowards(m_createRoomObj.position, m_midPos.position, m_uiMovingSpeed * Time.deltaTime);
            if (Vector2.Distance(m_createRoomObj.position, m_midPos.position) < 0.1f)
            {
                m_showCreateRoomUI = false;
                m_ani = false;
            }
        }
        else if (m_showRoomUI)
        {
            // 显示房间UI
            m_roomObj.position = Vector2.MoveTowards(m_roomObj.position, m_midPos.position, m_uiMovingSpeed * Time.deltaTime);
            if (Vector2.Distance(m_roomObj.position, m_midPos.position) < 0.1f)
            {
                m_showRoomUI = false;
                m_ani = false;
            }
        }

    }

    #region 加入房间相关操作

    /// <summary>
    /// 大厅的房间信息更新
    /// </summary>
    /// <param name="list"></param>
    public void LobbyRoomUpdate(List<Room_Lobby> list)
    {
        ClearRoomMsg();
        foreach (Room_Lobby room in list)
        {
            AddRoomMsg(room);
        }
    }

    /// <summary>
    /// 清除大厅中所有房间信息
    /// </summary>
    private void ClearRoomMsg()
    {
        List<Transform> list = new List<Transform>();
        foreach (Transform child in m_join_Content)
        {
            list.Add(child);
        }
        for (int i = 0; i < list.Count; i++)
        {
            Destroy(list[i].gameObject);
        }
    }

    /// <summary>
    /// 将一个房间信息添加到大厅中
    /// </summary>
    /// <param name="room">房间</param>
    private void AddRoomMsg(Room_Lobby room)
    {
        Transform roomMsg = Instantiate(m_join_RoomMsgPrefab, m_join_Content).transform;
        TMP_Text IDText = roomMsg.Find("IDText (TMP)").GetComponent<TMP_Text>();
        TMP_Text map = roomMsg.Find("MapText (TMP)").GetComponent<TMP_Text>();
        TMP_Text userNum = roomMsg.Find("UserNumText (TMP)").GetComponent<TMP_Text>();
        Button joinButton = roomMsg.Find("JoinButton").GetComponent<Button>();
        IDText.text = room.ID.ToString();
        map.text = room.map.ToString();
        userNum.text = room.nowPlayerNum + "/" + room.maxPlayerNum;
        joinButton.onClick.AddListener(delegate ()
        {
            if (Reconnect.m_reconnect || Quit.m_quit || m_ani)
            {
                return;
            }
            m_socket.writeSocket("@BJ" + room.ID + "#");
        });

    }



    #endregion

    #region 房间相关操作
    /// <summary>
    /// 更新房间信息，如果房间界面未显示，则显示房间界面
    /// </summary>
    /// <param name="roomID">房间ID</param>
    /// <param name="map">房间的地图</param>
    /// <param name="userNum">当前用户数</param>
    /// <param name="maxUsernum">房间最大用户数</param>
    /// <param name="players">玩家的信息</param>
    public void RoomUpdate(int roomID, int map, int userNum, int maxUsernum, List<Player_Room> players)
    {
        // 显示房间界面
        m_removeLobbyUI = true;
        m_removeJoinRoomUI = true;
        m_removeCreateRoomUI = true;
        m_showRoomUI = true;

        String owner = "";

        m_roomID = roomID;
        m_room_RoomIDText.text = "房间号:" + roomID;
        m_room_MapText.text = "地图:" + map;
        m_room_UserNumText.text = "人数:" + userNum + "/" + maxUsernum;
        ClearUserMsg();
        foreach (Player_Room player in players)
        {
            AddUserInRoom(player);
            if(player.ID == 1)
            {
                owner = player.Username;
            }
        }
        if(UserMessage.username.CompareTo(owner) == 0)
        {
            m_room_readyText.SetText("开始");
            m_owner = true;
        }
        else
        {
            m_room_readyText.SetText("准备");
            m_owner = false;
        }
        
    }

    private void AddUserInRoom(Player_Room player)
    {
        Transform userMsg = Instantiate(m_room_UserMsgPerfab, m_room_Content).transform;
        TMP_Text IDText = userMsg.Find("IDText (TMP)").GetComponent<TMP_Text>();
        TMP_Text usernameText = userMsg.Find("UsernameText (TMP)").GetComponent<TMP_Text>();
        TMP_Text readyText = userMsg.Find("ReadyText (TMP)").GetComponent<TMP_Text>();

        IDText.text = player.ID.ToString();
        usernameText.text = player.Username.ToString();
        readyText.text = player.ready ? "是" : "否";
    }


    /// <summary>
    /// 清除房间的所有用户信息
    /// </summary>
    private void ClearUserMsg()
    {
        List<Transform> list = new List<Transform>();
        foreach (Transform child in m_room_Content)
        {
            list.Add(child);
        }
        for (int i = 0; i < list.Count; i++)
        {
            Destroy(list[i].gameObject);
        }
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    /// <param name="start">是否可以开始游戏</param>
    public void Start(bool start,int map,List<PlayerMsg> players)
    {
        if (start)
        {
            LoadSceneManager.m_map = map;
            LoadSceneManager.m_userNum = players.Count;
            PlayerMsg.players.Clear();
            foreach (var player in players)
            {
                PlayerMsg.players.Add(player);
                if(player.username == UserMessage.username)
                {
                    UserMessage.roomID = player.roomID;
                    UserMessage.playerID = player.playerID;
                }
            }
            SceneManager.LoadScene("Load");
        }
        else
        {
            // 有人未准备，无法开始游戏
            m_hintText.SetText("所有人准备完毕方可开始游戏");
        }
    }

    /// <summary>
    /// 按下准备按钮
    /// </summary>
    public void Ready()
    {
        if (Reconnect.m_reconnect || Quit.m_quit || m_ani)
        {
            return;
        }
        if(!m_owner)
        {
            m_socket.writeSocket("@BR" + m_roomID + "#");
        }
        else if(m_owner)
        {
            m_socket.writeSocket("@BS" + m_roomID + "#");
        }
    }

    /// <summary>
    /// 退出房间
    /// </summary>
    public void QuitRoom()
    {
        if (Reconnect.m_reconnect || Quit.m_quit || m_ani)
        {
            return;
        }
        m_socket.writeSocket("@BQ" + m_roomID + "#");
    }
    #endregion

    #region 大厅相关操作
    /// <summary>
    /// 创建房间
    /// </summary>
    public void CreateRoom()
    {
        if (Reconnect.m_reconnect || Quit.m_quit || m_ani)
        {
            return;
        }
        m_removeLobbyUI = true;
        m_showCreateRoomUI = true;
    }

    /// <summary>
    /// 加入房间
    /// </summary>
    public void JoinRoom()
    {
        if (Reconnect.m_reconnect || Quit.m_quit || m_ani)
        {
            return;
        }
        m_removeLobbyUI = true;
        m_showJoinRoomUI = true;
        m_socket.writeSocket("@BG#");
    }
    #endregion

    /// <summary>
    /// 返回按钮
    /// </summary>
    public void Back()
    {
        if (Reconnect.m_reconnect || Quit.m_quit || m_ani)
        {
            return;
        }
        m_removeCreateRoomUI = true;
        m_removeJoinRoomUI = true;
        m_removeRoomUI = true;
        m_showLoobyUI = true;
    }


    public void SendMsgToServer(string msg)
    {
        m_socket.writeSocket(msg);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public networkSocket m_socket;

    public Map m_map;

    public Player m_player1;
    public Player m_player2;
    public Player m_player3;

    public Shadow m_shadow1;
    public Shadow m_shadow2;
    public Shadow m_shadow3;

    public Enemy m_enemy1;
    public Enemy m_enemy2;
    public Enemy m_enemy3;

    public GameObject m_hitEnemyEffect;
    public GameObject m_hitOtherEffect;
    public GameObject m_grenade;
    public GameObject m_grenadeEffect;
    public GameObject m_prop;

    public Transform m_gameEndImage;
    public GameObject m_gamePauseImage;
    public GameObject m_quitConfirmImage;

    public TMP_Text m_attackStateText;
    public TMP_Text m_ammoText;
    public TMP_Text m_gameTargetText;

    public Slider m_volumeSlider;
    public Slider m_backgroundvolumeSlider;

    public HP m_ownerHPImage;
    public ResurgenceImage m_resurgenceImage;

    public BufferPool m_hitEnemyPool;
    public BufferPool m_hitOtherPool;
    public BufferPool m_grenadeEffectPool;
    public PropBufferPool m_propPool;

    public List<GameObject> m_grenades;
    private List<int> m_boomIndex;

    public List<GameObject> m_props;
    private List<int> m_tempProps;

    public static bool m_gaming = true;
    public static bool m_pause = false;
    // Start is called before the first frame update
    void Awake()
    {
        m_socket = GameObject.Find("Network").GetComponent<networkSocket>();


        m_grenades = new List<GameObject>();
        m_props = new List<GameObject>();
        m_boomIndex = new List<int>();
        initPlayers();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        m_hitEnemyPool.SetBufferPool(m_hitEnemyEffect, 10);
        m_hitOtherPool.SetBufferPool(m_hitOtherEffect, 10);
        m_propPool.SetBufferPool(m_prop, 6);
        m_grenadeEffectPool.SetBufferPool(m_grenadeEffect, 3);

        if (PlayerPrefs.HasKey("Volume"))
        {
            m_volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        }
        if (PlayerPrefs.HasKey("BackgroundVolume"))
        {
            m_backgroundvolumeSlider.value = PlayerPrefs.GetFloat("BackgroundVolume");
        }
    }

    /// <summary>
    /// 计算网络Ping值（已废弃）
    /// </summary>
    public void ping()
    {
        if (m_socket)
        {
            m_socket.SendSynMsg();
        }
    }

    /// <summary>
    /// 声音滑杆值变化事件调用函数
    /// </summary>
    /// <param name="EventSender">发生变化的滑杆</param>
    public void OnVolumeValueChange(Slider EventSender)
    {
        float value = EventSender.value;
        switch (EventSender.name)
        {
            // 全局声音发生变化，更改AudioListener的大小
            case "VolumeSlider":
                AudioListener.volume = value;
                PlayerPrefs.SetFloat("Volume", value);
                break;
            // 背景音乐发生变化，仅更改背景音乐的大小
            case "BackgroundVolumeSlider":
                m_socket.GetComponent<AudioSource>().volume = value;
                PlayerPrefs.SetFloat("BackgroundVolume", value);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 返回大厅页面
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="username"></param>
    /// <param name="HP"></param>
    /// <param name="ammo"></param>
    /// <param name="LV"></param>
    /// <param name="EXP"></param>
    public void BackToLobby(int userID, string username, int HP, int ammo, int LV, int EXP)
    {
        if(userID != 0)
        {
            UserMessage.userID = userID;
            UserMessage.username = username;
            UserMessage.HP = HP;
            UserMessage.ammo = ammo;
            UserMessage.LV = LV;
            UserMessage.EXP = EXP;
        }
        SceneManager.LoadScene("Lobby");
    }

    /// <summary>
    /// 发送返回大厅信息给服务端
    /// </summary>
    public void SengBackMsg()
    {
        SendMessageToServer("UB" + UserMessage.username);
    }

    /// <summary>
    /// 游戏结束，记录新的等级和经验
    /// </summary>
    /// <param name="lv"></param>
    /// <param name="exp"></param>
    public void GameEnd(int lv, int exp)
    {
        if (m_gaming)
        {
            m_gaming = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            StartCoroutine(ShowGameEndDialog(lv, exp));
        }
    }

    /// <summary>
    /// 显示游戏结束窗口
    /// </summary>
    /// <param name="lv"></param>
    /// <param name="exp"></param>
    /// <returns></returns>
    IEnumerator ShowGameEndDialog(int lv, int exp)
    {
        m_gameEndImage.gameObject.SetActive(true);
        TMP_Text LVText = m_gameEndImage.Find("LVText").GetComponent<TMP_Text>();
        TMP_Text EXPText = m_gameEndImage.Find("EXPText").GetComponent<TMP_Text>();
        Slider EXPSlider = m_gameEndImage.Find("EXPSlider").GetComponent<Slider>();
        LevelUp flash = LVText.transform.Find("Flash").GetComponent<LevelUp>();
        int showLV = UserMessage.LV;
        int showEXP = UserMessage.EXP;
        LVText.text = "LV:" + UserMessage.LV;
        EXPText.text = "EXP:" + UserMessage.EXP;
        // 经验值增加动画，显示等级小于实际等级经验值不断上升
        while (showLV < lv)
        {
            showEXP += 10;
            // 等级+1
            if (showEXP >= 1000)
            {
                showEXP = showEXP - 1000;
                showLV += 1;
                LVText.text = "LV:" + showLV;
                flash.Flash();
            }
            EXPText.text = "EXP:" + showEXP;
            EXPSlider.value = showEXP / 1000f;
            yield return new WaitForEndOfFrame();
        }
        // 显示等级与实际等级相等，增加经验直到达到实际经验
        while (showEXP < exp)
        {
            EXPText.text = "EXP:" + Mathf.Min(showEXP += 10, exp);
            EXPSlider.value = Mathf.Min(showEXP, exp) / 1000f;
            yield return new WaitForEndOfFrame();
        }
        yield return 0;
    }

    /// <summary>
    /// 初始化角色
    /// </summary>
    private void initPlayers()
    {
        m_ownerHPImage.SetMaxHP(UserMessage.HP);
        List<PlayerMsg> players = PlayerMsg.players;
        if (players.Count == 1)
        {
            Destroy(m_player2.gameObject);
            Destroy(m_shadow2.gameObject);
            Destroy(m_player3.gameObject);
            Destroy(m_shadow3.gameObject);
        }
        else if (players.Count == 2)
        {
            Destroy(m_player3.gameObject);
            Destroy(m_shadow3.gameObject);
        }
        foreach (var p in players)
        {
            Player player = GetPlayer(p.playerID);
            Shadow shadow = GetShadow(p.playerID);
            player.transform.position = p.initPos;
            shadow.transform.position = new Vector3(p.initPos.x, shadow.transform.position.y, p.initPos.z);
            player.transform.eulerAngles = p.initRot;
            shadow.transform.eulerAngles = p.initRot;
            player.username = p.username;
            if (p.username == UserMessage.username)
            {
                Transform lookTarget = player.transform.Find("LookTarget");
                Camera.main.gameObject.GetComponent<TPSCamera>().target = lookTarget;
                player.gameObject.AddComponent<Owner>();
            }
        }
        for (int i = 0; i < players.Count; i++)
        {
            GameObject grenda = GameObject.Instantiate(m_grenade);
            grenda.GetComponent<Grenade>().m_from = i + 1;
            grenda.SetActive(false);
            m_grenades.Add(grenda);
        }
    }

    /// <summary>
    /// 设置游戏目标UI
    /// </summary>
    /// <param name="deathEnemy"></param>
    /// <param name="maxEnemy"></param>
    public void SetGameTarget(int deathEnemy, int maxEnemy)
    {
        m_gameTargetText.SetText("游戏进度:" + deathEnemy + "/" + maxEnemy);
    }

    /// <summary>
    /// 设置攻击模式UI
    /// </summary>
    /// <param name="text"></param>
    public void SetAttackText(string text)
    {
        m_attackStateText.text = "当前攻击模式:" + text;
    }

    /// <summary>
    /// 设置子弹数UI
    /// </summary>
    /// <param name="nowAmmo"></param>
    /// <param name="bagAmmo"></param>
    public void SetAmmoText(int nowAmmo, int bagAmmo)
    {
        m_ammoText.text = "子弹数:" + nowAmmo + "/" + bagAmmo;
    }

    /// <summary>
    /// 生成击中特效
    /// </summary>
    /// <param name="playerID">发起攻击的角色ID</param>
    /// <param name="pos">特效位置</param>
    /// <param name="hitTarget">击中目标属性</param>
    public void SetHitEffect(int playerID, Vector3 pos, int hitTarget)
    {
        Player player = GetPlayer(playerID);
        if (player.username == UserMessage.username)
        {
            //return;
        }
        if (hitTarget == 0)
        {
            // 击中其他目标
            //Instantiate(m_hitOtherEffect, pos, Quaternion.identity);
            m_hitOtherPool.CreateInstance(pos, Quaternion.identity, 1);
        }
        else
        {
            // 击中敌人
            //Instantiate(m_hitEnemyEffect, pos, Quaternion.identity);
            m_hitEnemyPool.CreateInstance(pos, Quaternion.identity, 1);
        }
    }

    /// <summary>
    /// 设置角色状态
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="serverTime"></param>
    /// <param name="username"></param>
    /// <param name="pos"></param>
    /// <param name="rotY"></param>
    /// <param name="lookPosY"></param>
    /// <param name="motion"></param>
    /// <param name="move"></param>
    /// <param name="attack"></param>
    /// <param name="HP"></param>
    /// <param name="curAmmo"></param>
    /// <param name="bagAmmo"></param>
    /// <param name="LV"></param>
    /// <param name="EXP"></param>
    public void SetPlayerState(int playerID, float serverTime, string username, Vector2 pos, float rotY, float lookPosY, int motion, int move, int attack,
                                int HP, int curAmmo, int bagAmmo, int LV, int EXP)
    {
        Player player = GetPlayer(playerID);
        Shadow shadow = GetShadow(playerID);
        // 死亡特殊处理
        if(motion == Motion.DEATH)
        {
            SetPlayerDeath(playerID, username, pos, motion,move, HP, curAmmo, bagAmmo, LV, EXP);

        }
        else if(player.GetMotion() == Motion.DEATH)
        {
            // 复活处理
            player.SetMotion(motion);
            player.SetMoveState(move);
            player.SetAttackState(attack);
            if(username == UserMessage.username)
            {
                m_resurgenceImage.Resurgence();
                Camera.main.GetComponent<DeathShader>().grayScaleAmount = 0.0f;
            }
        }
        //player.m_state = new PlayerState(playerID, username, pos, rot, lookTarget, motion, move, attack, HP, curAmmo, bagAmmo, LV, EXP);
        //if(player.username != UserMessage.username || 
        //    Vector2.Distance(new Vector2(pos.x,pos.z),new Vector2(player.transform.position.x,player.transform.position.z)) > 0.1f)
        {
            Vector2 look = VecRotate(Vector2.up, rotY);
            //pos = RepairPos(pos, look, motion, move, serverTime);
            shadow.SetState(pos, rotY, motion, move);
        }
        //player.transform.position = pos;
        if (player.username != UserMessage.username)
        {
            player.m_nextRotY = rotY;
            player.m_nextLookPosY = lookPosY;
            player.SetMotion(motion);
            player.SetMoveState(move);
            player.SetAttackState(attack);
        }
        else
        {
            SetAmmoText(curAmmo, bagAmmo);
            if (HP < m_ownerHPImage.m_greenHP.value)
            {
                m_ownerHPImage.ReduceHP((int)(m_ownerHPImage.m_greenHP.value) - HP);
            }
            else if (HP > m_ownerHPImage.m_greenHP.value)
            {
                m_ownerHPImage.AddHP(HP - (int)(m_ownerHPImage.m_greenHP.value));
            }
        }
        //player.transform.eulerAngles = rot;
        //player.m_looktarget.localPosition = lookTarget;
        //player.SetMotion(motion);
        //player.SetMoveState(move);
        //player.SetAttackState(attack);
        player.SetHP(HP);
        player.SetAmmo(curAmmo, bagAmmo);
        player.SetLV(LV);
        player.SetExp(EXP);
        // 技能处理
        if (attack == AttackState.THROW && !m_grenades[playerID - 1].activeSelf)
        {
            Vector3 grenadePos = player.transform.position + player.transform.right * -0.5f + player.transform.up * 2.1f;
            m_grenades[playerID - 1].transform.position = grenadePos;
            m_grenades[playerID - 1].transform.rotation = player.transform.rotation;
            m_grenades[playerID - 1].SetActive(true);
            m_boomIndex.Add(playerID - 1);
            Invoke("Boom", 1.6f);
        }
    }

    /// <summary>
    /// 设置玩家死亡
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="username"></param>
    /// <param name="pos"></param>
    /// <param name="motion"></param>
    /// <param name="time"></param>
    /// <param name="HP"></param>
    /// <param name="curAmmo"></param>
    /// <param name="bagAmmo"></param>
    /// <param name="lV"></param>
    /// <param name="eXP"></param>
    private void SetPlayerDeath(int playerID, string username, Vector2 pos, int motion, int time, int HP, int curAmmo, int bagAmmo, int lV, int eXP)
    {
        Player player = GetPlayer(playerID);
        Shadow shadow = GetShadow(playerID);
        player.SetMotion(motion);
        player.SetHP(0);
        // 当前操控角色死亡，播放死亡计时，画面变黑白
        if(username == UserMessage.username)
        {
            m_resurgenceImage.Death(time);
            Camera.main.GetComponent<DeathShader>().grayScaleAmount = 1.0f;
            
        }
    }

    /// <summary>
    /// 技能爆炸处理
    /// </summary>
    public void Boom()
    {
        int index = m_boomIndex[0];
        m_boomIndex.RemoveAt(0);
        m_grenadeEffectPool.CreateInstance(m_grenades[index].transform.position, m_grenades[index].transform.rotation, 1);
        if (m_grenades[index].GetComponent<Grenade>().m_from == UserMessage.playerID)
        {
            for (int i = 1; i <= 3; i++)
            {
                Enemy enemy = GetEnemy(i);
                if (enemy.m_state != EnemyState.ENEMY_STATE_DEATH && Vector3.Distance(enemy.transform.position, m_grenades[index].transform.position) < 5)
                {
                    SendMessageToServer("UD" + enemy.m_id + " " + 20);
                }
            }
        }
    }

    /// <summary>
    /// 道具初始化
    /// </summary>
    public void InitProp()
    {
        m_tempProps = new List<int>();
    }

    /// <summary>
    /// 放置道具
    /// </summary>
    /// <param name="id">道具ID</param>
    /// <param name="x">X坐标</param>
    /// <param name="z">Z坐标</param>
    public void SetProp(int id, float x, float z)
    {
        m_tempProps.Add(id);
        foreach (var item in m_props)
        {
            if (item.GetComponent<Prop>().ID == id)
            {
                return;
            }
        }
        GameObject prop = m_propPool.CreatePropInstance(new Vector3(x, 0, z), Quaternion.identity, id);
        m_props.Add(prop);
    }

    /// <summary>
    /// 销毁道具
    /// </summary>
    public void DestroyProp()
    {
        for (int i = 0; i < m_props.Count; i++)
        {
            if (!m_tempProps.Contains(m_props[i].GetComponent<Prop>().ID))
            {
                m_propPool.DestoryBuffer(m_props[i]);
                m_props.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>
    /// 2维向量转向
    /// </summary>
    /// <param name="from"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Vector2 VecRotate(Vector2 from, float angle)
    {
        float radian = angle / 180 * Mathf.PI;
        float x = from.x * Mathf.Cos(radian) + from.y * Mathf.Sin(radian);
        float y = -from.x * Mathf.Sin(radian) + from.y * Mathf.Cos(radian);
        return new Vector2(x, y);
    }

    /// <summary>
    /// 修补位置（已废弃）
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="look"></param>
    /// <param name="motion"></param>
    /// <param name="move"></param>
    /// <param name="serverTime"></param>
    /// <returns></returns>
    private Vector2 RepairPos(Vector2 pos, Vector3 look, int motion, int move, float serverTime)
    {
        float speed = 0;
        switch (motion)
        {
            case Motion.CROUCH:
                speed = Player.m_crouchSpeed;
                break;
            case Motion.RUN:
                speed = Player.m_runSpeed;
                break;
            case Motion.WALK:
                speed = Player.m_walkSpeed;
                break;
            default:
                break;
        }
        float delay = Time.time - (serverTime - networkSocket.timeDif);
        int angle = -1;
        if (move == MoveState.IDLE)
        {
            return pos;
        }
        if (move == MoveState.RUN)
        {
            // 前进
            angle = 0;
        }
        else if (move == MoveState.BACK)
        {
            // 后退
            angle = 180;
        }
        else if (move == MoveState.LEFT)
        {
            // 左移
            angle = 270;
        }
        else if (move == MoveState.RIGHT)
        {
            // 右移
            angle = 90;
        }
        else if (move == MoveState.RUN_LEFT)
        {
            // 左前
            angle = 315;
        }
        else if (move == MoveState.RUN_RIGHT)
        {
            //右前
            angle = 45;
        }
        else if (move == MoveState.BACK_LEFT)
        {
            // 左后
            angle = 225;
        }
        else if (move == MoveState.BACK_RIGHT)
        {
            // 右后
            angle = 135;
        }
        Vector2 moveDirect = VecRotate(look, angle);
        return pos + moveDirect * delay * speed;

    }

    /// <summary>
    /// 设置敌人状态
    /// </summary>
    /// <param name="enemyID"></param>
    /// <param name="enemyHP"></param>
    /// <param name="enemyPos"></param>
    /// <param name="enemyNextPos"></param>
    /// <param name="enemyState"></param>
    /// <param name="attackTarget"></param>
    public void SetEnemyState(int enemyID, int enemyHP, Vector2 enemyPos, Vector2 enemyNextPos, int enemyState, int attackTarget)
    {
        Enemy enemy = GetEnemy(enemyID);
        EnemyShadow shadow = GetEnemyShadow(enemyID);
        Transform target = transform;
        if (enemyState == EnemyState.ENEMY_STATE_ATTACK)
        {
            target = GetPlayer(attackTarget).transform;
        }
        enemy.SetState(enemyHP, enemyState, target);
        shadow.SetState(enemyPos, enemyNextPos, enemyState);
    }

    /// <summary>
    /// 获得敌人
    /// </summary>
    /// <param name="enemyID"></param>
    /// <returns></returns>
    private Enemy GetEnemy(int enemyID)
    {
        switch (enemyID)
        {
            case 1:
                return m_enemy1;
            case 2:
                return m_enemy2;
            case 3:
                return m_enemy3;
            default:
                return null;
        }
    }

    /// <summary>
    /// 获得敌人影子
    /// </summary>
    /// <param name="enemyID"></param>
    /// <returns></returns>
    private EnemyShadow GetEnemyShadow(int enemyID)
    {
        Enemy enemy = GetEnemy(enemyID);
        return enemy.m_shadow;
    }

    /// <summary>
    /// 获得玩家
    /// </summary>
    /// <param name="playerID"></param>
    /// <returns></returns>
    private Player GetPlayer(int playerID)
    {
        switch (playerID)
        {
            case 1:
                return m_player1;
            case 2:
                return m_player2;
            case 3:
                return m_player3;
            default:
                return null;
        }
    }

    /// <summary>
    /// 获得玩家影子
    /// </summary>
    /// <param name="shadowID"></param>
    /// <returns></returns>
    private Shadow GetShadow(int shadowID)
    {
        switch (shadowID)
        {
            case 1:
                return m_shadow1;
            case 2:
                return m_shadow2;
            case 3:
                return m_shadow3;
            default:
                return null;
        }
    }

    /// <summary>
    /// 发送信息给服务端
    /// </summary>
    /// <param name="msg"></param>
    public void SendMessageToServer(string msg)
    {
        StringBuilder sendMsg = new StringBuilder("@G" + UserMessage.roomID + " ");
        sendMsg.Append(msg + "#");
        if (m_socket)
        {
            m_socket.writeSocket(sendMsg.ToString());
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    TMP_Text LVText = m_gameEndImage.Find("LVText").GetComponent<TMP_Text>();
        //    LevelUp flash = LVText.transform.Find("Flash").GetComponent<LevelUp>();
        //    flash.Flash();

        //}
        if (!GameSceneManager.m_gaming)
        {
            return;
        }
        m_map.Restore();
        for (int i = 1; i <= PlayerMsg.players.Count; i++)
        {
            Player player = GetPlayer(i);
            if (player != null && player.username != UserMessage.username && player.GetMotion() != Motion.DEATH)
            {
                m_map.SetPlayer(player.transform.position, 1f);
            }
        }
        for (int i = 1; i <= 3; i++)
        {
            Enemy enemy = GetEnemy(i);
            if (enemy.m_state != EnemyState.ENEMY_STATE_DEATH)
            {
                m_map.SetPlayer(enemy.transform.position, 1f);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_pause = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            m_gamePauseImage.SetActive(true);
            // 退出按钮
            m_gamePauseImage.transform.Find("QuitButton").GetComponent<Button>().onClick.AddListener(delegate ()
            {
                m_quitConfirmImage.SetActive(true);
                // 确认按钮
                m_quitConfirmImage.transform.Find("SureButton").GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    // 发送退出消息给服务端
                    SengBackMsg();
                    m_pause = false;
                });
                // 取消按钮
                m_quitConfirmImage.transform.Find("CancelButton").GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    m_quitConfirmImage.SetActive(false);
                });
            });
            // 返回按钮
            m_gamePauseImage.transform.Find("BackButton").GetComponent<Button>().onClick.AddListener(delegate ()
            {
                if (m_gaming)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                m_gamePauseImage.SetActive(false);
                m_pause = false;
            });
        }

    }
}

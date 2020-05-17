using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

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

    public TMP_Text m_attackStateText;
    public TMP_Text m_ammoText;

    public HP m_ownerHPImage;

    public BufferPool m_hitEnemyPool;
    public BufferPool m_hitOtherPool;
    public BufferPool m_grenadeEffectPool;
    public PropBufferPool m_propPool;

    public List<GameObject> m_grenades;
    private List<int> m_boomIndex;

    public List<GameObject> m_props;
    private List<int> m_tempProps;
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
    }

    public void ping()
    {
        if (m_socket)
        {
            m_socket.SendSynMsg();
        }
    }

    private void initPlayers()
    {
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

    public void SetAttackText(string text)
    {
        m_attackStateText.text = "当前攻击模式:" + text;
    }

    public void SetAmmoText(int nowAmmo, int bagAmmo)
    {
        m_ammoText.text = "子弹数:" + nowAmmo + "/" + bagAmmo;
    }

    public void SetHitEffect(int playerID, Vector3 pos, int hitTarge)
    {
        Player player = GetPlayer(playerID);
        if (player.username == UserMessage.username)
        {
            //return;
        }
        if (hitTarge == 0)
        {
            //Instantiate(m_hitOtherEffect, pos, Quaternion.identity);
            m_hitOtherPool.CreateInstance(pos, Quaternion.identity, 1);
        }
        else
        {
            //Instantiate(m_hitEnemyEffect, pos, Quaternion.identity);
            m_hitEnemyPool.CreateInstance(pos, Quaternion.identity, 1);
        }
    }

    public void SetPlayerState(int playerID, float serverTime, string username, Vector2 pos, float rotY, float lookPosY, int motion, int move, int attack,
                                int HP, int curAmmo, int bagAmmo, int LV, int EXP)
    {
        Player player = GetPlayer(playerID);
        Shadow shadow = GetShadow(playerID);
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

    public void InitProp()
    {
        m_tempProps = new List<int>();
    }

    public void SetProp(int id, float x,float z)
    {
        m_tempProps.Add(id);
        foreach (var item in m_props)
        {
            if(item.GetComponent<Prop>().ID == id)
            {
                return;
            }
        }
        GameObject prop = m_propPool.CreatePropInstance(new Vector3(x, 0, z), Quaternion.identity, id);
        m_props.Add(prop);
    }

    public void DestroyProp()
    {
        for(int i = 0;i < m_props.Count; i++)
        {
            if (!m_tempProps.Contains(m_props[i].GetComponent<Prop>().ID))
            {
                m_propPool.DestoryBuffer(m_props[i]);
                m_props.RemoveAt(i);
                i--;
            }
        }
    }

    public Vector2 VecRotate(Vector2 from, float angle)
    {
        float radian = angle / 180 * Mathf.PI;
        float x = from.x * Mathf.Cos(radian) + from.y * Mathf.Sin(radian);
        float y = -from.x * Mathf.Sin(radian) + from.y * Mathf.Cos(radian);
        return new Vector2(x, y);
    }

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

    private EnemyShadow GetEnemyShadow(int enemyID)
    {
        Enemy enemy = GetEnemy(enemyID);
        return enemy.m_shadow;
    }

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
        m_map.Restore();
        for (int i = 1; i <= PlayerMsg.players.Count; i++)
        {
            Player player = GetPlayer(i);
            if (player.username != UserMessage.username && player.GetMotion() != Motion.DEATH)
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

    }
}

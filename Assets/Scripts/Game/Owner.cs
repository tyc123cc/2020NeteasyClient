using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class Owner : MonoBehaviour
{
    public Player m_player;
    public GameSceneManager m_manager;

    private int m_moveState;
    private int m_attackState = AttackState.SINGLE;

    public GameObject m_camera;
    public GameObject m_upBody;

    public bool m_jump = false;

    public float sensitivityX = 0.5f;   // X轴转向移动速度
    public float sensitivityY = 0.1f;   // Y轴转向移动速度
    public float minimumY = 1.5f;       // 定义俯视最低值
    public float maximumY = 3.0f;        // 定义俯视最高值

    
    private Vector2 m_crossPos;
    public GameObject m_hitEffect;

    public Skill m_skill;

    public TMP_Text m_propText;
    private int m_targetProp = 0;

    // Start is called before the first frame update
    void Start()
    {
        Transform cross = GameObject.Find("Canvas/Cross").transform;
        m_crossPos = cross.GetComponent<RectTransform>().position;//460,230
        m_hitEffect = Resources.Load("Effects\\ShootEnemy") as GameObject;
        m_player = GetComponent<Player>();
        m_manager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
        m_skill = GameObject.Find("Canvas/Skill").GetComponent<Skill>();
        m_propText = GameObject.Find("Canvas/PropText").GetComponent<TMP_Text>();
        m_camera = Camera.main.gameObject;
        m_upBody = transform.Find("Bip001").Find("Bip001 Pelvis").Find("Bip001 Spine").gameObject;
        m_moveState = MoveState.IDLE;
        StartCoroutine(ping());
        StartCoroutine(SendRot());
       // m_p = GameObject.Find("Cube").transform;
    }
    IEnumerator SendRot()
    {
        float lastRotY = transform.eulerAngles.y;
        float lastTime = 0;
        while (true)
        {
            if (!GameSceneManager.m_gaming)
            {
                break;
            }
            float nowRotY = transform.eulerAngles.y;
            float speed = (nowRotY - lastRotY) / Time.deltaTime;
            lastRotY = nowRotY;
            if(Time.time - lastTime > 0.1f)
            {
                SendOwnerMsgToServer("UR" + UserMessage.username + " " + Time.time + " " + nowRotY + " " + speed + " " + m_player.m_looktarget.position.y);
                lastTime = Time.time;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ping()
    {
        //while (true)
        //{
        //    m_manager.ping();
        //    yield return new WaitForSeconds(15);
        //}
        yield return 0;
    }
    private float m_burstTime = 0;
    private void BurstShoot()
    {
        if (m_player.GetAttackState() == AttackState.RELOAD)
        {
            return;
        }
        if (m_player.m_curAmmo <= 0)
        {
            UnBurstShoot();
            return;
        }
        m_player.SetAttackState(AttackState.BURST);
        if(m_burstTime == 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(m_crossPos);
            RaycastHit hit;
            bool isray = Physics.Raycast(ray, out hit, 1000, ~(1 << 8 | 1 << 10));
            string username = UserMessage.username;
            //射线碰到了物体
            if (isray)
            {
                Vector3 pos = hit.point;
                if (pos.y < 0)
                {
                    pos.y = 0;
                }
                //Instantiate(m_hitEffect, pos, hit.transform.rotation);
                if(hit.collider.tag == "Enemy")
                {
                    int enemyIndex = 0;
                    if (hit.collider.name.Contains("1"))
                    {
                        enemyIndex = 1;
                    }
                    else if (hit.collider.name.Contains("2"))
                    {
                        enemyIndex = 2;
                    }
                    else if (hit.collider.name.Contains("3"))
                    {
                        enemyIndex = 3;
                    }
                    SendOwnerMsgToServer("UA" + username + " " + AttackState.BURST + " " + 1 + " " + pos.x + " " + pos.y + " " + pos.z + " " + enemyIndex);
                }
                else
                {
                    SendOwnerMsgToServer("UA" + username + " " + AttackState.BURST + " " + 1 + " " + pos.x + " " + pos.y + " " + pos.z + " " + 0);
                }
            }
            else
            {
                SendOwnerMsgToServer("UA" + username + " " + AttackState.BURST + " " + 0);
            }
            m_burstTime += Time.deltaTime;

        }
        else
        {
            m_burstTime += Time.deltaTime;
            if(m_burstTime >= 0.3f)
            {
                m_burstTime = 0;
            }
        }

    }

    private void UnBurstShoot()
    {
        if (m_player.GetAttackState() == AttackState.RELOAD)
        {
            return;
        }
        m_burstTime = 0;
        m_player.SetAttackState(AttackState.IDLE);
        string username = UserMessage.username;
        SendOwnerMsgToServer("UA" + username + " " + AttackState.IDLE + " " + 0);
    }

    private void SingleShoot()
    {
        if(m_player.GetAttackState() == AttackState.SINGLE)
        {
            return;
        }
        if(m_player.m_curAmmo <= 0 || m_player.GetAttackState() == AttackState.RELOAD)
        {
            return;
        }
        m_player.SetAttackState(AttackState.SINGLE);
        Ray ray = Camera.main.ScreenPointToRay(m_crossPos);
        RaycastHit hit;
        bool isray = Physics.Raycast(ray, out hit, 1000, ~(1 << 8 | 1 << 10));
        string username = UserMessage.username;
        //射线碰到了物体
        if (isray)
        {
            Vector3 pos = hit.point;
            if (pos.y < 0)
            {
                pos.y = 0;
            }
            //Instantiate(m_hitEffect, pos, hit.transform.rotation);
            if (hit.collider.tag == "Enemy")
            {
                int enemyIndex = 0;
                if (hit.collider.name.Contains("1"))
                {
                    enemyIndex = 1;
                }
                else if (hit.collider.name.Contains("2"))
                {
                    enemyIndex = 2;
                }
                else if (hit.collider.name.Contains("3"))
                {
                    enemyIndex = 3;
                }
                SendOwnerMsgToServer("UA" + username + " " + AttackState.SINGLE + " " + 1 + " " + pos.x + " " + pos.y + " " + pos.z + " " + enemyIndex);
            }
            else
            {
                SendOwnerMsgToServer("UA" + username + " " + AttackState.SINGLE + " " + 1 + " " + pos.x + " " + pos.y + " " + pos.z + " " + 0);
            }
        }
        else
        {
            SendOwnerMsgToServer("UA" + username + " " + AttackState.SINGLE + " " + 0);
        }

    }

    public void ReLoad()
    {
        if (m_player.GetMotion() != Motion.JUMP)
        {
            m_player.SetAttackState(AttackState.RELOAD);
            string username = UserMessage.username;
            SendOwnerMsgToServer("UA" + username + " " + AttackState.RELOAD + " " + 0);
        }
    }

    public void Throw()
    {
        if (m_skill.m_CD)
        {
            return;
        }
        m_skill.Release();
        m_player.SetAttackState(AttackState.THROW);
        string username = UserMessage.username;
        SendOwnerMsgToServer("UA" + username + " " + AttackState.THROW + " " + 0);
    }

    //public Transform m_p;
    // Update is called once per frame
    void Update()
    {
        if (!GameSceneManager.m_gaming || GameSceneManager.m_pause || m_player.GetMotion() == Motion.DEATH)
        {
            return;
        }
        //m_p.position = Camera.main.ScreenToWorldPoint(m_crossPos);
        if (Input.GetMouseButton(0) && m_attackState == AttackState.BURST && (m_player.m_attackState == AttackState.IDLE || m_player.m_attackState == AttackState.BURST))
        {
            BurstShoot();
        }
        else if(Input.GetMouseButtonUp(0) && m_attackState == AttackState.BURST)
        {
            UnBurstShoot();
        }
        else if (Input.GetMouseButtonDown(0) && m_attackState == AttackState.SINGLE && m_player.m_attackState == AttackState.IDLE)
        {
            SingleShoot();
        }
        else if(Input.GetMouseButtonDown(1) && m_player.m_attackState == AttackState.IDLE)
        {
            Throw ();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            ReLoad();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_attackState = AttackState.SINGLE;
            m_manager.SetAttackText("点射");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_attackState = AttackState.BURST;
            m_manager.SetAttackText("连射");
        }
        Move();
        Rotate();
        if(m_jump && m_player.GetMotion() != Motion.JUMP)
        {
            m_jump = false;
            SetMotion(m_player.GetMotion(),true);
        }
        PickUpProp();
    }

    private void PickUpProp()
    {
        bool nearProp = false;
        foreach (var prop in m_manager.m_props)
        {
            Vector2 propPos = new Vector2(prop.transform.position.x, prop.transform.position.z);
            Vector2 ownerPos = new Vector2(transform.position.x, transform.position.z);
            if(Vector2.Distance(propPos,ownerPos) < 1)
            {
                m_propText.gameObject.SetActive(true);
                m_targetProp = prop.GetComponent<Prop>().ID;
                nearProp = true;
                break;
            }
        }
        if (nearProp)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                string username = UserMessage.username;
                SendOwnerMsgToServer("UP" + username + " " + m_targetProp);
            }
        }
        else
        {
            if (m_propText.gameObject.activeSelf)
            {
                m_propText.gameObject.SetActive(false);
            }
            m_targetProp = 0;
        }

    }

    float rotationY = 0f;
    private void Rotate()
    {
        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
        transform.localEulerAngles = new Vector3(0, rotationX, 0);
        m_player.SetLookPosY(rotationY);

        //m_camera.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
    }

    private void SetMove(int move)
    {
        if(move != m_player.GetMoveState())
        {
            m_moveState = move;
            string username = UserMessage.username;
            SendOwnerMsgToServer("UV" + username + " " + Time.time + " " +  move);
        }
    }

    private void SetMotion(int motion,bool force)
    {
        if (!force && motion == m_player.GetMotion())
        {
            return;
        }
        m_player.SetMotion(motion);
        string username = UserMessage.username;
        float time = Time.time;
        SendOwnerMsgToServer("UM" + username + " " + Time.time + " " + motion);
    }

    private void SetMotion(int motion)
    {
        SetMotion(motion, false);
    }

    private void Move()
    {
        if (m_player.GetMotion() != Motion.JUMP)
        {
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
            {
                SetMove(MoveState.RUN_RIGHT);
                m_player.SetMoveState(MoveState.RUN_RIGHT);
            }
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
            {
                SetMove(MoveState.RUN_LEFT);
                m_player.SetMoveState(MoveState.RUN_LEFT);
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
            {
                SetMove(MoveState.BACK_LEFT);
                m_player.SetMoveState(MoveState.BACK_LEFT);
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            {
                SetMove(MoveState.BACK_RIGHT);
                m_player.SetMoveState(MoveState.BACK_RIGHT);
            }
            else if (Input.GetKey(KeyCode.W))
            {
                SetMove(MoveState.RUN);
                m_player.SetMoveState(MoveState.RUN);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                SetMove(MoveState.BACK);
                m_player.SetMoveState(MoveState.BACK);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                SetMove(MoveState.LEFT);
                m_player.SetMoveState(MoveState.LEFT);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                SetMove(MoveState.RIGHT);
                m_player.SetMoveState(MoveState.RIGHT);
            }
            else
            {
                SetMove(MoveState.IDLE);
                m_player.SetMoveState(MoveState.IDLE);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // 跳跃
                if (m_player.GetMotion() == Motion.JUMP)
                {
                    return;
                }
                if (m_player.GetMotion() == Motion.RUN || m_player.GetMotion() == Motion.WALK)
                {
                    SetMotion(Motion.JUMP);
                    m_jump = true;
                }
                else if (m_player.GetMotion() == Motion.CROUCH)
                {
                    SetMotion(Motion.WALK);
                }
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (m_player.GetMotion() == Motion.WALK)
                {
                    SetMotion(Motion.RUN);
                }
            }
            if (Input.GetKeyUp(KeyCode.LeftShift) && m_player.GetMotion() == Motion.RUN)
            {
                SetMotion(Motion.WALK);
            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (m_player.GetMotion() == Motion.RUN || m_player.GetMotion() == Motion.WALK)
                {
                    SetMotion(Motion.CROUCH);
                }
            }
        }
   
       
    }

    private void  SendOwnerMsgToServer(string msg)
    {
        m_manager.SendMessageToServer(msg);
    }

}


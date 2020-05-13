using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    private Animator m_ani;
    private int m_baseLayer;
    private int m_upLayer;

    public static Map m_map;

    public PlayerState m_state;

    private int m_moveState = MoveState.IDLE;
    public int m_attackState = AttackState.IDLE;
    private int m_motion = Motion.WALK;

    public string username = "";
    public int m_HP = 100;
    public int m_curAmmo = 30;
    public int m_bagAmmo = 70;
    public int m_LV = 1;
    public int m_EXP = 0;

    public float m_nextLookPosY;
    public float m_nextRotY;

    private Vector3 moveDirection = Vector3.zero;

    private CharacterController m_controller;

    public const float m_runSpeed = 4.0f;
    public const float m_walkSpeed = 2.0f;
    public const float m_crouchSpeed = 1f;
    public const float m_jumpSpeed = 4.0f;

    private bool m_jumpUp = false;

    private float m_startTime;

    private float m_speed = 2.0f;
    public float m_gravity = 20.0f;

    public float m_correctAngle = 5;

    public bool m_shoot = false;

    public Transform m_looktarget;
    public Transform m_shadow;

    public GameSceneManager m_manager;


    // Start is called before the first frame update
    void Start()
    {
        m_ani = GetComponent<Animator>();
        m_baseLayer = m_ani.GetLayerIndex("Base Layer");
        m_upLayer = m_ani.GetLayerIndex("Up Layer");
        m_controller = GetComponent<CharacterController>();
        m_manager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
        m_nextLookPosY = m_looktarget.position.y;
        m_nextRotY = transform.eulerAngles.y;
        m_startTime = Time.time;
        if (username != UserMessage.username)
        {
            //GetComponent<Owner>().enabled = false;
        }
        else
        {
            //m_agent.enabled = false;
        }
    }

    public void SetMotion(int motion)
    {
        if (m_motion != motion)
        {
            m_motion = motion;
            switch (motion)
            {
                case Motion.RUN:
                    m_ani.Play("Run", m_baseLayer);
                    m_speed = m_runSpeed;
                    break;
                case Motion.WALK:
                    m_ani.Play("Walk", m_baseLayer);
                    m_speed = m_walkSpeed;
                    break;
                case Motion.JUMP:
                    m_ani.Play("infantry_jump_1_start", m_baseLayer);
                    m_jumpUp = true;
                    break;
                case Motion.CROUCH:
                    m_ani.Play("Crouch", m_baseLayer);
                    m_speed = m_crouchSpeed;
                    break;
                default:
                    break;
            }
            m_ani.SetFloat("Speed", m_speed);
            SetMoveState(m_moveState, true);
        }
    }

    public int GetMotion()
    {
        return m_motion;
    }

    public void SetMoveState(int moveState)
    {
        SetMoveState(moveState, false);
    }

    public int GetMoveState()
    {
        return m_moveState;
    }

    public void SetMoveState(int moveState, bool motionChange)
    {
        if ((!motionChange && moveState == m_moveState) || (m_motion == Motion.JUMP && !m_controller.isGrounded))
        {
            return;
        }
        m_moveState = moveState;

        // 奔跑状态动画
        switch (m_moveState)
        {
            case MoveState.IDLE:
                m_ani.SetFloat("ValX", 0.0f);
                m_ani.SetFloat("ValY", 0.0f);
                break;
            case MoveState.RUN:
                m_ani.SetFloat("ValX", 0.0f);
                m_ani.SetFloat("ValY", 1.0f);
                break;
            case MoveState.RUN_LEFT:
                m_ani.SetFloat("ValX", -1.0f);
                m_ani.SetFloat("ValY", 1.0f);
                break;
            case MoveState.RUN_RIGHT:
                m_ani.SetFloat("ValX", 1.0f);
                m_ani.SetFloat("ValY", 1.0f);
                break;
            case MoveState.BACK:
                m_ani.SetFloat("ValX", 0.0f);
                m_ani.SetFloat("ValY", -1.0f);
                break;
            case MoveState.BACK_LEFT:
                m_ani.SetFloat("ValX", 1.0f);
                m_ani.SetFloat("ValY", -1.0f);
                break;
            case MoveState.BACK_RIGHT:
                m_ani.SetFloat("ValX", -1.0f);
                m_ani.SetFloat("ValY", -1.0f);
                break;
            case MoveState.LEFT:
                m_ani.SetFloat("ValX", -1.0f);
                m_ani.SetFloat("ValY", 0.0f);
                break;
            case MoveState.RIGHT:
                m_ani.SetFloat("ValX", 1.0f);
                m_ani.SetFloat("ValY", 0.0f);
                break;
            default:
                break;
        }

    }

    public void SetAttackState(int attackState)
    {
        if (m_motion != Motion.JUMP && attackState != m_attackState)
        {
            m_attackState = attackState;
            switch (m_attackState)
            {
                case AttackState.IDLE:
                    m_ani.Play("Empty",m_upLayer);
                    m_shoot = false;
                    break;
                case AttackState.SINGLE:
                    m_shoot = true;
                    if(m_motion == Motion.WALK || m_motion == Motion.RUN || m_motion == Motion.JUMP)
                    {
                        m_ani.Play("infantry_combat_shoot", m_upLayer);
                    }
                    else
                    {
                        m_ani.Play("infantry_crouch_shoot", m_upLayer);
                    }
                    break;
                case AttackState.BURST:
                    m_shoot = true;
                    if (m_motion == Motion.WALK || m_motion == Motion.RUN || m_motion == Motion.JUMP)
                    {
                        m_ani.Play("infantry_combat_shoot_burst", m_upLayer);
                    }
                    else
                    {
                        m_ani.Play("fantry_crouch_shoot_burst", m_upLayer);
                    }
                    break;
                case AttackState.RELOAD:
                    m_shoot = true;
                    m_ani.Play("infantry_combat_reload", m_upLayer);
                    break;
                case AttackState.THROW:
                    m_shoot = true;
                    m_ani.Play("infantry_throw_grenade", m_upLayer);
                    break;
                default:
                    break;
            }
        }
    }

    public int GetAttackState()
    {
        return m_attackState;
    }

    public void SetHP(int HP)
    {
        m_HP = HP;
    }

    public int GetHP()
    {
        return m_HP;
    }

    public void SetAmmo(int curAmmo, int bagAmmo)
    {
        m_curAmmo = curAmmo;
        m_bagAmmo = bagAmmo;
    }

    public void SetExp(int exp)
    {
        m_EXP = exp;
    }

    public int GetEXP()
    {
        return m_EXP;
    }

    public void SetLV(int lv)
    {
        m_LV = lv;
    }

    public int GetLV()
    {
        return m_LV;
    }

    public void SetLookPosY(float y)
    {
        m_looktarget.localPosition = new Vector3(0, y, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (username == UserMessage.username)
        {
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(m_shadow.position.x, m_shadow.position.z)) < 2)
            {
                Move();
            }
            else
            {
                transform.position = new Vector3(m_shadow.position.x, transform.position.y, m_shadow.position.z);
            }
        }
        else
        {
            Rotate();
            //m_agent.speed = m_speed;
            //m_agent.SetDestination(m_shadow.position);
            Vector2 transPos = new Vector2(transform.position.x, transform.position.z);
            Vector2 targetPos = new Vector2(m_shadow.position.x, m_shadow.position.z);
            Vector2 nextPos = Vector2.MoveTowards(transPos, targetPos, m_speed * Time.deltaTime);
            float moveY = transform.position.y;
            if (m_motion == Motion.JUMP)
            {
                if (m_jumpUp)
                {
                    moveY += 2.0f * Time.deltaTime;
                    if (moveY >= 1.0f)
                    {
                        m_jumpUp = false;
                    }
                }
                else
                {
                    moveY -= 2.0f * Time.deltaTime;
                    if (moveY <= 0.0f)
                    {
                        moveY = 0.0f;
                        //SetMotion(Motion.WALK);
                    }
                }
            }
            transform.position = new Vector3(nextPos.x, moveY, nextPos.y);
        }
        //SetState();
    }
    private void FixedUpdate()
    {
        //Move();
        //SetState();
    }
    Vector3 oldPos;
    float oldTime;
    private void SetState()
    {
        //if(m_state == null)
        //{
        //    return;
        //}
        //if(Vector2.Distance(new Vector2(m_state.pos.x,m_state.pos.z),new Vector2(transform.position.x,transform.position.z)) > 1.0f)
        //{
        //    transform.position = new Vector3(m_state.pos.x, transform.position.y, m_state.pos.z);
        //}
        //if(m_state.username != UserMessage.username)
        //{
        //    transform.eulerAngles = m_state.rot;
        //    m_looktarget.localPosition = m_state.lookTarget;
        //    SetMotion(m_state.motion);
        //    SetMoveState(m_state.move);
        //    SetAttackState(m_state.attack);
        //}
        //SetAmmo(m_state.curAmmo, m_state.bagAmmo);
        //SetLV(m_state.LV);
        //SetExp(m_state.EXP);
        //m_state = null;
    }

    private void Rotate()
    {
        if (username != UserMessage.username)
        {
            float rotateY = Mathf.LerpAngle(transform.eulerAngles.y, m_nextRotY, 20 * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, rotateY, 0);
            float posY = Mathf.Lerp(m_looktarget.position.y, m_nextLookPosY, 2 * Time.deltaTime);
            // posY = Mathf.Clamp(posY, 1.5f, 3.0f);
            m_looktarget.localPosition = new Vector3(0, posY, 1);

        }
    }

    private void Move()
    {
        float moveY = transform.position.y;
        if (m_motion == Motion.JUMP)
        {
            if (m_jumpUp)
            {
                moveY += 2.0f * Time.deltaTime;
                if (moveY >= 1.0f)
                {
                    m_jumpUp = false;
                }
            }
            else
            {
                moveY -= 2.0f * Time.deltaTime;
                if (moveY <= 0.0f)
                {
                    moveY = 0.0f;
                    SetMotion(Motion.WALK);
                }
            }
        }
        {
            Vector3 move = Vector3.zero;
            if (m_moveState == MoveState.IDLE)
            {
                m_startTime = Time.time;
            }
            if (m_moveState == MoveState.RUN)
            {
                // 前进
                move = new Vector3(0, 0, 1);
            }
            else if (m_moveState == MoveState.BACK)
            {
                // 后退
                move = new Vector3(0, 0, -1);
            }
            else if (m_moveState == MoveState.LEFT)
            {
                // 左移
                move = new Vector3(-1, 0, 0);
            }
            else if (m_moveState == MoveState.RIGHT)
            {
                // 右移
                move = new Vector3(1, 0, 0);
            }
            else if (m_moveState == MoveState.RUN_LEFT)
            {
                // 左前
                move = new Vector3(-Mathf.Sqrt(0.5f), 0, Mathf.Sqrt(0.5f));
            }
            else if (m_moveState == MoveState.RUN_RIGHT)
            {
                //右前
                move = new Vector3(Mathf.Sqrt(0.5f), 0, Mathf.Sqrt(0.5f));
            }
            else if (m_moveState == MoveState.BACK_LEFT)
            {
                // 左后
                move = new Vector3(-Mathf.Sqrt(0.5f), 0, -Mathf.Sqrt(0.5f));
            }
            else if (m_moveState == MoveState.BACK_RIGHT)
            {
                // 右后
                move = new Vector3(Mathf.Sqrt(0.5f), 0, -Mathf.Sqrt(0.5f));
            }
            //moveDirection = transform.TransformDirection(moveDirection);
            //Vector2 dire = CorrectDirect(new Vector2(moveDirection.x, moveDirection.z));
            //moveDirection.Set(dire.x, moveDirection.y, dire.y);
            //moveDirection *= m_speed;
            Vector2 pos = new Vector2(transform.position.x, transform.position.z);
            move = transform.TransformDirection(move);
            Vector2 moveDirect = CorrectDirect(new Vector2(move.x, move.z));

            Vector2 serverPos = pos + moveDirect * m_speed * 0.12f;

            moveDirect *= m_speed * (Time.time - m_startTime);
            m_startTime = Time.time;
            pos += moveDirect;

            if (m_map.isObstancle(serverPos.x, serverPos.y))
            {
                pos = new Vector2(transform.position.x, transform.position.z);
            }
            transform.position = new Vector3(pos.x, moveY, pos.y);
        }
        moveDirection.y -= m_gravity * Time.deltaTime;
        //m_controller.Move(moveDirection * Time.deltaTime);
        //Vector2 nextPos = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.z), new Vector2(m_nextPos.x, m_nextPos.z), m_speed * Time.fixedDeltaTime);
        //if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(m_nextPos.x, m_nextPos.z)) > 1.0f)
        //{
        //    transform.position = new Vector3(m_nextPos.x, transform.position.y, m_nextPos.z);
        //}
    }

    private Vector2 CorrectDirect(Vector2 moveDirect)
    {
        Vector2 shadowPos = new Vector2(m_shadow.position.x, m_shadow.position.z);
        Vector2 actalVector = shadowPos - new Vector2(transform.position.x, transform.position.z);
        float angle = Vector2.SignedAngle(actalVector, moveDirect);
        Vector2 newDirect = moveDirect;
        if (Mathf.Abs(angle) > 0 && Mathf.Abs(angle) <= m_correctAngle)
        {
            newDirect = m_manager.VecRotate(moveDirect, angle);
        }
        else if (Mathf.Abs(angle) > m_correctAngle)
        {
            newDirect = m_manager.VecRotate(moveDirect, angle > 0 ? m_correctAngle : -m_correctAngle);
        }
        return newDirect;
    }

    private void LateUpdate()
    {       
        if(m_shoot && m_ani.GetCurrentAnimatorStateInfo(1).IsName("Empty"))
        {
            UnShoot();
        }
    }

    private void UnShoot()
    {
        SetAttackState(AttackState.IDLE);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        // 用IK来实现视角变化时抬头和抬手动作
        m_ani.SetLookAtWeight(1.0f, 1.0f, 1.0f, 1.0f);
        m_ani.SetLookAtPosition(m_looktarget.position);
    }
}

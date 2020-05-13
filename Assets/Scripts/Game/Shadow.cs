using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public int m_motion = Motion.WALK;
    public float m_speed = 0;
    public int m_moveState = MoveState.IDLE;

    public static Map m_map;

    public float m_startTime;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetState(Vector2 pos,float rotY,int motion,int move)
    {
        transform.position = new Vector3(pos.x, transform.position.y, pos.y);
        transform.rotation = Quaternion.Euler(new Vector3(0, rotY, 0));
        m_motion = motion;
        m_moveState = move;
        if(motion == Motion.CROUCH)
        {
            m_speed = Player.m_crouchSpeed;
        }
        else if(motion == Motion.WALK)
        {
            m_speed = Player.m_walkSpeed;
        }
        else if(motion == Motion.RUN)
        {
            m_speed = Player.m_runSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = Vector3.zero;
        //if (moveDirection.y <= 0 && m_controller.isGrounded)
        {
            if(m_moveState == MoveState.IDLE)
            {
                m_startTime = Time.time;
                return;
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
            Vector2 pos = new Vector2(transform.position.x, transform.position.z);
            move = transform.TransformDirection(move);
            move *= m_speed * (Time.time - m_startTime);
            m_startTime = Time.time;
            pos += new Vector2(move.x, move.z);
            if (m_map.isObstancle(pos.x, pos.y))
            {
                pos = new Vector2(transform.position.x, transform.position.z);
            }
            transform.position = new Vector3(pos.x, transform.position.y, pos.y);
        }
    }
}

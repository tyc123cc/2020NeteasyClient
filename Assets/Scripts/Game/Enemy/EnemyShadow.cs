using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShadow : MonoBehaviour
{
    public float m_speed = 2;

    public int m_state = EnemyState.ENEMY_STATE_IDLE;

    public Vector2 m_nextPos;

    public float m_startTime;

    // Start is called before the first frame update
    void Start()
    {
        m_nextPos = transform.position;
    }

    public void SetState(Vector2 pos,Vector2 nextPos,int state)
    {
        transform.position = new Vector3(pos.x, transform.position.y, pos.y);
        m_nextPos = new Vector3(nextPos.x,transform.position.y,nextPos.y);
        m_state = state;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_state == EnemyState.ENEMY_STATE_MOVE)
        {
            //transform.position = Vector3.MoveTowards(transform.position, m_nextPos, m_speed * Time.deltaTime);
        }
    }
}

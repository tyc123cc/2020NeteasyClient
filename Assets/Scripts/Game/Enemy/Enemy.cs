using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int m_id;
    public int m_HP = 100;
    public Vector3 m_pos;
    public Vector3 m_nextPos;
    public int m_range;

    public int m_speed = 2;
    public int m_state;

    public EnemyShadow m_shadow;

    public HP m_enemyHP;

    private Transform m_leg;
    private Transform m_head;
    private Vector2 m_oriProp;

    private Animator m_ani;

    public DamageNum m_damageNum;

    public AudioSource m_attackAudioSource;
    public AudioSource m_injuredAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        m_pos = transform.position;
        m_nextPos = m_pos;
        m_state = EnemyState.ENEMY_STATE_IDLE;
        m_enemyHP.SetMaxHP(100);
        m_enemyHP.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.7f, 1);
        m_ani = GetComponent<Animator>();
        m_leg = transform.Find("head_J");
        m_head = transform.Find("Legs_A");
        float offset = Mathf.Abs(Camera.main.WorldToScreenPoint(m_head.position).y - Camera.main.WorldToScreenPoint(m_leg.position).y);
        m_oriProp = new Vector2(0.3f / offset, 0.5f / offset);

        m_attackAudioSource = transform.Find("Attack").GetComponent<AudioSource>();
        m_injuredAudioSource = transform.Find("Injured").GetComponent<AudioSource>();
    }

    public void SetState(int hp,int state,Transform attackTarget)
    {
        SetHP(hp);
        if(m_state == state)
        {
            return;
        }
        m_state = state;
        switch (state)
        {
            case EnemyState.ENEMY_STATE_DIZZY:
                float offset = Mathf.Abs(Camera.main.WorldToScreenPoint(m_head.position).y - Camera.main.WorldToScreenPoint(m_leg.position).y);
                m_damageNum.SetText(transform, offset / 2, "麻痹");
                if (Vector3.Distance(transform.position, m_shadow.transform.position) >= 2)
                {
                    transform.position = m_shadow.transform.position;
                }
                gameObject.SetActive(true);
                m_enemyHP.gameObject.SetActive(true);
                GetComponent<CharacterController>().enabled = true;
                m_ani.Play("Idle");
                break;
            case EnemyState.ENEMY_STATE_IDLE:
                if (Vector3.Distance(transform.position, m_shadow.transform.position) >= 2)
                {
                    transform.position = m_shadow.transform.position;
                }
                gameObject.SetActive(true);
                m_enemyHP.gameObject.SetActive(true);
                GetComponent<CharacterController>().enabled = true;
                m_ani.Play("Idle");
                break;
            case EnemyState.ENEMY_STATE_DEATH:
                Death();
                break;
            case EnemyState.ENEMY_STATE_MOVE:
                if (Vector3.Distance(transform.position, m_shadow.transform.position) >= 2)
                {
                    transform.position = m_shadow.transform.position;
                }
                gameObject.SetActive(true);
                m_enemyHP.gameObject.SetActive(true);
                GetComponent<CharacterController>().enabled = true;
                m_ani.Play("Move");
                break;
            case EnemyState.ENEMY_STATE_ATTACK:
                m_attackAudioSource.Play();
                if (Vector3.Distance(transform.position, m_shadow.transform.position) >= 2)
                {
                    transform.position = m_shadow.transform.position;
                }
                gameObject.SetActive(true);
                m_enemyHP.gameObject.SetActive(true);
                GetComponent<CharacterController>().enabled = true;
                transform.LookAt(attackTarget);
                m_ani.Play("Attack");
                Invoke("AttackCD", 1);
                break;
        }
    }

    private void AttackCD()
    {
        if(m_state == EnemyState.ENEMY_STATE_ATTACK)
        {
            SetState(m_HP, EnemyState.ENEMY_STATE_IDLE, transform);
        }
    }

    private void Death()
    {
        m_ani.Play("Death");
        GetComponent<CharacterController>().enabled = false;
        StartCoroutine(resurgence());
    }

    IEnumerator resurgence()
    {
        // 2秒后清除尸体
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
        m_enemyHP.gameObject.SetActive(false);
    }

    private void SetHP(int hp)
    {
        if(m_HP < hp)
        {
            m_enemyHP.AddHP(hp - m_HP);
        }
        else if(m_HP > hp)
        {
            m_enemyHP.ReduceHP(m_HP - hp);
            float offset = Mathf.Abs(Camera.main.WorldToScreenPoint(m_head.position).y - Camera.main.WorldToScreenPoint(m_leg.position).y);
            m_damageNum.SetText(transform, offset / 2,(m_HP - hp).ToString());
            m_injuredAudioSource.Play();

        }
        m_HP = hp;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_state == EnemyState.ENEMY_STATE_MOVE && Vector3.Distance(transform.position, m_shadow.transform.position) < 2)
        {
            transform.LookAt(m_shadow.transform.position);
            transform.position = Vector3.MoveTowards(transform.position, m_shadow.transform.position, m_speed * Time.deltaTime);
        }
        else if (Vector3.Distance(transform.position, m_shadow.transform.position) >= 2)
        {
            transform.position = m_shadow.transform.position;
        }
        Vector3 HPPos = Camera.main.WorldToScreenPoint(transform.position);
        float offset = Mathf.Abs(Camera.main.WorldToScreenPoint(m_head.position).y - Camera.main.WorldToScreenPoint(m_leg.position).y);
        m_enemyHP.transform.position = new Vector3(HPPos.x, HPPos.y + offset * 1.3f, 0);
        float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        m_enemyHP.GetComponent<RectTransform>().localScale = new Vector3(0.4f - distance / 150, 0.6f - distance / 150, 1);
    }

    private void UnShoot()
    {

    }

}

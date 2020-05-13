using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    public Slider m_greenHP;
    public Slider m_redHP;

    public Image m_greenHPFill;
    public Image m_redHPFill;

    public int m_maxHP;

    public float m_bleedSpeed = 20;

    // Start is called before the first frame update
    void Start()
    {
        m_greenHP.maxValue = m_maxHP;
        m_redHP.maxValue = m_maxHP;
        m_greenHP.value = m_maxHP;
        m_redHP.value = m_maxHP;
    }

    public void SetMaxHP(int maxHP)
    {
        m_maxHP = maxHP;
        m_greenHP.maxValue = m_maxHP;
        m_redHP.maxValue = m_maxHP;
        m_greenHP.value = m_maxHP;
        m_redHP.value = m_maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    ReduceHP(1);
        //}
        //if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    AddHP(1);
        //}
        if (m_redHP.value != m_greenHP.value)
        {
            m_redHP.value = Mathf.MoveTowards(m_redHP.value, m_greenHP.value, m_bleedSpeed * Time.deltaTime);
            if(m_redHP.value <= 0)
            {
                Color color = m_redHPFill.color;
                m_redHPFill.color = new Color(color.r, color.g, color.b, 0);
            }
        }
    }

    public void ReduceHP(int value)
    {
        m_greenHP.value -= value;
        if(m_greenHP.value <= 0)
        {
            Color color = m_greenHPFill.color;
            m_greenHPFill.color = new Color(color.r, color.g, color.b, 0);
        }
    }

    public void AddHP(int value)
    {
        Color color = m_greenHPFill.color;
        m_greenHPFill.color = new Color(color.r, color.g, color.b, 1);
        color = m_redHPFill.color;
        m_redHPFill.color = new Color(color.r, color.g, color.b, 1);
        m_greenHP.value += value;
        if(m_redHP.value <= m_greenHP.value)
        {
            m_redHP.value = m_greenHP.value;
        }
    }
}

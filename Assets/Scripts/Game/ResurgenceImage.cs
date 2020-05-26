using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResurgenceImage : MonoBehaviour
{
    public TMP_Text m_text;

    public Vector3 m_oriPos;
    public Vector3 m_resetPos;

    public float m_moveSpeed = 50;

    public bool m_death = false;

    // Start is called before the first frame update
    void Start()
    {
        m_text = transform.Find("Text").GetComponent<TMP_Text>();
        m_oriPos = transform.position;
        m_resetPos = transform.Find("ResetPos").position;
        m_moveSpeed = 200;
    }

    public void Death(int time)
    {
        m_death = true;
        m_text.text = "复活时间:" + time.ToString();
    }

    public void Resurgence()
    {
        m_death = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_death)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_resetPos, m_moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, m_oriPos, m_moveSpeed * Time.deltaTime);
        }
    }
}

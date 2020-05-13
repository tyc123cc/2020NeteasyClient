using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNum : MonoBehaviour
{
    public TMP_Text m_text;
    private Transform m_from;
    private float m_up;
    private float m_offset;
    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<TMP_Text>();
        gameObject.SetActive(false);
    }

    public void SetText(Transform from, float offset,string text)
    {
        m_text.text = text;
        m_from = from;
        m_offset = offset;
        m_up = 0;
        m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, 1);
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(m_from != null)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(m_from.position);
            pos.y += m_offset + (m_up += 10 * Time.deltaTime);
            transform.position = pos;
            m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, m_text.color.a - Time.deltaTime);
            if(m_text.color.a <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}

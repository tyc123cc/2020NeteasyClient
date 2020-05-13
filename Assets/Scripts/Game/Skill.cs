using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Skill : MonoBehaviour
{
    public Image m_CDImage;
    public TMP_Text m_CDText;

    public bool m_CD = false;
    public float m_CDTimeSet = 5;

    private float m_CDTime;
    // Start is called before the first frame update
    void Start()
    {
        m_CDImage.fillAmount = 0;
        m_CDText.text = "";
        m_CDTime = m_CDTimeSet;
    }

    public void Release()
    {
        m_CD = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (m_CD)
        {
            m_CDTime -= Time.deltaTime;
            m_CDText.text = ((int)(m_CDTime + 1)).ToString();
            m_CDImage.fillAmount = m_CDTime / m_CDTimeSet;
            if (m_CDTime <= 0)
            {
                m_CD = false;
                m_CDTime = m_CDTimeSet;
                m_CDText.text = "";
                m_CDImage.fillAmount = 0;
            }
        }
    }
}

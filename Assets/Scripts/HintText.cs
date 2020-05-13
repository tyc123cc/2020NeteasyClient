using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintText : MonoBehaviour
{
    TMP_Text m_hintText;
    // Start is called before the first frame update
    void Start()
    {
        m_hintText = GetComponent<TMP_Text>();
        Color color = m_hintText.color;
        m_hintText.color = new Color(color.r, color.g, color.b, 0.0f);
        transform.SetAsFirstSibling();
    }

    // Update is called once per frame
    void Update()
    {
        Color color = m_hintText.color;
        if (color.a > 0)
        {
            m_hintText.color = new Color(color.r, color.g, color.b, color.a - 0.5f * Time.deltaTime);
        }
        if(color.a <= 0)
        {
            transform.SetAsFirstSibling();
        }
    }

    public void SetText(string msg)
    {
        m_hintText.SetText(msg);
        Color color = m_hintText.color;
        m_hintText.color = new Color(color.r, color.g, color.b, 1.0f);
        transform.SetAsLastSibling();
    }
}

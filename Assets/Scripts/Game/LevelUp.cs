using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUp : MonoBehaviour
{
    private Image m_image;
    public List<Sprite> m_ani;
    public bool m_aniPlay = false;
    public float m_frequency = 0.1f;
    private int m_aniIndex = 0;
    private float time = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_image = GetComponent<Image>();
    }

    public void Flash()
    {
        m_aniIndex = 0;
        time = 0;
        m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, 1);
        m_aniPlay = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_aniPlay)
        {
            m_image.sprite = m_ani[m_aniIndex];
            time += Time.deltaTime;
            m_aniIndex = (int)(time / m_frequency);
            if(m_aniIndex >= m_ani.Count)
            {
                m_aniPlay = false;
                m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, 0);
            }
        }
    }
}

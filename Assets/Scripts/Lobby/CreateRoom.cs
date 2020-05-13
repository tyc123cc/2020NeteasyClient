using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreateRoom : MonoBehaviour
{
    public LobbySceneManager m_manager;

    private int m_userNum = 1;
    private int m_map = 1;

    public TMP_Text m_maxUserNumText;
    public GameObject m_maxUserNumAdvanceArrow;
    public GameObject m_maxUserNumBackArrow;
    public int m_maxUserNum = 3;

    public TMP_Text m_mapText;
    public GameObject m_mapAdvanceArrow;
    public GameObject m_mapBackArrow;
    public int m_maxMapNum = 1;

   
    // Start is called before the first frame update
    void Start()
    {
        m_manager = GameObject.Find("LobbySceneManager").GetComponent<LobbySceneManager>();
        m_maxUserNumBackArrow.SetActive(false);
        m_mapBackArrow.SetActive(false);
        if(m_userNum >= m_maxUserNum)
        {
            m_maxUserNumAdvanceArrow.SetActive(false);
        }
        if(m_map >= m_maxMapNum)
        {
            m_mapAdvanceArrow.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Create()
    {
        if (m_manager.m_ani)
        {
            return;
        }
        string msg = "@BC" + m_map + " " + m_userNum +  "#";
        m_manager.m_socket.writeSocket(msg);
    }

    /// <summary>
    /// 地图的增加按钮
    /// </summary>
    public void MapAdvance()
    {
        m_map++;
        m_mapText.text = m_map.ToString();
        if (!m_mapBackArrow.activeInHierarchy)
        {
            m_mapBackArrow.SetActive(true);
        }
        if(m_map >= m_maxMapNum)
        {
            m_mapAdvanceArrow.SetActive(false);
        }
    }

    /// <summary>
    /// 地图的减少按钮
    /// </summary>
    public void MapBack()
    {
        m_map--;
        m_mapText.text = m_map.ToString();
        if (!m_mapAdvanceArrow.activeInHierarchy)
        {
            m_mapAdvanceArrow.SetActive(true);
        }
        if (m_map <= 1)
        {
            m_mapBackArrow.SetActive(false);
        }
    }

    /// <summary>
    /// 最大用户数目增加
    /// </summary>
    public void UserNumAdvance()
    {
        m_userNum++;
        m_maxUserNumText.text = m_userNum.ToString();
        if (!m_maxUserNumBackArrow.activeInHierarchy)
        {
            m_maxUserNumBackArrow.SetActive(true);
        }
        if (m_userNum >= m_maxUserNum)
        {
            m_maxUserNumAdvanceArrow.SetActive(false);
        }
    }

    /// <summary>
    /// 最大用户数目减少
    /// </summary>
    public void UserNumBack()
    {
        m_userNum--;
        m_maxUserNumText.text = m_userNum.ToString();
        if (!m_maxUserNumAdvanceArrow.activeInHierarchy)
        {
            m_maxUserNumAdvanceArrow.SetActive(true);
        }
        if (m_userNum <= 1)
        {
            m_maxUserNumBackArrow.SetActive(false);
        }
    }
}

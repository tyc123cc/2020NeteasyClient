using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Reconnect : MonoBehaviour
{
    networkSocket m_socket;
    public static bool m_reconnect = false;
    public TMP_Text m_text;
    public GameObject m_sureButton;
    public GameObject m_noButton;
    // Start is called before the first frame update
    void Start()
    {
        m_socket = GameObject.Find("Network").GetComponent<networkSocket>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    IEnumerator connect()
    {
        // 将重连于按下按钮后2帧执行，否则无法更新UI
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        m_socket.setupSocket();
        if (m_socket.socket_ready)
        {
            if (UserMessage.username.CompareTo("") != 0)
            {
                m_socket.writeSocket("@E" + UserMessage.username + "#");
            }
            m_reconnect = false;
        }
        
        Destroy(gameObject);
    }

    /// <summary>
    /// 按下确定按钮，重新连接
    /// </summary>
    public void Sure()
    {
        m_text.text = "重连中...";
        m_sureButton.SetActive(false);
        m_noButton.SetActive(false);
        StartCoroutine(connect());
        
    }

    /// <summary>
    /// 按下取消按钮 返回主界面
    /// </summary>
    public void NO()
    {
        m_reconnect = false;
        SceneManager.LoadScene("Start");
    }
}

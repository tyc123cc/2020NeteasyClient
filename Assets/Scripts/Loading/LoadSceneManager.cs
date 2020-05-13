using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    public static int m_map;
    public static int m_userNum;

    public networkSocket m_socket;

    public TMP_Text m_user1NameText;
    public TMP_Text m_user1ProcessText;

    public TMP_Text m_user2NameText;
    public TMP_Text m_user2ProcessText;

    public TMP_Text m_user3NameText;
    public TMP_Text m_user3ProcessText;

    private TMP_Text m_user1Name;
    private TMP_Text m_user1Process;

    private TMP_Text m_user2Name;
    private TMP_Text m_user2Process;

    private TMP_Text m_user3Name;
    private TMP_Text m_user3Process;

    public Slider m_slider;
    public float m_progressValue;

    public bool m_ready = false;

    private AsyncOperation async;
    // Start is called before the first frame update
    void Start()
    {
        m_socket = GameObject.Find("Network").GetComponent<networkSocket>();
        SetUserNum(m_userNum);
        m_socket.SendSynMsg();
        StartCoroutine("LoadScene");
    
    }

    public void SetUserNum(int num)
    {
        if(num == 1)
        {
            m_user1Name = m_user2NameText;
            m_user1Process = m_user2ProcessText;
            m_user1NameText.gameObject.SetActive(false);
            m_user1ProcessText.gameObject.SetActive(false);
            m_user3NameText.gameObject.SetActive(false);
            m_user3ProcessText.gameObject.SetActive(false);
        }
        else if(num == 2)
        {
            m_user1Name = m_user1NameText;
            m_user1Process = m_user1ProcessText;
            m_user2Name = m_user3NameText;
            m_user2Process = m_user3ProcessText;
            m_user2NameText.gameObject.SetActive(false);
            m_user2ProcessText.gameObject.SetActive(false);
        }
        else if(num == 3)
        {
            m_user1Name = m_user1NameText;
            m_user1Process = m_user1ProcessText;
            m_user2Name = m_user2NameText;
            m_user2Process = m_user2ProcessText;
            m_user3Name = m_user3NameText;
            m_user3Process = m_user3ProcessText;
        }
    }

    public void SetProcess(int ID,string username,float process)
    {
        if(ID == 1)
        {
            m_user1Name.text = username;
            m_user1Process.text = Mathf.RoundToInt(process * 100) + "%";
        }
        else if(ID == 2)
        {
            m_user2Name.text = username;
            m_user2Process.text = Mathf.RoundToInt(process * 100) + "%";
        }
        else if(ID == 3)
        {
            m_user3Name.text = username;
            m_user3Process.text = Mathf.RoundToInt(process * 100) + "%";
        }
    }

    /// <summary>
    /// 所有玩家准备完毕，可以开始游戏
    /// </summary>
    public void Ready()
    {
        if (m_user1Process)
        {
            m_user1Process.text = "100%";
        }
        if (m_user2Process)
        {
            m_user2Process.text = "100%";
        }
        if (m_user3Process)
        {
            m_user3Process.text = "100%";
        }
        m_ready = true;
    }

    private void SendProcess()
    {
        m_socket.writeSocket("@G" + UserMessage.roomID + " L" + UserMessage.playerID + " " + m_progressValue + "#");
    }

    IEnumerator LoadScene()
    {
        if (m_map == 1)
        {
            async = SceneManager.LoadSceneAsync("Game_Desert");
        }
        async.allowSceneActivation = false;
        while (!async.isDone)
        {
            if (async.progress < 0.9f)
                m_progressValue = async.progress + 0.1f;
            else
                m_progressValue = 1.0f;

            m_slider.value = m_progressValue;
            SendProcess();

            if (m_ready)
            {
                async.allowSceneActivation = true;
                print("gameStart");
            }
            // 每0.5秒更新一次进度条
            yield return new WaitForSeconds(0.25f);
        }

        yield return null;
    }
}

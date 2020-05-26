using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginSceneManager : MonoBehaviour
{
    public networkSocket m_socket;
    public TMP_InputField m_usernameInputField;
    public TMP_InputField m_passwordInputField;

    public Toggle m_rememberUsernameToggle;
    public Toggle m_rememberPasswordToggle;

    public TMP_Text m_hintText;

    // Start is called before the first frame update
    void Start()
    {
        m_socket = GameObject.Find("Network").GetComponent<networkSocket>();
        if (PlayerPrefs.HasKey("Username"))
        {
            m_usernameInputField.text = PlayerPrefs.GetString("Username");
            m_rememberUsernameToggle.isOn = true;
        }
        if (PlayerPrefs.HasKey("Password"))
        {
            m_passwordInputField.text = PlayerPrefs.GetString("Password");
            m_rememberPasswordToggle.isOn = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    /// 检测用户名和密码的合法性（不允许为空或有特殊符号，长度小于20）
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    /// <returns></returns>
    private bool validity(string username, string password)
    {
        if (username.Length == 0 || password.Length == 0 || username.Length > 20 || password.Length > 20)
        {
            return false;
        }
        foreach (char c in username)
        {
            if (!char.IsLetterOrDigit(c))
            {
                return false;
            }
        }
        foreach (char c in password)
        {
            if (!char.IsLetterOrDigit(c))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 显示注册结果
    /// </summary>
    /// <param name="res">注册成功与否</param>
    public void showRegisterRes(bool res)
    {
        if (res)
        {
            m_hintText.text = "注册成功";
        }
        else
        {
            m_hintText.text = "已存在该用户,请重新注册";
        }
    }

    /// <summary>
    /// 显示登陆结果
    /// </summary>
    /// <param name="msg">从服务器反馈到客户端的用户数据</param>
    /// <param name="res">登陆成功与否</param>
    public void showLoginRes(string userMsg,bool res)
    {
        if (res)
        {
            string[] msg = userMsg.Split(' ');
            UserMessage.userID = int.Parse(msg[0]);
            UserMessage.username = msg[1];
            UserMessage.HP = int.Parse(msg[2]);
            UserMessage.ammo = int.Parse(msg[3]);
            UserMessage.LV = int.Parse(msg[4]);
            UserMessage.EXP = int.Parse(msg[5]);
            m_hintText.text = "登陆成功,请稍等";
            SceneManager.LoadScene("Lobby");
            
        }
        else
        {
            m_hintText.text = "登陆失败,用户名或密码错误";
        }
    }

    /// <summary>
    /// 发送注册信息
    /// </summary>
    public void register()
    {
        if(Reconnect.m_reconnect || Quit.m_quit)
        {
            return;
        }
        string username = m_usernameInputField.text;
        string password = m_passwordInputField.text;
        if (validity(username, password)){
            m_socket.writeSocket("@R" + username + " " + password + "#");
        }
        else
        {
            m_hintText.text = "用户名和密码不合法(不为空,不含特殊符号,长度小于20),请重新输入";
        }
         
    }

    /// <summary>
    /// 发送登录信息
    /// </summary>
    public void login()
    {
        if (Reconnect.m_reconnect || Quit.m_quit)
        {
            return;
        }
        string username = m_usernameInputField.text;
        string password = m_passwordInputField.text;
        if (validity(username, password))
        {
            // 存储用户名
            if (m_rememberUsernameToggle.isOn)
            {
                PlayerPrefs.SetString("Username", username);
            }
            else if(PlayerPrefs.HasKey("Username"))
            {
                PlayerPrefs.DeleteKey("Username");
            }
            if (m_rememberPasswordToggle.isOn)
            {
                PlayerPrefs.SetString("Password", password);
            }
            else if (PlayerPrefs.HasKey("Password"))
            {
                PlayerPrefs.DeleteKey("Password");
            }
            m_socket.writeSocket("@L" + username + " " + password + "#");
        }
        else
        {
            m_hintText.text = "用户名和密码不合法(不为空,不含特殊符号,长度小于20),请重新输入";
        }
    }
}

using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class networkSocket : MonoBehaviour
{
    public String host = Dns.GetHostName();
    public Int32 port = 12345;

    internal Boolean socket_ready = false;
    internal String input_buffer = "";
    TcpClient tcp_socket;
    NetworkStream net_stream;

    StreamWriter socket_writer;
    StreamReader socket_reader;

    NetTextParse parse;

    public GameObject m_reconnectObj;

    List<string> msgList;

    public static Dictionary<float, float> delay = new Dictionary<float, float>();

    public static float timeDif;

    public int targetFrameRate = 30;

    private void Awake()
    {
        //修改当前的FPS
        Application.targetFrameRate = targetFrameRate;
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().name == "Start")
        {
            Destroy(gameObject);
        }
        if(SceneManager.GetActiveScene().name != "Start" && parse == null)
        {
            parse = GameObject.Find("NetTextParse").GetComponent<NetTextParse>();
        }
        if(delay.Count >= 12)
        {
            float delayTime = CalDelay();
            writeSocket("@ST" + delayTime + " " + Time.time + "#");
            delay.Clear();
            
        }
        if (socket_ready)
        {
            string received_data = readSocket();
            if (received_data != "")
            {
                // Do something with the received data,
                // print it in the log for now
                parse.parse(received_data);
                if (!received_data.Contains("GUR"))
                {
                    //print(received_data);
                }
               

            }
        }
        //if(msgList.Count > 0)
        //{
        //    for(int i = 0;i < msgList.Count;i++)
        //    {
        //        parse.parse(msgList[i]);
        //    }
        //    msgList.Clear();
        //}

    }

    IEnumerator SendDelayMsg()
    {
        while (!socket_ready)
        {
            yield return new WaitForSecondsRealtime(0.01f);
        }
        for(int i = 0;i < 12; i++)
        {
            writeSocket("@SD" + Time.time + "#");
            yield return new WaitForSecondsRealtime(0.2f);
        }
        yield return new WaitForSecondsRealtime(0.5f);
        while(delay.Count > 0 && delay.Count < 12)
        {
            writeSocket("@SD" + Time.time + "#");
            yield return new WaitForSecondsRealtime(0.5f);
        }
        yield return null;
        
    }

    public float CalDelay()
    {
        List<float> list = new List<float>();
        foreach (var start in delay.Keys)
        {
            list.Add(delay[start] - start);
        }
        list.Sort();
        list.RemoveAt(0);
        list.RemoveAt(list.Count - 1);
        float ave = 0;
        foreach (var item in list)
        {
            ave += item;
        }
        return ave / list.Count;
    }

    private void Receive()
    {
        //while (true)
        //{
        //    if (socket_ready)
        //    {
        //        string received_data = readSocket();
        //        if (received_data != "")
        //        {
        //            // Do something with the received data,
        //            // print it in the log for now
        //            //parse.parse(received_data);
        //            msgList.Add(received_data);
        //            print(received_data);

        //        }
        //    }
           
        //    Thread.Sleep(10);
        //}
        
    }

    public void SendSynMsg()
    {
        StartCoroutine(SendDelayMsg());
    }


    void Start()
    {
        m_reconnectObj = Resources.Load("Prefabs\\ReconnectImage") as GameObject;
        msgList = new List<string>();
        setupSocket();
        DontDestroyOnLoad(gameObject);
        Thread thread = new Thread(new ThreadStart(Receive));
        thread.Start();
        //print(Vector2.SignedAngle(Vector2.down, Vector2.left));
        //m_reconnectObj.SetActive(false);

    }


    void OnApplicationQuit()
    {
        closeSocket();
    }

    public void setupSocket()
    {
        try
        {
            tcp_socket = new TcpClient(host, port);

            net_stream = tcp_socket.GetStream();
            socket_writer = new StreamWriter(net_stream);
            socket_reader = new StreamReader(net_stream);
            
            socket_ready = true;
        }
        catch (Exception e)
        {
        	// Something went wrong
            Debug.Log("Socket error: " + e);
            reconnect();
        }
    }

    public void writeSocket(string line)
    {
        if (!socket_ready)
            return;


        try
        {           
            socket_writer.Write(line);
            socket_writer.Flush();
        }
        catch (Exception e)
        {
            // Something went wrong
            Debug.Log("Socket error: " + e);
            reconnect();
        }
    }

    public String readSocket()
    {
        if (!socket_ready)
            return "";

        if (net_stream.DataAvailable)
            return socket_reader.ReadLine();

        return "";
    }

    public void reconnect()
    {
        closeSocket();
        if (!Quit.m_quit)
        {
            Reconnect.m_reconnect = true;
            GameObject reconnectGameObj = Instantiate(m_reconnectObj, GameObject.Find("Canvas").transform);
        }         
    }

    public void closeSocket()
    {
        if (!socket_ready)
            return;

        socket_writer.Close();
        socket_reader.Close();
        tcp_socket.Close();
        socket_ready = false;
    }

}
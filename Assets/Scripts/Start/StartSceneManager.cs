using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UserMessage.userID = 0;
        UserMessage.username = "";
        UserMessage.HP = 0;
        UserMessage.ammo = 0;
        UserMessage.LV = 0;
        UserMessage.EXP = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Login");
    }
}

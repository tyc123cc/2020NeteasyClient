using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Quit : MonoBehaviour
{
    public TMP_Text m_hintText;
    public static bool m_quit = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string text)
    {
        m_hintText.text = text;
        m_quit = true;
    }

    public void OK()
    {
        m_quit = false;
        SceneManager.LoadScene("Start");
    }
}

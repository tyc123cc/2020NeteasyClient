using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneMail : MonoBehaviour
{
    public GameObject mItemPrefab;
    public Transform mContentTransform;
    public Scrollbar mScrollbar;
    // Use this for initialization
    void Start()
    {
        Color c = transform.GetComponent<Image>().color;
        c.r = 1;
        c.b = 0;
        c.g = 0;
        transform.GetComponent<Image>().color = c;
        //mScrollbar.value = 1.0f;
    }

    /// <summary>
    /// 显示Item列表
    /// </summary>
    void ShowItems()
    {
        for (int i = 0; i < 20; i++)
        {
            GameObject item = Instantiate(mItemPrefab,mContentTransform);
        }
    }
}

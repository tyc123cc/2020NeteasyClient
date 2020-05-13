using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public BufferPool pool;
    public GameObject m_obj;
    // Start is called before the first frame update
    void Start()
    {
        pool.SetBufferPool(m_obj, 10);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 mousePosOnScreen = Input.mousePosition;
            mousePosOnScreen.z = screenPos.z;
            Vector3 mousePosInWorld = Camera.main.ScreenToWorldPoint(mousePosOnScreen);
            pool.CreateInstance(new Vector3(10,5,2), Quaternion.identity, 1);
        }
    }
}

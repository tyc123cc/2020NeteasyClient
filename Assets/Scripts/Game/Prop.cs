using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    private float m_upLength;
    private bool m_up;

    public int ID;
    // Start is called before the first frame update
    void Start()
    {
        m_upLength = 1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,Time.deltaTime * 20,0));
        if(m_up)
        {
            m_upLength += Time.deltaTime;
        }
        else
        {
            m_upLength -= Time.deltaTime;
        }
        if(m_upLength > 1.5f)
        {
            m_up = false;
        }
        if(m_upLength < 0.5f)
        {
            m_up = true;
        }
        transform.position = new Vector3(transform.position.x, m_upLength, transform.position.z);
    }
}

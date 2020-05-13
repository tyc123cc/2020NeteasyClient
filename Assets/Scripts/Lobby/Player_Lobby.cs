using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Lobby : MonoBehaviour
{
    private Vector2 m_lastMousePos;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDrag()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x - Screen.width / 2f, Input.mousePosition.y - Screen.height / 2f);
        transform.Rotate(new Vector3(0,(m_lastMousePos.x - mousePos.x) * Time.deltaTime * 20,0));
        m_lastMousePos = mousePos;
    }

    private void OnMouseDown()
    {
        m_lastMousePos = new Vector2(Input.mousePosition.x - Screen.width / 2f, Input.mousePosition.y - Screen.height / 2f);
    }

    private void OnMouseUp()
    {
        
    }
}

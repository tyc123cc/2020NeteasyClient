using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCamera : MonoBehaviour
{
    //目标对象
    public Transform target;
    //垂直方向偏移
    public float distanceUp = 15f;
    //水平方向偏移
    public float distanceAway = 10f;
    //位置平滑值
    public float posSmooth = 2f;

    // X轴偏移
    public float distanceRight = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LateUpdate()
    {
        //相机的位置
        Vector3 targetPos = target.position + Vector3.up * distanceUp - target.forward * distanceAway + target.right * distanceRight;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * posSmooth);
        //相机的角度
        transform.LookAt(target.position);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

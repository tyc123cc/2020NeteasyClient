using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float Power = 10;//代表发射时的速度/力度等，可以通过此来模拟不同的力大小
    public float Angle = 45;//发射的角度
    public float Gravity = -10;//代表重力加速度
    public bool IsOne = false;
    private Vector3 MoveSpeed;//初速度向量
    private Vector3 GritySpeed = Vector3.zero;//重力的速度向量，t时为0
    private float dTime;//已经过去的时间
    private Vector3 currentAngle;

    public int m_from;
    // Use this for initialization
    void Start()
    {
        //通过一个公式计算出初速度向量
        //角度*力度
        MoveSpeed = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y - 90, Angle)) * Vector3.right * Power;
        //currentAngle = transform.eulerAngles;
    }
    private void OnBecameVisible()
    {
        //通过一个公式计算出初速度向量
        //角度*力度
        MoveSpeed = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y - 90, Angle)) * Vector3.right * Power;
        dTime = 0;
        //currentAngle = transform.eulerAngles;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //计算物体的重力速度
        //v = at ;
        GritySpeed.y = Gravity * (dTime += Time.fixedDeltaTime);
        //位移模拟轨迹
        transform.position += (MoveSpeed + GritySpeed) * Time.fixedDeltaTime;
        if (transform.position.y < 0)
        {
            gameObject.SetActive(false);
        }
        //currentAngle.z = Mathf.Atan((GritySpeed.y) / 10) * Mathf.Rad2Deg;
        //transform.eulerAngles = currentAngle;
    }
}

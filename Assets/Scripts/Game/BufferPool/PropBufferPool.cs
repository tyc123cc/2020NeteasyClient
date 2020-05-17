using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropBufferPool : BufferPool
{
    public GameObject CreatePropInstance(Vector3 pos, Quaternion quaternion,int ID)
    {
        GameObject gameObject = CreateInstance(pos, quaternion, 0);
        gameObject.GetComponent<Prop>().ID = ID;
        return gameObject;
    }
}

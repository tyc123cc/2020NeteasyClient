using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferPool:MonoBehaviour
{
    class Buffer
    {
        public GameObject obj;
        public bool created;
    }
    List<Buffer> m_buffers;
    List<int> m_destroyIndex;
    public void SetBufferPool(GameObject bufferObj,int num)
    {
        m_buffers = new List<Buffer>();
        m_destroyIndex = new List<int>();
        for (int i = 0;i < num; i++)
        {
            Buffer buffer = new Buffer();
            buffer.obj = GameObject.Instantiate(bufferObj);
            buffer.obj.SetActive(false);
            buffer.created = false;
            m_buffers.Add(buffer);
        }
    }

    public GameObject CreateInstance(Vector3 pos,Quaternion quaternion,float time)
    {
        GameObject instance = null;
        int index = 0;
        for(int i = 0;i < m_buffers.Count;i++)
        {
            if (!m_buffers[i].created)
            {
                Buffer buffer = m_buffers[i];
                buffer.created = true;
                m_buffers[i] = buffer;
                instance = buffer.obj;
                index = i;
                break;
            }
        }
        if(instance == null)
        {
            GameObject obj = m_buffers[0].obj;
            instance = GameObject.Instantiate(obj, pos, quaternion);
            if(time != 0)
            {
                GameObject.Destroy(instance, time);
            }
        }
        else
        {
            instance.transform.position = pos;
            instance.transform.rotation = quaternion;
            instance.SetActive(true);
            if(time != 0)
            {
                m_destroyIndex.Add(index);
                Invoke("DestoryBuffer", time);
            }
        }
        return instance;
    }

    public void DestoryBuffer(GameObject buffer)
    {
        for(int i = 0;i < m_buffers.Count;i++)
        {
            if(m_buffers[i].obj == buffer)
            {
                m_buffers[i].obj.SetActive(false);
                m_buffers[i].created = false;
                return;
            }
        }
        Destroy(buffer);
    }

    private void DestoryBuffer()
    {
        int index = m_destroyIndex[0];
        m_destroyIndex.RemoveAt(0);
        m_buffers[index].obj.SetActive(false);
        m_buffers[index].created = false;
    }

}

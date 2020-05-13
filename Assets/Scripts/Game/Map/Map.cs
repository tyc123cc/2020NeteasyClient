using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public int min_X = 0;
    public int min_Y = 0;
    public int moveLength = 0;
    public int moveWidth = 0;
    public int mapLength = 0;
    public int mapWidth = 0;
    public int[,] map;

    public void initMap(int minX, int minY, int moveL, int moveW, int mapL, int mapW)
    {
        min_X = minX;
        min_Y = minY;
        moveLength = moveL;
        moveWidth = moveW;
        mapLength = mapL;
        mapWidth = mapW;
        map = new int[mapL, mapW];
        for (int i = 0; i < mapL; i++)
        {
            for (int j = 0; j < mapW; j++)
            {
                map[i, j] = 1;
            }
        }
        for (int i = min_X; i < min_X + moveL; i++)
        {
            for (int j = min_Y; j < min_Y + moveW; j++)
            {
                map[i, j] = 0;
            }
        }
    }

    public bool isObstancle(float x,float z)
    {
        if(map[(int)(x),(int)(z)] == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void printMap()
    {
        string res = "";
        for (int i = 0; i < mapLength; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                res += map[i, j] + " ";
            }
            res += "\n";
        }
        print(res);
    }

    public void setObstacle(int startX, int startY, int width, int length)
    {
        for (int i = startX; i < startX + width; i++)
        {
            for(int j = startY;j < startY + length; j++)
            {
                //print("map" + i + "," + j + " " + map[i, j]);
                map[i, j] = 1;
            }
        }
    }

    public void SetPlayer(Vector3 pos,float radius)
    {
        for(int i = (int)((pos.x - radius));i < (int)((pos.x + radius)); i++)
        {
            for (int j = (int)((pos.z - radius)); j < (int)((pos.z + radius)); j++)
            {
                map[i, j] = 1;
            }
        }
    }

    public virtual void Restore()
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

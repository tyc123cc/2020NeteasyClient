using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map01 : Map
{
    public void Start()
    {
        initMap(15, 15, 70, 70, 100, 100);
        setObstacle(75, 65, 3, 3);
        setObstacle(76, 56, 3, 3);
        setObstacle(71, 75, 3, 3);
        setObstacle(60, 78, 3, 3);
        setObstacle(34, 52, 3, 3);
        setObstacle(70, 36, 2, 1);
        Player.m_map = this;
        Shadow.m_map = this;
    }

    public override void Restore()
    {
        initMap(15, 15, 70, 70, 100, 100);
        setObstacle(75, 65, 3, 3);
        setObstacle(76, 56, 3, 3);
        setObstacle(71, 75, 3, 3);
        setObstacle(60, 78, 3, 3);
        setObstacle(34, 52, 3, 3);
        setObstacle(70, 36, 2, 1);
    }
}

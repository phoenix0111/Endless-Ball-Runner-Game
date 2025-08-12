

using System;
using UnityEngine;

[Serializable]
public class ObstacleData
{
    public ObstacleType obstacleType = ObstacleType.None;
    public Obstacle[] prefabs;
    [Min(10)]
    public int maxCount = 20;
}

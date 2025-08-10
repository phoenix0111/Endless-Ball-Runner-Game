

using System;
using UnityEngine;

[Serializable]
public class ObstacleData
{
    public LaneHeightType obstacleHeight = LaneHeightType.Small;
    public ObstacleType obstacleType = ObstacleType.Stone;
    public Obstacle prefab;
    [Min(10)]
    public int maxCount = 20;
}

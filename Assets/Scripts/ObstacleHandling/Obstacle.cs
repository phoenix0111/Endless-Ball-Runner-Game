using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private ObstacleType obstacleType = ObstacleType.None;
    [SerializeField] private LaneHeightType laneHeightType = LaneHeightType.None;
    
    public ObstacleType ObstacleType { get => obstacleType; }
    public LaneHeightType LaneHeightType { get => laneHeightType; }
}

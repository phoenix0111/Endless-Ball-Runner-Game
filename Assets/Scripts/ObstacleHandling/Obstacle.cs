using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private ObstacleType obstacleType = ObstacleType.None;
    [SerializeField] private LaneHeightType laneHeightType = LaneHeightType.None;
    [Min(0.1f)]
    [SerializeField] private float height = 2.0f;
    public ObstacleType ObstacleType { get => obstacleType; }
    public LaneHeightType LaneHeightType { get => laneHeightType; }
    public float Height { get => height;}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * height);
    }
}

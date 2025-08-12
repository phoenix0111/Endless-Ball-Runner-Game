using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private ObstacleType obstacleType = ObstacleType.None;
    [Min(0.1f)]
    [SerializeField] private float height = 2.0f;
    public ObstacleType ObstacleType { get => obstacleType; }
    public float Height { get => height;}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * height);
    }
}

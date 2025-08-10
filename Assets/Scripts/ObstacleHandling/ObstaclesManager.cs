using System.Collections.Generic;
using System.Linq;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class ObstaclesManager : MonoBehaviour
{
    [SerializeField] private ObstacleData[] obstaclesData;
    [SerializeField] private LayerMask obstacleLayerMask;
    [Min(0.01f)]
    [SerializeField] private float obstacleCheckRadius = 0.3f;

    private Dictionary<ObstacleKey, ObjectPool<Obstacle>> obstaclePools;
    private List<ObstacleKey> obstacleKeys;

    public float ObstacleCheckRadius { get => obstacleCheckRadius; set => obstacleCheckRadius = value; }

    public bool IsObstacleThere(Vector3 position)
    {
        return Physics.CheckSphere(position, obstacleCheckRadius, obstacleLayerMask.value);
    }

    public Obstacle Spawn(Vector3 position, Quaternion rotation, ObstacleType type, LaneHeightType laneHeightType)
    {
        //Debug.Log("Getting obstacle");
        IEnumerable<ObstacleKey> enumerable = obstacleKeys.Where(value => value.Type == type && 
                                                value.Height == laneHeightType);
        
        if(enumerable.Count() == 0)
        {
            return null;
        }

        var key = enumerable.ToArray()[0];
        if(!obstaclePools.ContainsKey(key))
        {
            return null;
        }
        var instance = obstaclePools[key].Get();
        if (instance != null)
        {
            instance.transform.position = position;
            instance.transform.rotation = rotation;
        }

        return instance;
    }

    public void Unspawn(Obstacle obstacle)
    {
        if(obstacle == null)
        {
            return;
        }

        //Debug.Log("Unspawning obstacle");
        ObstacleType type = obstacle.ObstacleType;
        LaneHeightType laneHeightType = obstacle.LaneHeightType;

        IEnumerable<ObstacleKey> enumerable = obstacleKeys.Where(value => value.Type == type &&
                                                value.Height == laneHeightType);

        if (enumerable.Count() == 0)
        {
            Destroy(obstacle.gameObject);
            return;
        }
        var key = enumerable.ToArray()[0];
        obstaclePools[key].ReturnToPool(obstacle);
    }

    private Obstacle CreateObstacle(ObstacleData data)
    {
        var instance = Instantiate(data.prefab, Vector3.zero, Quaternion.identity);
        instance.transform.parent = transform;
        instance.gameObject.SetActive(false);
        return instance;
    }

    private void OnGetObstacle(Obstacle obstacle)
    {
        obstacle.gameObject.SetActive(true);
    }

    private void OnReturnObstacle(Obstacle obstacle)
    {
        obstacle.gameObject.SetActive(false);
    }

    private void OnDestroyObstacle(Obstacle obstacle)
    {
        Destroy(obstacle.gameObject);
    }

    private void Awake()
    {
        obstaclePools = new Dictionary<ObstacleKey, ObjectPool<Obstacle>>();
        obstacleKeys = new List<ObstacleKey>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ServiceLocator.ForSceneOf(this).Register<ObstaclesManager>(this);

        if (obstaclePools.Count == 0)
        {
            foreach(var data in obstaclesData)
            {
                var key = new ObstacleKey(data.obstacleType, data.obstacleHeight);

                var obstaclePool = new ObjectPool<Obstacle>(() => CreateObstacle(data),
                                                            OnGetObstacle,
                                                            OnReturnObstacle,
                                                            OnDestroyObstacle,
                                                            data.maxCount);

                obstaclePools.Add(key, obstaclePool);
                obstacleKeys.Add(key);
            }
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, obstacleCheckRadius);
    }

}
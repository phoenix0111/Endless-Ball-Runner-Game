using System.Collections.Generic;
using UnityEngine;

public class ObstaclesManager : MonoBehaviour
{
    [SerializeField] private ObstacleData[] obstaclesData;
    [SerializeField] private LayerMask obstacleLayerMask;
    [Min(0.01f)]
    [SerializeField] private float obstacleCheckRadius = 0.3f;

    private Dictionary<ObstacleType, ObjectPool<Obstacle>> obstaclePools;
    private Collider[] detectedColliders;
    public float ObstacleCheckRadius { get => obstacleCheckRadius; set => obstacleCheckRadius = value; }

    public bool IsObstacleThere(Vector3 position)
    {
        return Physics.CheckSphere(position, obstacleCheckRadius, obstacleLayerMask.value);
    }

    public bool IsObstacleThere(Vector3 position, float radius, out Obstacle obstacle)
    {
        int count = Physics.OverlapSphereNonAlloc(position, radius, detectedColliders, obstacleLayerMask.value);
        if(count == 0)
        {
            obstacle = null;
            return false;
        }
        obstacle = detectedColliders[0].gameObject.GetComponent<Obstacle>();
        return true;
    }

    public Obstacle Spawn(Vector3 position, Quaternion rotation, ObstacleType type)
    {
        //Debug.Log("Getting obstacle");
        
        if(!obstaclePools.ContainsKey(type))
        {
            return null;
        }
        var instance = obstaclePools[type].Get();
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
        
        if (!obstaclePools.ContainsKey(type))
        {
            Destroy(obstacle.gameObject);
            return;
        }
        obstaclePools[type].ReturnToPool(obstacle);
    }

    private Obstacle CreateObstacle(ObstacleData data)
    {
        var randomPrefab = data.prefabs[Random.Range(0, data.prefabs.Length)];
        var instance = Instantiate(randomPrefab, Vector3.zero, Quaternion.identity);
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
        obstaclePools = new Dictionary<ObstacleType, ObjectPool<Obstacle>>();
        detectedColliders = new Collider[Constants.MAX_COLLIDER_COUNT];
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ServiceLocator.ForSceneOf(this).Register<ObstaclesManager>(this);

        if (obstaclePools.Count == 0)
        {
            foreach(var data in obstaclesData)
            {
                var key = data.obstacleType;

                var obstaclePool = new ObjectPool<Obstacle>(() => CreateObstacle(data),
                                                            OnGetObstacle,
                                                            OnReturnObstacle,
                                                            OnDestroyObstacle,
                                                            data.maxCount);

                obstaclePools.Add(key, obstaclePool);
            }
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, obstacleCheckRadius);
    }

}
using UnityEngine;
using System.Collections.Generic;

public class PathGenerator : MonoBehaviour
{
    [Header("Path Settings")]
    public GameObject[] pathPrefabs;
    public int initialSpawnCount = 5;
    public float pathLength = 10f;

    [Header("References")]
    public Transform Player; // The object whose position will decide spawning
    public Transform spawnStartPoint;    // Where the first tile should spawn

    private List<GameObject> activePaths = new List<GameObject>();
    private float spawnZ;
    private int safeZone = 15;

    void Start()
    {
        spawnZ = spawnStartPoint.position.z;

        // Spawn initial paths
        for (int i = 0; i < initialSpawnCount; i++)
        {
            if (i == 0)
                SpawnPath(0);
            else
                SpawnPath();
        }
    }

    void Update()
    {
        if (Player.position.z - safeZone > (spawnZ - initialSpawnCount * pathLength))
        {
            SpawnPath();
            DeleteOldPath();
        }
    }

    public void SpawnPath(int prefabIndex = -1)
    {
        GameObject go;
        if (prefabIndex == -1)
            go = Instantiate(pathPrefabs[Random.Range(0, pathPrefabs.Length)]) as GameObject;
        else
            go = Instantiate(pathPrefabs[prefabIndex]) as GameObject;

        go.transform.SetParent(transform);
        go.transform.position = new Vector3(0, 0, spawnZ);
        spawnZ += pathLength;
        activePaths.Add(go);
    }

    void DeleteOldPath()
    {
        Destroy(activePaths[0]);
        activePaths.RemoveAt(0);
    }
}

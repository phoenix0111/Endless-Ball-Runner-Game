

using UnityEngine;
using System.Collections.Generic;

public class MountainSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject prefab;
    public int maxSpawned = 5;     // max objects in scene at once
    public float gap = 150f;
    public float startZ = 0f;

    private float nextZ;
    private bool spawnLeft = true;
    private Queue<GameObject> spawnedObjects = new Queue<GameObject>();

    void Start()
    {
        nextZ = startZ;

        // Spawn initial objects
        for (int i = 0; i < maxSpawned; i++)
        {
            SpawnObject();
        }
    }

    void Update()
    {
        if (ShouldSpawn())
        {
            SpawnObject();

            // Destroy oldest if exceeding limit
            if (spawnedObjects.Count > maxSpawned)
            {
                GameObject oldObj = spawnedObjects.Dequeue();
                Destroy(oldObj);
            }
        }
    }

    void SpawnObject()
    {
        float xPos = spawnLeft ? -49f : 59f;
        Vector3 spawnPos = new Vector3(xPos, -45f, nextZ);

        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);
        spawnedObjects.Enqueue(obj);

        nextZ += gap;
        spawnLeft = !spawnLeft;
    }

    bool ShouldSpawn()
    {
        // Example condition: player near nextZ
        return Camera.main.transform.position.z + (gap * 5) >= nextZ;
    }
}

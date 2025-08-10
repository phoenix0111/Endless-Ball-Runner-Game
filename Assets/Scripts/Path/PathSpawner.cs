using UnityEngine;
using System.Collections.Generic;

public class PathSpawner : MonoBehaviour
{
    public Transform player; // Player reference
    public Chunk[] pathPrefabs; // Different types of paths (some with gaps)
    public float distanceBetweenChunks = 3.0f; // Length of each path tile
    public int startTileCount = 7; // Number of tiles at start
    public int deleteThresholdIndex = 3;
    public int spawnTriggerIndex = 5;
    [Min(0.1f)]
    public float pathDistance = 50.0f;

    private float spawnZ = 0f;
   
    private List<Chunk> activeTiles = new List<Chunk>();

    void Start()
    {
        // Spawn initial path tiles
        for (int i = 0; i < startTileCount; i++)
        {
            SpawnTile(false);
        }
    }

    void Update()
    {
        
            float playerZ = player.position.z;

            // Check if we need to spawn a new tile
            float distanceZ = playerZ + (startTileCount - spawnTriggerIndex) * pathDistance;
            Debug.Log("DistanceZ: " + distanceZ + ", SpawnZ: " + spawnZ);
            if (distanceZ > spawnZ)
            {
                SpawnTile(true);
            }

            // Delete tiles that are behind the player
            // You can adjust this 'safe zone' value
            float safeZone = pathDistance * 2;

            if (activeTiles.Count > 0 && playerZ - activeTiles[0].transform.position.z > safeZone)
            {
                DeleteTile();
            }
        

    }

    void SpawnTile(bool generateObstacles)
    {
        // Randomly pick a prefab from the list
        Chunk prefab = pathPrefabs[Random.Range(0, pathPrefabs.Length)];
        Chunk newTile = Instantiate(prefab, Vector3.forward * spawnZ, Quaternion.identity);
        newTile.ShouldGenerateObstacles = generateObstacles;
        newTile.StartCoroutine(newTile.GenerateObstacles());
        activeTiles.Add(newTile);
        spawnZ += newTile.Length + distanceBetweenChunks;
    }

    void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }
}

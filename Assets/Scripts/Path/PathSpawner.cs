using UnityEngine;
using System.Collections.Generic;

public class PathSpawner : MonoBehaviour
{
    public Transform player; // Player reference
    public GameObject[] pathPrefabs; // Different types of paths (some with gaps)
    public float pathLength = 10f; // Length of each path tile
    public int startTileCount = 7; // Number of tiles at start
    public int deleteThresholdIndex = 3;
    public int spawnTriggerIndex = 5;

    private float spawnZ = 0f;
   
    private List<GameObject> activeTiles = new List<GameObject>();

    void Start()
    {
        // Spawn initial path tiles
        for (int i = 0; i < startTileCount; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        
            float playerZ = player.position.z;

            // Check if we need to spawn a new tile
            if (playerZ + (startTileCount - spawnTriggerIndex) * pathLength > spawnZ)
            {
                SpawnTile();
            }

            // Delete tiles that are behind the player
            // You can adjust this 'safe zone' value
            float safeZone = pathLength * 2;

            if (activeTiles.Count > 0 && playerZ - activeTiles[0].transform.position.z > safeZone)
            {
                DeleteTile();
            }
        

    }

    void SpawnTile()
    {
        // Randomly pick a prefab from the list
        GameObject prefab = pathPrefabs[Random.Range(0, pathPrefabs.Length)];
        GameObject newTile = Instantiate(prefab, Vector3.forward * spawnZ, Quaternion.identity);
        activeTiles.Add(newTile);
        spawnZ += pathLength;
    }

    void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }
}

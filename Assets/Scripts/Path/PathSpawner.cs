using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;

public class PathSpawner : MonoBehaviour
{
   public Transform player; // Player reference
    [SerializeField] private Chunk[] pathPrefabs; // Different types of paths (some with gaps)
    [SerializeField] private float distanceBetweenChunks = 3.0f; // Length of each path tile
    [Min(1)]
    [SerializeField] private int startTilesWithoutObstaclesCount = 2;
    [Min(1)]
    [SerializeField] private int startTilesWithObstaclesCount = 7; // Number of tiles with obstacles at start
    [Min(1)]
    [SerializeField] private int tilesCountAfterResettingOrigin = 5;
    [Min(0.1f)]
    [SerializeField] private float pathDistance = 50.0f;
    

    private float spawnZ = 0f;
   
    private List<Chunk> activeTiles = new List<Chunk>();

    void Start()
    {
        ServiceLocator.ForSceneOf(this).Register<PathSpawner>(this);
        Random.InitState((int)System.DateTime.Now.Ticks);
        // Spawn initial path tiles
        for (int i = 0; i < startTilesWithoutObstaclesCount; i++)
        {
            SpawnTile(false);
        }
            for (int i = 0; i < startTilesWithObstaclesCount; i++)
        {
            SpawnTile(true);
        }
    }

    void FixedUpdate()
    {
        float playerZ = 0;
        if (player != null) playerZ = player.position.z;
      
        // Check if we need to spawn a new tile
        //If yes, then spawn it.
        //And then reset origin of all the things.
        //Then delete the tiles behind the player.


        Debug.Log("Path Distance: " + pathDistance + ", PlayerZ: " + playerZ);
        if (playerZ > pathDistance)
        {
            ResetOriginOfTiles(playerZ);
            player.transform.position -= Vector3.forward * playerZ;
            DeleteTiles();
            for(int i = 0; i < tilesCountAfterResettingOrigin; i++)
            {
                SpawnTile(true);
            }
        }

    }

    

    private void ResetOriginOfTiles(float distance)
    {
        foreach (Chunk chunk in activeTiles)
        {
            chunk.MoveBack(distance);
        }

        spawnZ -= distance;
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

    void DeleteTiles()
    {
        int lastTileIndexBehindThePlayer = -1;
        for(int i = 0; i < activeTiles.Count; i++)
        {
            Chunk chunk = activeTiles[i];
            if(chunk.transform.position.z < player.transform.position.z)
            {
                lastTileIndexBehindThePlayer = i;
            }
            else
            {
                break;
            }
        }

        if (lastTileIndexBehindThePlayer > 0)
        {
            lastTileIndexBehindThePlayer--;
        }

        for (int i = 0; i < lastTileIndexBehindThePlayer; i++)
        {
            Destroy(activeTiles[0]);
            activeTiles.RemoveAt(0);
        }

        
    }
}

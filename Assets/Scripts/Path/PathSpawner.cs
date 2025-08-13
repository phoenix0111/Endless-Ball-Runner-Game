using UnityEngine;
using System.Collections.Generic;

public class PathSpawner : MonoBehaviour
{
    public Transform player; // Player reference
    [SerializeField] private Chunk[] startingChunks;
    [SerializeField] private Chunk[] pathPrefabs; // Different types of paths (some with gaps)
    [Min(100)]
    [SerializeField] private int maxChunkCount = 100;
    [SerializeField] private float distanceBetweenChunks = 3.0f; // Length of each path tile
    [Min(1)]
    [SerializeField] private int startingTilesCount = 7; // Number of tiles with obstacles at start
    [Min(1)]
    [SerializeField] private int tilesCountAfterResettingOrigin = 5;
    [Min(0.1f)]
    [SerializeField] private float pathDistance = 50.0f;
    
    private float spawnZ = 0f;
    private List<Chunk> activeTiles = new List<Chunk>();
    private ObjectPool<Chunk> chunkPool;

    void Start()
    {
        if (chunkPool == null)
        {
            chunkPool = new ObjectPool<Chunk>(CreateChunk,
                                              OnGetChunk,
                                              OnReturnChunk,
                                              OnDestroyChunk,
                                              maxChunkCount);
        }
        ServiceLocator.ForSceneOf(this).Register<PathSpawner>(this);
        Random.InitState((int)System.DateTime.Now.Ticks);
        
        //Adjust spawnZ for initial tiles
        AdjustSpawnZ();

        
        //Then spawn tiles with obstacles
        for (int i = 0; i < startingTilesCount; i++)
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

    private Chunk CreateChunk()
    {
        int randomIndex = Random.Range(0, pathPrefabs.Length);
        var instance = Instantiate(pathPrefabs[randomIndex], Vector3.zero, Quaternion.identity);
        instance.transform.parent = transform;
        instance.gameObject.SetActive(false);
        return instance;
    }

    private void OnGetChunk(Chunk chunk)
    {
        chunk.gameObject.SetActive(true);
    }

    private void OnReturnChunk(Chunk chunk)
    {
        chunk.gameObject.SetActive(false);
    }

    private void OnDestroyChunk(Chunk chunk)
    {
        Destroy(chunk.gameObject);
    }

    private void ResetOriginOfTiles(float distance)
    {
        foreach (Chunk chunk in activeTiles)
        {
            chunk.MoveBack(distance);
        }

        spawnZ -= distance;
    }

    private void AdjustSpawnZ()
    {
        for(int i = 0; i < startingChunks.Length; i++)
        {
            Chunk current = startingChunks[i];
            Chunk next = (i + 1 < startingChunks.Length) ? startingChunks[i + 1] : null;
            spawnZ += current.Length;
            if (next != null) 
            {
                float gap = (next.transform.position.z - (current.transform.position.z + current.Length));
                spawnZ += gap;
            }

            activeTiles.Add(current);
        }

        spawnZ += distanceBetweenChunks;
    }

    void SpawnTile(bool generateObstacles)
    {
        // Randomly pick a prefab from the list
        Chunk prefab = pathPrefabs[Random.Range(0, pathPrefabs.Length)];
        Chunk newTile = chunkPool.Get();//Instantiate(prefab, Vector3.forward * spawnZ, Quaternion.identity);
        newTile.gameObject.transform.position = Vector3.forward * spawnZ;
        newTile.gameObject.transform.rotation = Quaternion.identity;

        newTile.ShouldGenerateObstacles = generateObstacles;
        newTile.StartCoroutine(newTile.GenerateObstacles());
        newTile.StartCoroutine(newTile.GenerateCoins());

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
            if(!chunkPool.ReturnToPool(activeTiles[0]))
            {
                Destroy(activeTiles[0].gameObject);
            }
            activeTiles.RemoveAt(0);
        }
    }
}

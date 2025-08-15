using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathGenerator : MonoBehaviour
{
    [Header("Path Settings")]
    public GameObject[] pathPrefabs;
    public int initialSpawnCount = 5;
    public float pathLength = 10f;

    [Header("Special Path Settings")]
    public int blueTileIndex = 7;     // blue rare tile
    public int yellowTileIndex = 8;   // yellow special tile
      
    public int blueTileCooldownLength = 6; // how many tiles after blue until it can appear again
    public float blueTileChance = 0.1f; // 10% chance when allowed

    private bool queueSpecial = false;
  
    private int lastSpecialIndex = -1;

    private int blueTileCooldown = 0; // prevents blue spawning too often
    private bool firstnobluetile = false; 
    [Header("References")]
    public Transform Player;
    public Transform spawnStartPoint;
    public Gamemanager gameManager;
    public CoinSpawner coinSpawner;

    private List<GameObject> activePaths = new List<GameObject>();
    private float spawnZ;
    [SerializeField] int safeZone = 15;

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
        // Check for yellow special milestone
        int currentScore = gameManager.score;
        if (currentScore == 350 || currentScore == 920 || currentScore == 1420)
        {
            queueSpecial = true;
        }

        // Spawn new path when player moves forward enough
        if (Player.position.z - safeZone > (spawnZ - initialSpawnCount * pathLength))
        {
            SpawnPath();
            DeleteOldPath();
        }
    }

    public void SpawnPath(int prefabIndex = -1)
    {
        GameObject go;

        if (queueSpecial)
        {
            // Spawn yellow special tile
            go = Instantiate(pathPrefabs[yellowTileIndex]);
            lastSpecialIndex = yellowTileIndex;
            queueSpecial = false;

            // Yellow tile also progresses blue cooldown
            if (blueTileCooldown > 0) blueTileCooldown--;
        }
        else
        {
            int randIndex;

            if (blueTileCooldown > 0)
            {
                // Avoid spawning blue or yellow while cooldown active
                randIndex = Random.Range(0, pathPrefabs.Length);
                while (randIndex == yellowTileIndex || randIndex == blueTileIndex)
                    randIndex = Random.Range(0, pathPrefabs.Length);

                blueTileCooldown--;
            }
            else
            {
                // Chance to spawn blue tile
                if (Random.value < blueTileChance && firstnobluetile)
                {
                    randIndex = blueTileIndex;
                    blueTileCooldown = blueTileCooldownLength; // reset cooldown
                }
                else
                {
                    randIndex = Random.Range(0, pathPrefabs.Length);
                    while (randIndex == yellowTileIndex || randIndex == blueTileIndex)
                        randIndex = Random.Range(0, pathPrefabs.Length);
                }
            }

            go = Instantiate(pathPrefabs[randIndex]);
            firstnobluetile = true;
        }

        go.transform.SetParent(transform);
        go.transform.position = new Vector3(0, 0, spawnZ);
        spawnZ += pathLength;
        activePaths.Add(go);
         
        if (coinSpawner != null)                                     // for coin
            coinSpawner.SpawnCoinsOnPath(go.transform);     // for coin
    }

    void DeleteOldPath()
    {
        Destroy(activePaths[0]);
        activePaths.RemoveAt(0);
    }


}

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
   
    public int specialIndex1 = 8; // index in pathPrefabs for special path 2
    public int scoreInterval = 500; // spawn every 500 score

    private bool queueSpecial = false; // flag to force next path to be special
    private int lastSpecialScore = -1; // track milestone to avoid repeats
    private int lastSpecialIndex = -1; // remember which special was last

    [Header("References")]
    public Transform Player;
    public Transform spawnStartPoint;
    public Gamemanager gameManager; // Reference to your score script

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
        // Check for score milestone
        int currentScore = Mathf.FloorToInt(gameManager.score);
        //if (currentScore > 0 && currentScore % scoreInterval == 0 && currentScore != lastSpecialScore)
        //{
        //    queueSpecial = true; // force next path to be special
        //    lastSpecialScore = currentScore;
        //}
        if (currentScore == 350 || currentScore == 850 || currentScore == 1300)
        {
            queueSpecial = true; // force next path to be special
           // lastSpecialScore = currentScore;
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
            // Always spawn the single special path
            go = Instantiate(pathPrefabs[specialIndex1]);
            lastSpecialIndex = specialIndex1;

            queueSpecial = false; // reset queue
        }
        else
        {
            // Normal path, excluding special one
            int randIndex = Random.Range(0, pathPrefabs.Length);
            while (randIndex == specialIndex1)
                randIndex = Random.Range(0, pathPrefabs.Length);

            go = Instantiate(pathPrefabs[randIndex]);
        }

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

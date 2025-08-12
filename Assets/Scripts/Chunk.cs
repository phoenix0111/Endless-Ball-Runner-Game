using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Min(0.1f)]
    [SerializeField] private float length = 20.0f;
    
    [SerializeField] private bool shouldGenerateObstacles = false;
    [SerializeField] private bool shouldGenerateCoinsAtStart = false;
    
    [SerializeField] private Vector3 firstElementLocalPosition = Vector3.zero;
    
    [Min(1)]
    [SerializeField] private int rows = 2;
    
    [Min(1)]
    [SerializeField] private int columns = 2;

    [SerializeField] private bool[] activeRows;
    
    [SerializeField] private float offsetX = 1.0f;
    [SerializeField] private float offsetZ = 1.0f;
    
    [Min(0.01f)]
    [SerializeField] private float chunkSpaceCheckRadius = 0.1f;
    [Min(1)]
    [SerializeField] private int obstacleSpawnCount = 5;
    [Min(1)]
    [SerializeField] private int coinsSpawnCount = 5;

    [SerializeField]private List<Obstacle> currentObstacles;
    private List<Coin> currentCoins;

    private List<List<Vector3>> possibleLocalSpawnPos;
    private ObstaclesManager obstaclesManager = null;
    private CoinManager coinManager = null;
    public bool ShouldGenerateObstacles { get => shouldGenerateObstacles; set => shouldGenerateObstacles = value; }
    public float Length { get => length; }

    public void MoveBack(float distance)
    {
        //Move chunk backward first.
        transform.position -= Vector3.forward * distance;

        //Then move all the obstacles back.
        foreach(var obstacle in currentObstacles)
        {
            if(obstacle != null)
            {
                obstacle.transform.position -= Vector3.forward * distance;
            }
        }

        foreach(var coin in currentCoins)
        {
            if (coin != null)
            {
                coin.transform.position -= Vector3.forward * distance;
            }
        }
    }

    private bool CheckIfRowIsEmptyAndGroundIsPresent(int row)
    {
        //Get chunk's layer mask.
        int chunklayerMask = 1 << gameObject.layer;
        for (int y = 0; y < 3; y++)
        {
            Vector3 checkPosition = transform.position + possibleLocalSpawnPos[row][y];
            if (!Physics.CheckSphere(checkPosition, chunkSpaceCheckRadius, chunklayerMask) ||
                obstaclesManager.IsObstacleThere(checkPosition))
            {
                return false;
            }
        }

        return activeRows[row] == true;
    }
    public IEnumerator GenerateObstacles()
    {
        if (!ShouldGenerateObstacles)
        {
            yield break;
        }

        int tries = 10;
        while (tries > 0 && !ServiceLocator.ForSceneOf(this).TryGetService<ObstaclesManager>(out obstaclesManager))
        {
            tries--;
            yield return null;
        }

        if (obstaclesManager == null)
        {
            yield break;
        }
        
        currentObstacles.Clear();
        
        for(int k = 0; k < obstacleSpawnCount; k++)
        {
            yield return null;
            GenerateRandomObstacles();
            yield return null;
        }
    }

    public IEnumerator GenerateCoins()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        int tries = 50;
        while (
               !ServiceLocator.ForSceneOf(this).TryGetService<ObstaclesManager>(out obstaclesManager) ||
               !ServiceLocator.ForSceneOf(this).TryGetService<CoinManager>(out coinManager))
        {
            tries--;
            yield return null;
        }

        if (coinManager == null || obstaclesManager == null)
        {
            Debug.Log("Either coin manager or obstacles manager is null");
            yield break;
        }

        currentCoins.Clear();
        //Get chunk's layer mask.
        int chunklayerMask = 1 << gameObject.layer;

        for (int i = 0; i < coinsSpawnCount; i++)
        {
            yield return null;

            //Select random row and column.
            int randomRow = Random.Range(1, possibleLocalSpawnPos.Count - 1);
            int randomColumn = Random.Range(0, 3);

            Vector3 position = transform.position + possibleLocalSpawnPos[randomRow][randomColumn];
            if(!Physics.CheckSphere(position, chunkSpaceCheckRadius, chunklayerMask) || 
                coinManager.IsCoinThere(position, 2.0f))
            {
                yield return null;
                continue;
            }
            if (obstaclesManager.IsObstacleThere(position, 0.5f, out Obstacle obstacle) && obstacle != null)
            {
                position += Vector3.up * obstacle.Height;
            }

            var coin = coinManager.Spawn(position, Quaternion.identity);
            currentCoins.Add(coin);
        }
    }

    private void GenerateRandomObstacles()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        //Select the random lane.
        LaneType randomLaneType = Utility.GetRandomEnum<LaneType>(1, 4);

        //Select the random obstacle type.
        ObstacleType obstacleType = Utility.GetRandomEnum<ObstacleType>(1, 6);

        //Debug.Log("Random lane: " + randomLaneType.ToString() + ", Obstacle Type: " + obstacleType.ToString());
        
        //Select random row and column.
        int randomRow = Random.Range(1, possibleLocalSpawnPos.Count - 1);
        int randomColumn = Random.Range(0, 3);

        //Check if chunk ground exists on this row or not. If not, skip it.
        //Also, check for obstacle here.
        if (!CheckIfRowIsEmptyAndGroundIsPresent(randomRow))
        {
            return;
        }

        
        //Now, spawn the obstacles based on random lane type.
        switch (randomLaneType)
        {
            case LaneType.Single:
                if(obstacleType == ObstacleType.MovingPlatform)
                {
                    randomColumn = 1; 
                }
                else
                {
                    while (obstacleType == ObstacleType.BigStone)
                    {
                        obstacleType = Utility.GetRandomEnum<ObstacleType>(1, 6);
                    }
                }

                var obstacle = obstaclesManager.Spawn(transform.position + possibleLocalSpawnPos[randomRow][randomColumn],
                                       Quaternion.identity,
                                       obstacleType);

                currentObstacles.Add(obstacle);
                break;
            case LaneType.Double:
                while (obstacleType == ObstacleType.MovingPlatform)
                {
                    obstacleType = Utility.GetRandomEnum<ObstacleType>(1, 6);
                }
                GenerateObstaclesInDoubleLane(randomRow,
                                              randomColumn,
                                              obstacleType,
                                              obstaclesManager);
                break;
            case LaneType.Triple:
                while (obstacleType == ObstacleType.MovingPlatform)
                {
                    obstacleType = Utility.GetRandomEnum<ObstacleType>(1, 6);
                }
                var obstacle1 = obstaclesManager.Spawn(transform.position + possibleLocalSpawnPos[randomRow][0],
                                       Quaternion.identity,
                                       obstacleType);

                var obstacle2 = obstaclesManager.Spawn(transform.position + possibleLocalSpawnPos[randomRow][1],
                                       Quaternion.identity,
                                       obstacleType);

                var obstacle3 = obstaclesManager.Spawn(transform.position + possibleLocalSpawnPos[randomRow][2],
                                       Quaternion.identity,
                                       obstacleType);

                currentObstacles.Add(obstacle1);
                currentObstacles.Add(obstacle2);
                currentObstacles.Add(obstacle3);
                break;
        }
    }
    private void GenerateObstaclesInDoubleLane(int row, 
                                               int unselectedColumn, 
                                               ObstacleType type, 
                                               ObstaclesManager obstaclesManager)
    {
        
        int column1 = unselectedColumn;
        int column2 = unselectedColumn;

        //If it is big stone, then spawn them at 1st and 3rd position.
        if(type == ObstacleType.BigStone)
        {
            column1 = 0;
            column2 = 2;
        }

        //Else, you can spawn them like this
        else
        {
            column1 = Random.Range(0, 3);
            while (column1 == unselectedColumn)
            {
                column1 = Random.Range(0, 3);
            }

            if (unselectedColumn == 0)
            {
                if (column1 == 1)
                {
                    column2 = 2;
                }
                else if (column1 == 2)
                {
                    column2 = 1;
                }
            }
            else if (unselectedColumn == 1)
            {
                if (column1 == 1)
                {
                    column2 = 2;
                }
                else if (column1 == 2)
                {
                    column2 = 0;
                }
            }
            else if (unselectedColumn == 2)
            {
                if (column1 == 0)
                {
                    column2 = 1;
                }
                else if (column1 == 1)
                {
                    column2 = 0;
                }
            }
        }
        
        var obstacle1 = obstaclesManager.Spawn(transform.position + possibleLocalSpawnPos[row][column1],
                                   Quaternion.identity,
                                   type);

        var obstacle2 = obstaclesManager.Spawn(transform.position + possibleLocalSpawnPos[row][column2],
                               Quaternion.identity,
                               type);
        
        currentObstacles.Add(obstacle1);
        currentObstacles.Add(obstacle2);
    }

    
    private void UnspawnObstacles()
    {
        foreach (var obstacle in currentObstacles)
        {
            if(obstaclesManager != null)
            {
                obstaclesManager.Unspawn(obstacle);
            }
            else
            {
                if(obstacle != null)
                {
                    Destroy(obstacle.gameObject);
                }
            }
        }

        currentObstacles.Clear();
    }

    private void UnspawnCoins()
    {
        foreach (var coin in currentCoins)
        {
            if (coinManager != null)
            {
                coinManager.Unspawn(coin);
            }
            else
            {
                if (coin != null)
                {
                    Destroy(coin.gameObject);
                }
            }
        }

        currentCoins.Clear();
    }


    private void CalculateAllObstacleSpawnPos()
    {
        possibleLocalSpawnPos.Clear();

        //Calculate world position of first element position based on specified local position.
        Vector3 cornerWorldPosition = firstElementLocalPosition;

        for (int i = 0; i < rows; i++)
        {
            List<Vector3> row = new List<Vector3>();
            for(int j = 0; j < columns; j++) 
            {
                //Calculate each element's position.
                Vector3 currentPosition = cornerWorldPosition + new Vector3(j * offsetX, 0.0f, i * offsetZ);
                
                //Add it in a row list
                row.Add(currentPosition);
            }

            //Add row in obstacle local spawn pos.
            possibleLocalSpawnPos.Add(row);
        }
    }

    private void OnEnable()
    {
        CalculateAllObstacleSpawnPos();
    }

    private void OnValidate()
    {
        if(activeRows == null || activeRows.Length != rows)
        {
            activeRows = new bool[rows];
        }
    }

    private void OnDisable()
    {
        UnspawnObstacles();
        UnspawnCoins();
    }

    private void Awake()
    {
        currentObstacles = new List<Obstacle>();
        currentCoins = new List<Coin>();
        possibleLocalSpawnPos = new List<List<Vector3>>();
    }

    private void Start()
    {
        if(shouldGenerateCoinsAtStart)
        {
            StartCoroutine(GenerateCoins());
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Calculate world position of first element position based on specified local position.
        Vector3 cornerWorldPosition = transform.position + firstElementLocalPosition;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(cornerWorldPosition, 1.0f);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                //Calculate each element's position.
                Vector3 currentPosition = cornerWorldPosition + new Vector3(j * offsetX, 0.0f, i * offsetZ);

                //Show spawnable position.
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(currentPosition, chunkSpaceCheckRadius);
            }
        }

        //Show length of chunk.
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * length);
    }
}
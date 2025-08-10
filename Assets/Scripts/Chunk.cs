using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Min(0.1f)]
    [SerializeField] private float length = 20.0f;
    
    [SerializeField] private bool shouldGenerateObstacles = false;
    
    [Range(0.0f, 1.0f)]
    [SerializeField] private float generateObstacleProbability = 0.2f;
    
    [SerializeField] private Vector3 firstElementLocalPosition = Vector3.zero;
    
    [Min(1)]
    [SerializeField] private int rows = 2;
    
    [Min(1)]
    [SerializeField] private int columns = 2;
    
    [SerializeField] private float offsetX = 1.0f;
    [SerializeField] private float offsetZ = 1.0f;
    
    [Min(0.01f)]
    [SerializeField] private float chunkSpaceCheckRadius = 0.1f;
    [Min(1)]
    [SerializeField] private int spawnCount = 5;

    private List<Obstacle> currentObstacles;
    private List<List<Vector3>> obstacleLocalSpawnPos;
    private ObstaclesManager obstaclesManager = null;
    public bool ShouldGenerateObstacles { get => shouldGenerateObstacles; set => shouldGenerateObstacles = value; }
    public float Length { get => length; }

    public void MoveBack(float distance)
    {
        //Move chunk backward first.
        transform.position -= Vector3.forward * distance;

        //Then move all the obstacles back.
        foreach(var obstacle in currentObstacles)
        {
            obstacle.transform.position -= Vector3.forward * distance;
        }
    }

    private bool CheckIfRowIsEmptyAndGroundIsPresent(int row)
    {
        //Get chunk's layer mask.
        int chunklayerMask = 1 << gameObject.layer;
        for (int y = 0; y < 3; y++)
        {
            Vector3 checkPosition = transform.position + obstacleLocalSpawnPos[row][y];
            if (!Physics.CheckSphere(checkPosition, chunkSpaceCheckRadius, chunklayerMask) ||
                obstaclesManager.IsObstacleThere(checkPosition))
            {
                return false;
            }
        }

        return true;
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
        
        for(int k = 0; k < spawnCount; k++)
        {
            yield return null;
            GenerateRandomObstacles();
            yield return null;
        }
    }

    private void GenerateRandomObstacles()
    {
        //Select the random lane.
        LaneType randomLaneType = Utility.GetRandomEnum<LaneType>(1, 4);

        //Select the random obstacle type.
        ObstacleType obstacleType = ObstacleType.Block; //Utility.GetRandomEnum<ObstacleType>(1, 4);

        //Debug.Log("Random lane: " + randomLaneType.ToString() + ", Obstacle Type: " + obstacleType.ToString());
        
        //Select random row and column.
        int randomRow = Random.Range(1, obstacleLocalSpawnPos.Count - 1);
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
                var obstacle = obstaclesManager.Spawn(transform.position + obstacleLocalSpawnPos[randomRow][randomColumn],
                                       Quaternion.identity,
                                       obstacleType,
                                       Utility.GetRandomEnum<LaneHeightType>(1, 3));

                currentObstacles.Add(obstacle);
                break;
            case LaneType.Double:
                GenerateObstaclesInDoubleLane(randomRow,
                                              randomColumn,
                                              obstacleType,
                                              obstaclesManager);
                break;
            case LaneType.Triple:
                var obstacle1 = obstaclesManager.Spawn(transform.position + obstacleLocalSpawnPos[randomRow][0],
                                       Quaternion.identity,
                                       obstacleType,
                                       Utility.GetRandomEnum<LaneHeightType>(1, 3));

                var obstacle2 = obstaclesManager.Spawn(transform.position + obstacleLocalSpawnPos[randomRow][1],
                                       Quaternion.identity,
                                       obstacleType,
                                       Utility.GetRandomEnum<LaneHeightType>(1, 3));

                var obstacle3 = obstaclesManager.Spawn(transform.position + obstacleLocalSpawnPos[randomRow][2],
                                       Quaternion.identity,
                                       obstacleType,
                                       Utility.GetRandomEnum<LaneHeightType>(1, 3));

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
        //Select the first column based on specified column.
        int column1 = unselectedColumn;

        column1 = Random.Range(0, 3);
        while (column1 == unselectedColumn)
        {
            column1 = Random.Range(0, 3);
        }

        //Now, select the second column based on specified column and column 1.
        int column2 = unselectedColumn;

        if(unselectedColumn == 0)
        {
            if(column1 == 1)
            {
                column2 = 2;
            }
            else if(column1 == 2)
            {
                column2 = 1;
            }
        }
        else if(unselectedColumn == 1)
        {
            if(column1 == 1)
            {
               column2 = 2;
            }
            else if(column1 == 2)
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

        var obstacle1 = obstaclesManager.Spawn(transform.position + obstacleLocalSpawnPos[row][column1],
                               Quaternion.identity,
                               type,
                               Utility.GetRandomEnum<LaneHeightType>(1, 3));

        var obstacle2 = obstaclesManager.Spawn(transform.position + obstacleLocalSpawnPos[row][column2],
                               Quaternion.identity,
                               type,
                               Utility.GetRandomEnum<LaneHeightType>(1, 3));
        
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
    
    private void CalculateAllObstacleSpawnPos()
    {
        obstacleLocalSpawnPos.Clear();

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
            obstacleLocalSpawnPos.Add(row);
        }
    }

    private void OnEnable()
    {
        CalculateAllObstacleSpawnPos();
    }

    private void OnDisable()
    {
        UnspawnObstacles();
    }

    private void Awake()
    {
        currentObstacles = new List<Obstacle>();
        obstacleLocalSpawnPos = new List<List<Vector3>>();
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
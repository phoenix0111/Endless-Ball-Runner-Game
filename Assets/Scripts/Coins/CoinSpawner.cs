using UnityEngine;
using System.Collections.Generic;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] Transform[] SpawnStraightPoints;
    [SerializeField] Transform[] CurveSpawnPoint;
    
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        
        for (int i = 0; i < SpawnStraightPoints.Length; i++)
        {
            SpawnStraightPoints[i].transform.position = new Vector3(Random.Range(-2,2), SpawnStraightPoints[i].transform.position.y, SpawnStraightPoints[i].transform.position.z);
        }

        


        SpawnStraightCoins();
        SpawnCurvedCoins();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnStraightCoins()
    {
        if (SpawnStraightPoints.Length < 2) return; // need at least 2 spawn points

        // pick first random point
        Transform firstPoint = SpawnStraightPoints[Random.Range(0, SpawnStraightPoints.Length)];
        CoinsObjectPool.Instance.SpawnStraightCoinsOnPath(firstPoint);

        

        
    }

    void SpawnCurvedCoins()
    {
        if (CurveSpawnPoint.Length == 0) return;
        
        Transform CurvePoint = CurveSpawnPoint[Random.Range(0, CurveSpawnPoint.Length)];
        CoinsObjectPool.Instance.SpawnCurvedCoinsOnPath(CurvePoint);
    }
}

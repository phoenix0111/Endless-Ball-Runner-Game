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

        // pick second random point different from first
        Transform secondPoint;
        do
        {
            secondPoint = SpawnStraightPoints[Random.Range(0, SpawnStraightPoints.Length)];
        } while (secondPoint == firstPoint);

        CoinsObjectPool.Instance.SpawnStraightCoinsOnPath(secondPoint);
    }

    void SpawnCurvedCoins()
    {
        // pick first random point
        Transform CurvePoint = CurveSpawnPoint[Random.Range(0, CurveSpawnPoint.Length)];
        CoinsObjectPool.Instance.SpawnCurvedCoinsOnPath(CurvePoint);
    }
}

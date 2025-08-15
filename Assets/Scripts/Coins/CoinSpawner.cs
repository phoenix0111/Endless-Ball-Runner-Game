using UnityEngine;
using System.Collections.Generic;


public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public int poolSize = 30;
    public CoinPattern[] patterns;

    private Queue<GameObject> coinPool = new Queue<GameObject>();

    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject coin = Instantiate(coinPrefab);
            coin.SetActive(false);
            coinPool.Enqueue(coin);
        }
    }

    public void SpawnCoinsOnPath(Transform path)
    {
        // Pick a random pattern
        if (patterns.Length == 0) return;
        CoinPattern pattern = patterns[Random.Range(0, patterns.Length)];

        foreach (Vector3 localPos in pattern.positions)
        {
            GameObject coin = GetCoinFromPool();
            coin.transform.SetParent(path);
            coin.transform.localPosition = localPos;
            coin.transform.localRotation = Quaternion.identity;
            
            coin.SetActive(true);
        }
       
    }

    private GameObject GetCoinFromPool()
    {
        if (coinPool.Count > 0)
        {
            GameObject coin = coinPool.Dequeue();
            return coin;
        }
        else
        {
            // if pool is empty, create new
            return Instantiate(coinPrefab);
        }
    }

    public void ReturnCoin(GameObject coin)
    {
        coin.SetActive(false);
        coinPool.Enqueue(coin);
    }
}

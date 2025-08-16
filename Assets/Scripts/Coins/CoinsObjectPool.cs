using UnityEngine;
using System.Collections.Generic;

public class CoinsObjectPool : MonoBehaviour
{
    public static CoinsObjectPool Instance { get; private set; }  // Singleton instance

    [Header("Pool Settings")]
    public GameObject coinPrefab;
    public int poolSize = 30;
    public CoinPattern[] patterns;

    private Queue<GameObject> coinPool = new Queue<GameObject>();

    private void Awake()
    {
        // --- Singleton Setup ---
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // kill duplicate
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // optional, keep across scenes

        // --- Initialize Pool ---
        for (int i = 0; i < poolSize; i++)
        {
            GameObject coin = Instantiate(coinPrefab);
            coin.SetActive(false);
            coinPool.Enqueue(coin);
        }
    }

    public void SpawnStraightCoinsOnPath(Transform path)
    {
        if (patterns.Length == 0) return;
        CoinPattern pattern = patterns[1]; // straight

        foreach (Vector3 localPos in pattern.positions)
        {
            GameObject coin = GetCoinFromPool();
            coin.transform.SetParent(path);
            coin.transform.localPosition = localPos;
            coin.transform.localRotation = Quaternion.identity;
            coin.SetActive(true);
        }
    }

    public void SpawnCurvedCoinsOnPath(Transform path)
    {
        if (patterns.Length == 0) return;
        CoinPattern pattern = patterns[0]; // curved

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
            return coinPool.Dequeue();
        }
        else
        {
            return Instantiate(coinPrefab);
        }
    }

    public void ReturnCoin(GameObject coin)
    {
        coin.SetActive(false);
        coinPool.Enqueue(coin);
    }
}

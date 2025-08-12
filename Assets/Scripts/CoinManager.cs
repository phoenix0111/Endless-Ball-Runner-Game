using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private Coin[] coinPrefabs;
    [Min(50)]
    [SerializeField] private int maxCoinsCount = 100;
    [SerializeField] private LayerMask coinLayerMask;
    private ObjectPool<Coin> coinsPool;
    
    public Coin Spawn(Vector3 spawnPosition, Quaternion rotation)
    {
        var coin = coinsPool.Get();
        if(coin != null)
        {
            coin.transform.position = spawnPosition;
            coin.transform.rotation = rotation;
        }

        return coin;
    }

    public void Unspawn(Coin coin)
    {
        coinsPool.ReturnToPool(coin);
    }

    public bool IsCoinThere(Vector3 position, float checkRadius)
    {
        return Physics.CheckSphere(position, checkRadius, coinLayerMask.value);
    }

    private Coin CreateCoin()
    {
        var instance = Instantiate(coinPrefabs[Random.Range(0, coinPrefabs.Length)], Vector3.zero, Quaternion.identity);
        instance.transform.parent = transform;
        instance.gameObject.SetActive(false);
        return instance;
    }

    private void OnGetCoin(Coin coin)
    {
        coin.gameObject.SetActive(true);
    }

    private void OnReturnCoin(Coin coin)
    {
        coin.gameObject.SetActive(false);
    }

    private void DestroyCoin(Coin coin)
    {
        Destroy(coin.gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ServiceLocator.ForSceneOf(this).Register<CoinManager>(this);
        if (coinsPool == null)
        {
            coinsPool = new ObjectPool<Coin>(CreateCoin,
                                             OnGetCoin,
                                             OnReturnCoin,
                                             DestroyCoin,
                                             maxCoinsCount);
        }
    }
}

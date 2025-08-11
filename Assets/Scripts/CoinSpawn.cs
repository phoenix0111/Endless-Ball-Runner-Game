using UnityEngine;

public class CoinSpawn : MonoBehaviour
{
    public GameObject coinPrefab; // Your coin prefab
    public Transform spawnParent; // Parent to keep hierarchy clean
    public float coinSpacing = 1.0f; // Distance between coins in a pattern
    [SerializeField] GameObject playerpos;

    private int  bunchnumer;

    private void Start()
    {
        bunchnumer = Random.Range(2, 6);
        spawncoins();
      
    }

    private void Update()
    {
       
    }

    public void spawncoins()
    {
        for (int i = 0; i < bunchnumer; i++)
        {
            Instantiate(coinPrefab, playerpos.transform.position + new Vector3(0, 0, 30)+ new Vector3(0, 0, i * coinSpacing), Quaternion.identity, spawnParent);
        }
       
    }

}
   
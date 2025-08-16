using UnityEngine;

public class CoinCleaning
    : MonoBehaviour
{
    [SerializeField] private Transform player;   // assign Player in Inspector
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f); // stays 10 units behind

    private void LateUpdate()
    {
        if (player != null)
        {
            // Follow player with offset
            transform.Translate( player.position + offset);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            // Just return coin to pool, no score
            CoinsObjectPool.Instance.ReturnCoin(other.gameObject);
            Debug.Log("fere");
        }
    }
}

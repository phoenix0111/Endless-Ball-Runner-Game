
using UnityEngine;

public class CoinCleaning : MonoBehaviour
{
    public Transform player;      // drag your Player here
    public Vector3 offset = new Vector3(0, 0, -10); // set offset in Inspector
    public float followSpeed = 5f; // smoothness of follow

    void Update()
    {
        // target position (player + offset)
        Vector3 targetPosition = player.position + offset;

        // move follower towards target using Translate
        Vector3 direction = targetPosition - transform.position;
        transform.Translate(direction * followSpeed * Time.deltaTime, Space.World);
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

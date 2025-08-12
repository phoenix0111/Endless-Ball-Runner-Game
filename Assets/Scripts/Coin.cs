using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayerMask;

    private void OnCollisionEnter(Collision collision)
    {
        int layerMask = 1 << collision.gameObject.layer;

        if((playerLayerMask.value & layerMask) != 0)
        {
            if(ServiceLocator.ForSceneOf(this).
               TryGetService<GameManager>(out GameManager gameManager))
            {
                gameManager.CoinUpdate();
            }
        }

        
        if(ServiceLocator.ForSceneOf(this).
        TryGetService<CoinManager>(out CoinManager coinManager))
        {
            coinManager.Unspawn(this);
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up, 100 * Time.deltaTime, Space.World);
    }
    
}

using UnityEngine;

public class GameOverDecider : MonoBehaviour
{
    [SerializeField] private LayerMask gameOverLayerMask;

    private void OnTriggerEnter(Collider other)
    {
        int layerMask = 1 << other.gameObject.layer;

        if((layerMask & gameOverLayerMask) != 0)
        {
            ServiceLocator.ForSceneOf(this).TryGetService<GameManager>(out GameManager gameManager);
            gameManager.OnGameOver?.Invoke();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        int layerMask = 1 << other.gameObject.layer;

        if ((layerMask & gameOverLayerMask) != 0)
        {
            ServiceLocator.ForSceneOf(this).TryGetService<GameManager>(out GameManager gameManager);
            gameManager.OnGameOver?.Invoke();
        }
    }
}

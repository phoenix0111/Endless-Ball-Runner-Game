using UnityEngine;

public class GameOverDecider : MonoBehaviour
{
    [SerializeField] private LayerMask gameOverLayerMask;
    [SerializeField] private float fallHeight = 60.0f;
    private bool hasFallenToDeath = false;
    private void Update()
    {
        float currentY = transform.position.y;
        if(!hasFallenToDeath && currentY < -fallHeight)
        {
           ServiceLocator.ForSceneOf(this).TryGetService<GameManager>(out GameManager gameManager);
            gameManager.OnGameOver?.Invoke();
            gameObject.SetActive(false);
            hasFallenToDeath = true;
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        int layerMask = 1 << other.gameObject.layer;

        if((layerMask & gameOverLayerMask) != 0)
        {
            ServiceLocator.ForSceneOf(this).TryGetService<GameManager>(out GameManager gameManager);
            gameManager.OnGameOver?.Invoke();
            gameObject.SetActive(false);
            Destroy(gameObject); // Optionally destroy the object that triggered the game over
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        int layerMask = 1 << other.gameObject.layer;

        if ((layerMask & gameOverLayerMask) != 0)
        {
            ServiceLocator.ForSceneOf(this).TryGetService<GameManager>(out GameManager gameManager);
            gameManager.OnGameOver?.Invoke();
            Destroy(gameObject);
        }
    }
}

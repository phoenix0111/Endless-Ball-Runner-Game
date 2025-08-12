using UnityEngine;

public class GameOverDecider : MonoBehaviour
{
    [SerializeField] private LayerMask gameOverLayerMask;
    [SerializeField] private float fallHeight = 60.0f;
    private bool hasFallenToDeath = false;

    public void ResetGameOver()
    {
        hasFallenToDeath = false;
    }

    private void Update()
    {
        float currentY = transform.position.y;
        if(!hasFallenToDeath && currentY < -fallHeight)
        {
           ServiceLocator.ForSceneOf(this).TryGetService<GameManager>(out GameManager gameManager);
            gameManager.OnGameOver?.Invoke();
            gameObject.SetActive(false);
            hasFallenToDeath = true;
            Debug.Log("Death due to falling");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        int layerMask = 1 << other.gameObject.layer;

        if((layerMask & gameOverLayerMask.value) != 0)
        {
            ServiceLocator.ForSceneOf(this).TryGetService<GameManager>(out GameManager gameManager);
            gameManager.OnGameOver?.Invoke();
            //gameObject.SetActive(false);
            Debug.Log("Game over due to trigger");
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        int layerMask = 1 << other.gameObject.layer;

        if ((layerMask & gameOverLayerMask.value) != 0)
        {
            ServiceLocator.ForSceneOf(this).TryGetService<GameManager>(out GameManager gameManager);
            gameManager.OnGameOver?.Invoke();
            Debug.Log("Game over due to collision with " + other.gameObject.name);
        }
    }
}

using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("Movement")]
    public SwipeController swipeController;
    public ballMove player;

    [Header("UI Manager")]
    [SerializeField] UIManager uiManager;
    public float score = 0f;
    float Fscore = 0f;
    [SerializeField] float scoreIncrementRate = 10f; // Points per second

    public UnityEvent OnGameOver;

    private bool isGameOver = false;

    public bool IsGameOver { get => isGameOver; }

    private void Start()
    {
        swipeController = GetComponent<SwipeController>();

        ServiceLocator.ForSceneOf(this).Register<GameManager>(this);
        swipeController.OnSwipeLeft += player.MoveLeft;
        swipeController.OnSwipeRight += player.MoveRight;
        OnGameOver.AddListener(SetGameOverStateToTrue);
        OnGameOver.AddListener(uiManager.ShowGameOverScreen);
    }

    void Update()
    {
        ScoreLogic();
    }

    void ScoreLogic()
    {
        if (isGameOver)
        {
            return;
        }

        float deltaTime = Time.deltaTime;

        Fscore += deltaTime * scoreIncrementRate; // Increment score by  points per second
        score = Mathf.FloorToInt(Fscore);

        uiManager.scoreText.text = "Score: " + score.ToString("F0"); // Update the score text in UI
    }

    void SetGameOverStateToTrue()
    {
        isGameOver = true;
    }
}

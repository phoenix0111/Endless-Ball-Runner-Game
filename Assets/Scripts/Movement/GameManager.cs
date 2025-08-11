using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("Movement")]
     [SerializeField] SwipeController swipeController;
     [SerializeField] ballMove player;


    [Header("UI Manager")]
    [SerializeField] UIManager uiManager;
    public float score = 0f;
    private float Fscore = 0f;
    [SerializeField] float scoreIncrementRate = 10f; // Points per second
    public int coins = 0; 

    [Header("Difficulty Settings")]
    [SerializeField] int nextScoreThreshold = 100; // Initial threshold for difficulty increase    
    [SerializeField] int SpeedIncreaseRate = 1; // How much to increase speed each time

    [Header("Game Restart")]
    public Vector3 Restartpos;
    [SerializeField] GameObject playerPrefab;

    public UnityEvent OnGameOver;

    private bool isGameOver = false;

    public bool IsGameOver { get => isGameOver; }

    private void Start()
    {
        swipeController = GetComponent<SwipeController>();

        ServiceLocator.ForSceneOf(this).Register<GameManager>(this);
        if (player != null)
        {
            swipeController.OnSwipeLeft += player.MoveLeft;
            swipeController.OnSwipeRight += player.MoveRight;
        }
       
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

    public void CoinUpdate()
    {
        coins++; // Increment coins by 1
        uiManager.coinsText.text = "Coins: " + coins.ToString(); // Update the coins text in UI
    }

    public void RestartGame()
    {
       GameObject ballplayer = Instantiate(playerPrefab, Restartpos, Quaternion.identity); // Instantiate the player at the restart position
        ballplayer.SetActive(true);
        uiManager.Allpaneldisable(); // Disable all panels
        player = ballplayer.GetComponent<ballMove>(); // Get the player component from the instantiated player 
        swipeController = GetComponent<SwipeController>();
        swipeController.OnSwipeLeft += player.MoveLeft;
        swipeController.OnSwipeRight += player.MoveRight;
        isGameOver = false;
        ServiceLocator.ForSceneOf(this).TryGetService < PathSpawner >(out PathSpawner pathspawner);
        pathspawner.player = ballplayer.transform; // Set the player transform in PathSpawner

    }
}

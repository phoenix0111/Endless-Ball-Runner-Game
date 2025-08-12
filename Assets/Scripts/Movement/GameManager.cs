using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    [SerializeField] GameObject BallPlayer;

    public UnityEvent OnGameOver;

    private bool isGameOver = false;

    public bool IsGameOver { get => isGameOver; }

    private void Start()
    {
        ServiceLocator.ForSceneOf(this).Register<GameManager>(this);

        swipeController = GetComponent<SwipeController>();
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

    public void CoinUpdate()
    {
        coins++; // Increment coins by 1
        uiManager.coinsText.text = "Coins: " + coins.ToString(); // Update the coins text in UI
    }

    public void RestartGame()
    {
        BallPlayer.transform.position = Restartpos; // Reset player position
        BallPlayer.SetActive(true);
        BallPlayer.GetComponent<GameOverDecider>().ResetGameOver();
        uiManager.Allpaneldisable(); // Disable all panels
        uiManager.scoreText.gameObject.SetActive(true);
        isGameOver = false;
        coins = coins - uiManager.coinsneededtoreespawn; // Deduct coins needed to respawn
        uiManager.coinsneededtoreespawn += 10;

    }

   
       

}

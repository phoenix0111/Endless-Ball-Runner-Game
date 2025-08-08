using UnityEngine;

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


    private void Start()
    {
        swipeController = GetComponent<SwipeController>();

        swipeController.OnSwipeLeft += player.MoveLeft;
        swipeController.OnSwipeRight += player.MoveRight;
    }

    void Update()
    {
        ScoreLogic();


        if (score >= nextScoreThreshold)
        {
            increaseDifficulty();
            nextScoreThreshold += 100; // Set next threshold (200, 300, etc.)
        }

    }



    void ScoreLogic()
    {
        float deltaTime = Time.deltaTime;

        Fscore += deltaTime * scoreIncrementRate; // Increment score by  points per second
        score = Mathf.FloorToInt(Fscore);



        uiManager.scoreText.text = "Score: " + score.ToString("F0"); // Update the score text in UI
    }

    void increaseDifficulty()
    {
        player.baseSpeed += SpeedIncreaseRate; // Increase player speed
        player.boostSpeed += SpeedIncreaseRate; // Increase boost speed
    }

    public void CoinUpdate()
    {
          coins++; // Increment coins by 1
        uiManager.coinsText.text = "Coins: " + coins.ToString(); // Update the coins text in UI
    }
}

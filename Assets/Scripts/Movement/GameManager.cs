using UnityEngine;

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

    private void Start()
    {
        swipeController = GetComponent<SwipeController>();

        swipeController.OnSwipeLeft += player.MoveLeft;
        swipeController.OnSwipeRight += player.MoveRight;
    }

    void Update()
    {
        ScoreLogic();




    }




    void ScoreLogic()
    {
        float deltaTime = Time.deltaTime;

        Fscore += deltaTime * scoreIncrementRate; // Increment score by  points per second
        score = Mathf.FloorToInt(Fscore);



        uiManager.scoreText.text = "Score: " + score.ToString("F0"); // Update the score text in UI
    }
}

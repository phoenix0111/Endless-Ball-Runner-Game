using UnityEngine;
using UnityEngine.Splines.Interpolators;

public class Gamemanager : MonoBehaviour
{
    [Header("Essentials")]
    [SerializeField] uiManager uiManager; 
    [SerializeField] MovementPlayer Player;
    [SerializeField] Camera Camera;
   
    [Header("Score")]
    public int score = 0;
    private float Fscore = 0f;
    [SerializeField] float scoreMultiplier = 1f;

    [Header("DifficultySettings")]
    [SerializeField] int ScoreThreshold = 100;
    [SerializeField] float ScoreMultiplyerRate = 1.2f;
    [SerializeField] float ScoreCap = 50;

    [Header("Camera Settings")]
    [SerializeField] int FOVIncreaser = 5;
    private int cameraFOVvalue;
    void Start()
    {
        
    }

   
    void Update()
    {
        ScoreCalculate();


        if (Player.forwardSpeed <= ScoreCap)
        {
            if (score >= ScoreThreshold)
            {
                increaseDifficulty();
                ScoreThreshold += 100; // Set next threshold (200, 300, etc.)
                cameraFOVvalue = ((int)Camera.fieldOfView)+ FOVIncreaser ;
                Camera.fieldOfView = Mathf.Lerp(Camera.fieldOfView, cameraFOVvalue, 2);
             }

        }

         
    }

    void ScoreCalculate()
    {
        float t = Time.deltaTime;
        Fscore += t* scoreMultiplier; // Increment score by  points per second
        score = Mathf.FloorToInt(Fscore);
   
        uiManager.scoreText.text = "Score: " + score.ToString();
    }

    void increaseDifficulty()
    {
        {
            Player.forwardSpeed *= ScoreMultiplyerRate; // Increase player's base speed
            Player.sidewaysSpeed *= ScoreMultiplyerRate;
        }



    }
}

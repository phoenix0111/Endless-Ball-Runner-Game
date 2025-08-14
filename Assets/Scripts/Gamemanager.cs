using UnityEngine;

public class Gamemanager : MonoBehaviour
{
    [Header("Essentials")]
    [SerializeField] uiManager uiManager; 
    [SerializeField] MovementPlayer Player;
   
    [Header("Score")]
    public int score = 0;
    private float Fscore = 0f;
    [SerializeField] float scoreMultiplier = 1f;

    void Start()
    {
        
    }

   
    void Update()
    {
        ScoreCalculate();
    }

    void ScoreCalculate()
    {
        float t = Time.deltaTime;
        Fscore += t* scoreMultiplier; // Increment score by  points per second
        score = Mathf.FloorToInt(Fscore);
   
        uiManager.scoreText.text = "Score: " + score.ToString();
    }
}

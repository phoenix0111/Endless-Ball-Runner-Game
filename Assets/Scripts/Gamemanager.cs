using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines.Interpolators;


public class Gamemanager : MonoBehaviour
{
    [Header("Essentials")]
    [SerializeField] uiManager uiManager; 
    [SerializeField] MovementPlayer Player;
    
    [SerializeField] CinemachineCamera cinecamera;
    public GameObject player;
    [SerializeField] GameObject SpeedLines;

   
    [Header("Score")]
    public int score = 0;
    private float Fscore = 0f;
    [SerializeField] float scoreMultiplier = 1f;

    [Header("Coins")]
    public int CoinCount = 0;

    [Header("DifficultySettings")]
    [SerializeField] int ScoreThreshold = 100;
    [SerializeField] float ScoreMultiplyerRate = 1.2f;
    [SerializeField] float ScoreCap = 50;

    [Header("Camera Settings")]
    [SerializeField] int FOVIncreaser = 5;
    private int cameraFOVvalue = 40;
    [SerializeField] float fovSmoothSpeed = 5;

    [Header("Respawning")]
    public Vector3 RespawnPos;
    private float currentspeed;
    private float currentsidespeed;





    void Start()
    {
        PlayerPrefs.SetInt("GameCoins", 0);
        PlayerPrefs.Save();
    }

   
    void Update()
    {
        ScoreCalculate();

        if (score >= 500) SpeedLines.SetActive(true);


        if (Player.forwardSpeed <= ScoreCap)
        {
            if (score >= ScoreThreshold)
            {
                increaseDifficulty();
                ScoreThreshold += 100; // Set next threshold (200, 300, etc.)
                cameraFOVvalue = ((int)cinecamera.Lens.FieldOfView) + FOVIncreaser ;
               



             }

        }
        cinecamera.Lens.FieldOfView = Mathf.Lerp(cinecamera.Lens.FieldOfView, cameraFOVvalue, Time.deltaTime * fovSmoothSpeed);

        currentspeed = Player.forwardSpeed;
      
        currentsidespeed = Player.sidewaysSpeed;
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
            currentspeed = Player.forwardSpeed;
            Player.sidewaysSpeed *= ScoreMultiplyerRate;
            currentsidespeed = Player.sidewaysSpeed;
        }



    }

    public  void IncreaseCoinCount()
    {
       
        CoinCount++;
        uiManager.CoinText.text = "Coins: " + CoinCount.ToString();
        PlayerPrefs.SetInt("GameCoins", CoinCount);
        PlayerPrefs.Save();
    }

    public void RespawnGame()
    {
       player.transform.position = RespawnPos; // Reset player position
        Player.forwardSpeed = currentspeed;
        Player.sidewaysSpeed = currentsidespeed;
       player.SetActive(true);
       
       uiManager.Allpaneldisable(); // Disable all panels
        uiManager.scoreText.gameObject.SetActive(true);
        uiManager.CoinText.gameObject.SetActive(true);

        CoinCount = CoinCount - uiManager.coinsneededtoRespawn;
        uiManager.coinsneededtoRespawn += 10;
        uiManager.CoinText.text = "Coins: " + CoinCount.ToString();

        PlayerPrefs.SetInt("GameCoins", CoinCount);
        PlayerPrefs.Save();
    }
}

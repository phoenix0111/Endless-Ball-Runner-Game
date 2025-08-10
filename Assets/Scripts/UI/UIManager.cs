using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UIManager : MonoBehaviour
    {
    [Header("Essentials")]
    [SerializeField] GameManager gameManager;
    [SerializeField] InputActions PlayerInputManager;


    [Header("Score")]
        public TextMeshProUGUI scoreText;
        public float UIscore = 0;
       
        [SerializeField] TextMeshProUGUI scoreGameOver;

        [Header("Pause Game")]
        [SerializeField] GameObject Pausemenu;
        [SerializeField] GameObject GameOverScreen;

        void Start()
        {
            if(Pausemenu != null) Pausemenu.SetActive(false);
            if (GameOverScreen != null) GameOverScreen.SetActive(false);

            Time.timeScale = 1; // Ensure the game starts at normal speed

            //Registering UI manager using Service Locator.
            ServiceLocator.ForSceneOf(this).Register<UIManager>(this);
        }


        void Update()
        {
            if (scoreText != null)        // score IN GAMEOVER PANEL
            {
              UIscore = gameManager.score;
              scoreGameOver.text = "Your Score\n" +UIscore.ToString(); // Update the score text in Game Over screen
            }

        }

        public void ShowGameOverScreen()
        {
            GameOverScreen.SetActive(true);
        }

        public void PlayGame()
        {
            Time.timeScale = 1; // Ensure the game is running at normal speed
            SceneManager.LoadScene("Game"); // Load the game scene
        }

        public void pausegame()
        {
            Time.timeScale = 0; 
            Pausemenu.SetActive(true); 
        }

        public void resumeGame()
        {
            Time.timeScale = 1; 
            Pausemenu.SetActive(false);
       
    }

        public void quitGame()
        {
            Application.Quit(); 
           
        }

        public void restartGame()
        {
            Time.timeScale = 1; 
           SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Restart the current scene
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1; 
            SceneManager.LoadScene("MainMenu"); // Load the main menu scene
        }
    }


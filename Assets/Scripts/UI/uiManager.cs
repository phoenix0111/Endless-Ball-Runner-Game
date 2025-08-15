using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;


public class uiManager : MonoBehaviour
{
    [Header("Essentials")]
    [SerializeField] Gamemanager gameManager ;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI CoinText;
    [SerializeField] GameObject Pausemenu;
    [SerializeField] GameObject Respawnmenu;
    [SerializeField] GameObject GameOvermenu;

    [Header("Respawn mechanic")]
    public int coinsneededtoRespawn;


    void Start()
    {
        Allpaneldisable();
    }

   
    void Update()
    {
        
    }

   public void  Allpaneldisable()
    {
        if (Pausemenu != null) Pausemenu.SetActive(false);
        if (Respawnmenu != null) Respawnmenu.SetActive(false);
        if (GameOvermenu != null) GameOvermenu.SetActive(false);
    }

    public void PlayGame()
    {
        Time.timeScale = 1; // Ensure the game is running at normal speed
        SceneManager.LoadScene("GameScene"); // Load the game scene
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


    public void OnPlayerDead()
    {
        int coin;
        coin = gameManager.CoinCount;

        if (coin >= coinsneededtoRespawn) Respawnmenu.SetActive(true);

        else
        {
            Respawnmenu.SetActive(false);
            GameOvermenu.SetActive(true);
        }

        scoreText.gameObject.SetActive(false);
        CoinText.gameObject.SetActive(false);
    }
}

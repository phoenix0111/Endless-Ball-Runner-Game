using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Net.Mail;

public class RegionLoader : MonoBehaviour
{
    [Header("Essentials")]
    public int TotalCoins;
    [SerializeField] TextMeshProUGUI coinsText;
    private int levelcheck = 1;


    [Header(" Panels")]
    [SerializeField] GameObject MainPanel;
    [SerializeField] GameObject ShopPanel;
    [SerializeField] GameObject RegionFullPanel;
    [SerializeField] GameObject Region1Panel;
    [SerializeField] GameObject Region2Panel;
    [SerializeField] GameObject OutfitShopPanel;


    [Header("Region2 Lock")]
    [SerializeField] GameObject Region2Lock;
    [SerializeField] int CoinsToBuyRegion2;

    [Header("Buttons")]
    [SerializeField] Button Region1Select;
    [SerializeField] Button Region2Select;
    [SerializeField] Button Region2Buy;


    [Header("Shop Essentials")]
    [SerializeField] int CharacterIndex = 1;
    [SerializeField] int CoinsToBuyChar2;
    [SerializeField] Button BuyChar2;
    [SerializeField] Button Char1;
    [SerializeField] Button Char2;

    
    void Start()
    {
        MainPanel.SetActive(true);
        ShopPanel.SetActive(false);
        RegionFullPanel.SetActive(false);
        OutfitShopPanel.SetActive(false);
      

        

        CoinSystem();



        int RegionCheck = PlayerPrefs.GetInt("RegionIndex",1);
        if (RegionCheck == 2)
        {
           
            Destroy(Region2Buy.gameObject);
            Destroy(Region2Lock);
            Region2Select.interactable = true;

        }

        int OutfitCheck = PlayerPrefs.GetInt("OutfitIndex", 1);
        if (OutfitCheck == 2)
        {
            Destroy(BuyChar2.gameObject);
            Char2.interactable = true;

        }
    }

  
    void Update()
    {
        if (TotalCoins >= CoinsToBuyRegion2)
        {
            Region2Buy.interactable = true;
        }

        if (TotalCoins >= CoinsToBuyChar2)
        {
            BuyChar2.interactable = true;
        }


    }
    public void PlayGame()
    {
        Time.timeScale = 1; // Ensure the game is running at normal speed

        if(levelcheck ==1)
        SceneManager.LoadScene("GameScene1"); // Load the game scene

        else
        {
            SceneManager.LoadScene("GameScene2");
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ShopPanelOpen()
    {
        MainPanel.SetActive(false);
        ShopPanel.SetActive(true);
        
    }

    public void OutFitButton()
    {
        ShopPanel.SetActive(false); 
        OutfitShopPanel.SetActive(true);    
    }

    public void RegionButton()
    {
        RegionFullPanel.SetActive(true);
        ShopPanel.SetActive(false);
    }

    public void ShopBackToMain()
    {
        ShopPanel.SetActive(false);
        MainPanel.SetActive(true);  
    }
    void CoinSystem()
    {
        TotalCoins = PlayerPrefs.GetInt("MenuCoins");
        int Coin = PlayerPrefs.GetInt("GameCoins");
        TotalCoins = TotalCoins + Coin;
        Region2Select.interactable = false;

        coinsText.text = TotalCoins.ToString();

        PlayerPrefs.SetInt("MenuCoins", TotalCoins);
        PlayerPrefs.Save();
    }

    public void nextButton()
    {
        Region1Panel.SetActive(false);
        Region2Panel.SetActive(true);
    }

    public void prevButton()
    {

        Region2Panel.SetActive(false);
        Region1Panel.SetActive(true);
    }

    public void SelectRegion1()
    {
        levelcheck = 1;
        
        RegionFullPanel.SetActive(false) ;
        ShopPanel.SetActive(true);



    }

    public void SelectRegion2()
    {

        levelcheck = 2;
        
        RegionFullPanel.SetActive(false);
        ShopPanel.SetActive(true);
    }

    public void OutfitShopLoad()
    {
        OutfitShopPanel.gameObject.SetActive(true);
    }

    public void OutfitShopClose()
    {
        OutfitShopPanel.gameObject.SetActive(false);
        ShopPanel.gameObject.SetActive(true);
    }

    public void CharSelect1()
    {
        CharacterIndex = 1;
        PlayerPrefs.SetInt("CharIndex", CharacterIndex);
        PlayerPrefs.Save();
      
        OutfitShopClose();

    }

    public void CharSelect2()
    {
        CharacterIndex = 2;
        PlayerPrefs.SetInt("CharIndex", CharacterIndex);
        PlayerPrefs.Save();
        OutfitShopClose();

    }

    public void BuyRegion2()
    {
        if (TotalCoins >= CoinsToBuyRegion2)
        {
            Destroy(Region2Lock);
            Region2Select.interactable = true;
            TotalCoins = TotalCoins - CoinsToBuyRegion2;
            coinsText.text = " Coins" + TotalCoins.ToString();
           PlayerPrefs.SetInt("MenuCoins", TotalCoins);
           PlayerPrefs.Save();
           
            PlayerPrefs.SetInt("RegionIndex", 2);
            PlayerPrefs.Save();
            Destroy(Region2Buy.gameObject);
        }

        else Debug.Log("Not enough coins");
    }

    public void BuyCharacterSkin2()
    {
        if (TotalCoins >= CoinsToBuyChar2)
        {

            Char2.interactable = true;
            TotalCoins = TotalCoins - CoinsToBuyChar2;
            coinsText.text = " Coins" + TotalCoins.ToString();
            PlayerPrefs.SetInt("MenuCoins", TotalCoins);
            PlayerPrefs.Save();
            Debug.Log("SKIN2 BOUGHT");

            PlayerPrefs.SetInt("OutfitIndex", 2);
            PlayerPrefs.Save();
            Destroy(BuyChar2.gameObject);
        }

        else Debug.Log("Not enough coins");
    }
}

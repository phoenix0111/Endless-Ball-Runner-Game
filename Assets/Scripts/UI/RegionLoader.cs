using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RegionLoader : MonoBehaviour
{
    [Header("Essentrials")]
    public int Coin;


    [Header(" Panels")]
    [SerializeField] GameObject Region1Panel;
    [SerializeField] GameObject Region2Panel;
    [SerializeField] GameObject OutfitShopPanel;

    [Header("Region2 Lock")]
    [SerializeField] GameObject Region2Lock;
    [SerializeField] int CoinsToBuyRegion2;

    [Header("Buttons")]
    [SerializeField] Button Region1Select;
    [SerializeField] Button Region2Select;


    [Header("Shop Essentials")]
    [SerializeField] int CharacterIndex = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Region1Panel.SetActive(true);
        Region2Panel.SetActive(false);
        OutfitShopPanel.gameObject.SetActive(false);

        Coin =   PlayerPrefs.GetInt("Coins");
        Region2Select.interactable = false;


    }

    // Update is called once per frame
    void Update()
    {
        if (Coin >= CoinsToBuyRegion2)
        {
            Destroy(Region2Lock);
            Region2Select.interactable = true;
        }

        
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
        SceneManager.LoadScene("GameScene1");
    }

    public void SelectRegion2()
    {
        SceneManager.LoadScene("GameScene2");
    }

    public void OutfitShopLoad()
    {
        OutfitShopPanel.gameObject.SetActive(true);
    }

    public void OutfitShopClose()
    {
        OutfitShopPanel.gameObject.SetActive(false);
        Region1Panel.gameObject.SetActive(true);
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
}

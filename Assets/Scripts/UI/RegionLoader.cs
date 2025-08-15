using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RegionLoader : MonoBehaviour
{
    [Header("Essentrials")]
    public int Coin;

    [Header("Regions Panels")]
    [SerializeField] GameObject Region1Panel;
    [SerializeField] GameObject Region2Panel;

    [Header("Region2 Lock")]
    [SerializeField] GameObject Region2Lock;
    [SerializeField] int CoinsToBuyRegion2;

    [Header("Buttons")]
    [SerializeField] Button Region1Select;
    [SerializeField] Button Region2Select;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Region1Panel.SetActive(true);
        Region2Panel.SetActive(false);

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

}

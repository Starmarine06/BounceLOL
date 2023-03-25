using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ShopManager2 : MonoBehaviour
{
    public int[,] shopItems2 = new int[5, 5];
    public float coins;
    public Text CoinsTxt;
    public RewardedAdsButton rewardedAdsButton;
    public AudioSource audioSource;

    void Start()
    {
        coins = PlayerPrefs.GetInt("Coins");
        CoinsTxt.text = "Money: $" + coins.ToString();

        //ID's
        shopItems2[1, 1] = 1;
        shopItems2[1, 2] = 2;
        shopItems2[1, 3] = 3;

        //Price
        shopItems2[2, 1] = 1;
        shopItems2[2, 2] = 3;
        shopItems2[2, 3] = 5;

        //Quantityasda
        shopItems2[3, 1] = 500;
        shopItems2[3, 2] = 1500;
        shopItems2[3, 3] = 2500;
    }

    void Update()
    {
        CoinsTxt.text = "Money: $" + coins.ToString();
        if (PlayerPrefs.GetInt("Audio") == 1)
        {
            audioSource.enabled = true;
        }
        else
        {
            audioSource.enabled = false;
        }
    }

    public void Buy()
    {
        GameObject ButtonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>().currentSelectedGameObject;
        PlayerPrefs.SetInt("AdsNo", shopItems2[2, ButtonRef.GetComponent<ButtonInfo2>().ItemID]);
        rewardedAdsButton.LoadAd();
    }
    public void ChangeScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public int[,] shopItems = new int[5, 5];
    public float coins;
    public Text CoinsTxt;
    public Text NotEnoughTxt;
    public AudioSource audioSource;
    string name;

    void Start()
    {
        NotEnoughTxt.enabled = false;

        coins = PlayerPrefs.GetInt("Coins");
        CoinsTxt.text = "Coins:" + coins.ToString();

        //ID's
        shopItems[1, 1] = 1;
        shopItems[1, 2] = 2;
        shopItems[1, 3] = 3;
        shopItems[1, 4] = 4;

        //Price
        shopItems[2, 1] = 1000;
        shopItems[2, 2] = 2000;
        shopItems[2, 3] = 3500;
        shopItems[2, 4] = 5000;

        //Quantity
        shopItems[3, 1] = 0;
        shopItems[3, 2] = 0;
        shopItems[3, 3] = 0;
        shopItems[3, 4] = 0;
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

    public void Buy(Button button)
    {
        GameObject ButtonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>().currentSelectedGameObject;

        if (coins >= shopItems[2, ButtonRef.GetComponent<ButtonInfo>().ItemID])
        {
            name = "Button" + ButtonRef.GetComponent<ButtonInfo>().ItemID;
            PlayerPrefs.SetInt(name, 1);
            button.interactable = false;
            Debug.Log(button.interactable);
            coins -= shopItems[2, ButtonRef.GetComponent<ButtonInfo>().ItemID];
            shopItems[3, ButtonRef.GetComponent<ButtonInfo>().ItemID]++;
            CoinsTxt.text = "Money: $" + coins.ToString();
            PlayerPrefs.SetInt("Coins", ((int)coins));
            PlayerPrefs.SetInt(ButtonRef.GetComponent<ButtonInfo>().ItemID.ToString(), 1);
        }
        else
        {
            StartCoroutine(CoinsLess());
        }
    }
    IEnumerator CoinsLess()
    {
        NotEnoughTxt.enabled = true;
        yield return new WaitForSeconds(1);
        NotEnoughTxt.enabled = false;
    }
    public void ChangeScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
}

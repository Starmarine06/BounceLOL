using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsManager : MonoBehaviour
{
    public Text coinText;
    public int coins;
    public int Value;
    void Start()
    {
        coins = PlayerPrefs.GetInt("Coins");
        coinText.text = coins.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        coins = PlayerPrefs.GetInt("Coins");
        coinText.text = "Money: $" + PlayerPrefs.GetInt("Coins").ToString();
    }

    public void adCoins(int value)
    {
        Value = coins + value;
        PlayerPrefs.SetInt("Coins", Value);
        Value = 0;
    }
    public void removeCoins(int value)
    {
        Value = coins - value;
        PlayerPrefs.SetInt("Coins", Value);
        Value = 0;
    }
}

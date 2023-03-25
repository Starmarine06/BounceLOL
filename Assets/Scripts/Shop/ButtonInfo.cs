using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInfo : MonoBehaviour
{
    public int ItemID;
    public Text PriceTxt;
    public GameObject ShopManager;
    public ShopManager shopManager;
    public Button button;
    string name;

    void Awake()
    {
        name = "Button" + ItemID;
        if(PlayerPrefs.GetInt(name) == 1)
        {
            button.interactable = false;
        }
    }

    void Update()
    {
        PriceTxt.text = "$" + shopManager.shopItems[2,ItemID];
    }
}

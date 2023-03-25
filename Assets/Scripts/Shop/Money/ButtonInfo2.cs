using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInfo2 : MonoBehaviour
{
    public int ItemID;
    public Text PriceTxt;
    public Text AdsTxt;
    public ShopManager2 shopManager;
    void Update()
    {
        PriceTxt.text = "Ads: " + shopManager.shopItems2[2,ItemID];
        AdsTxt.text = "$" + shopManager.shopItems2[3, ItemID];
    }
}

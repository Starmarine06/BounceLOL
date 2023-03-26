using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailsShop : MonoBehaviour
{
    public Material[] material;
    public int[] price;
    public int i = 0;
    public LineRenderer line;
    int money;
    public void NextTrail()
    {
        if (i > 6)
        {
            i = 0;
        }
        else
        {
            i++;
        }
    }
    public void PrevTrail()
    {
        if (i < 1)
        {
            i = 7;
        }
        else
        {
            i--;
        }
    }

    private void Update()
    {
        line.material = material[i];
    }

    public void BuyTrail()
    {
        if (PlayerPrefs.GetInt("Coins") >= price[i])
        {
            money = PlayerPrefs.GetInt("Coins");
            money -= price[i];
            PlayerPrefs.SetInt("Coins", money);
            PlayerPrefs.SetInt("Trail" + i, 1);
            this.gameObject.active = false;
        }
    }

    public void SelectTrail()
    {
        if (PlayerPrefs.GetInt("Trail"+i) == 1)
        {
            PlayerPrefs.SetInt("Trail", i);
        }
        
    }
}

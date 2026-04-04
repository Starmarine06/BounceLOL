using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum ShopItemType
{
    Skin = 0,
    Trail = 1
}

[System.Serializable]
public class SkinShopItem
{
    public string displayName;
    public int cost;
    public Sprite previewSprite;
}

[System.Serializable]
public class TrailShopItem
{
    public string displayName;
    public int cost;
    public Color previewColor = Color.white;
}

public class ShopManager : MonoBehaviour
{
    [Header("Catalog")]
    public SkinShopItem[] skins;
    public TrailShopItem[] trails;

    [Header("UI")]
    public Text coinsText;
    public Text notEnoughText;
    public float notEnoughDuration = 2f;

    private const string CoinsKey = "Coins";
    private const string SelectedSkinKey = "selectedOption";
    private const string SelectedTrailKey = "Trail";

    private void Awake()
    {
        if (notEnoughText != null)
        {
            notEnoughText.enabled = false;
        }
    }

    private void Start()
    {
        EnsureDefaultsOwned();
        RefreshCoinsUi();
    }

    public int GetCoins()
    {
        return PlayerPrefs.GetInt(CoinsKey, 0);
    }

    public void AddCoins(int amount)
    {
        int next = Mathf.Max(0, GetCoins() + amount);
        PlayerPrefs.SetInt(CoinsKey, next);
        PlayerPrefs.Save();
        RefreshCoinsUi();
    }

    public int GetCost(ShopItemType itemType, int itemIndex)
    {
        if (itemType == ShopItemType.Skin)
        {
            if (skins == null || itemIndex < 0 || itemIndex >= skins.Length) return -1;
            return skins[itemIndex].cost;
        }

        if (trails == null || itemIndex < 0 || itemIndex >= trails.Length) return -1;
        return trails[itemIndex].cost;
    }

    public Sprite GetSkinPreview(int itemIndex)
    {
        if (skins == null || itemIndex < 0 || itemIndex >= skins.Length) return null;
        return skins[itemIndex].previewSprite;
    }

    public Color GetTrailPreviewColor(int itemIndex)
    {
        if (trails == null || itemIndex < 0 || itemIndex >= trails.Length) return Color.white;
        return trails[itemIndex].previewColor;
    }

    public bool IsOwned(ShopItemType itemType, int itemIndex)
    {
        if (itemIndex == 0) return true;
        return PlayerPrefs.GetInt(GetOwnedKey(itemType, itemIndex), 0) == 1;
    }

    public bool IsEquipped(ShopItemType itemType, int itemIndex)
    {
        if (itemType == ShopItemType.Skin)
        {
            return PlayerPrefs.GetInt(SelectedSkinKey, 0) == itemIndex;
        }

        return PlayerPrefs.GetInt(SelectedTrailKey, 0) == itemIndex;
    }

    public bool Purchase(ShopItemType itemType, int itemIndex)
    {
        if (IsOwned(itemType, itemIndex))
        {
            Equip(itemType, itemIndex);
            return true;
        }

        int cost = GetCost(itemType, itemIndex);
        if (cost < 0) return false;

        int coins = GetCoins();
        if (coins < cost)
        {
            ShowNotEnough();
            return false;
        }

        PlayerPrefs.SetInt(CoinsKey, coins - cost);
        PlayerPrefs.SetInt(GetOwnedKey(itemType, itemIndex), 1);
        Equip(itemType, itemIndex);
        PlayerPrefs.Save();
        RefreshCoinsUi();
        return true;
    }

    public void Equip(ShopItemType itemType, int itemIndex)
    {
        if (!IsOwned(itemType, itemIndex)) return;

        if (itemType == ShopItemType.Skin)
        {
            PlayerPrefs.SetInt(SelectedSkinKey, itemIndex);
        }
        else
        {
            PlayerPrefs.SetInt(SelectedTrailKey, itemIndex);
        }

        PlayerPrefs.Save();
        ShopEquipmentEvents.RaiseEquipmentChanged();
    }

    private void RefreshCoinsUi()
    {
        if (coinsText != null)
        {
            coinsText.text = "Money: $" + GetCoins();
        }
    }

    private void EnsureDefaultsOwned()
    {
        PlayerPrefs.SetInt(GetOwnedKey(ShopItemType.Skin, 0), 1);
        PlayerPrefs.SetInt(GetOwnedKey(ShopItemType.Trail, 0), 1);
        PlayerPrefs.Save();
        ShopEquipmentEvents.RaiseEquipmentChanged();
    }

    private static string GetOwnedKey(ShopItemType itemType, int itemIndex)
    {
        return (itemType == ShopItemType.Skin ? "Spurchased" : "Tpurchased") + itemIndex;
    }

    private void ShowNotEnough()
    {
        if (notEnoughText == null) return;
        notEnoughText.enabled = true;
        StopAllCoroutines();
        StartCoroutine(HideNotEnoughAfterDelay());
    }

    private IEnumerator HideNotEnoughAfterDelay()
    {
        yield return new WaitForSeconds(notEnoughDuration);
        if (notEnoughText != null)
        {
            notEnoughText.enabled = false;
        }
    }

    public void GoToMainMenu(int i)
    {
        SceneManager.LoadScene(i);
    }
}

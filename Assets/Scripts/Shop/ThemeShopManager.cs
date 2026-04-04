using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ThemeShopItem
{
    public string displayName;
    public int cost;
    [Tooltip("Index into ColorEffect.ThemeColors — controls obstacle / background palette.")]
    [Range(0, 32)]
    public int paletteIndex;
    public Sprite previewSprite;
    public Color previewSwatch = Color.white;
}

public class ThemeShopManager : MonoBehaviour
{
    [Header("Catalog")]
    public ThemeShopItem[] themes;

    [Header("UI")]
    public Text coinsText;
    public Text notEnoughText;
    public float notEnoughDuration = 2f;

    private const string CoinsKey = "Coins";
    private const string OwnedPrefix = "Themepurchased";
    private const string SelectedCatalogIndexKey = "ThemeSelectedCatalogIndex";
    public const string ThemePaletteIndexKey = "ThemePaletteIndex";

    private void Awake()
    {
        if (notEnoughText != null)
        {
            notEnoughText.enabled = false;
        }
    }

    private void Start()
    {
        EnsureDefaultOwned();
        ApplyEquippedPaletteToPrefs();
        RefreshCoinsUi();
    }

    public int GetCoins()
    {
        return PlayerPrefs.GetInt(CoinsKey, 0);
    }

    public int GetCost(int itemIndex)
    {
        if (themes == null || itemIndex < 0 || itemIndex >= themes.Length) return -1;
        return themes[itemIndex].cost;
    }

    public bool IsOwned(int itemIndex)
    {
        if (itemIndex == 0) return true;
        return PlayerPrefs.GetInt(OwnedPrefix + itemIndex, 0) == 1;
    }

    public bool IsEquipped(int itemIndex)
    {
        return PlayerPrefs.GetInt(SelectedCatalogIndexKey, 0) == itemIndex;
    }

    public int GetPaletteIndexForCatalogIndex(int itemIndex)
    {
        if (themes == null || itemIndex < 0 || itemIndex >= themes.Length) return 0;
        return Mathf.Clamp(themes[itemIndex].paletteIndex, 0, MaxPaletteIndex());
    }

    public static int GetEquippedPaletteIndexFromPrefs()
    {
        return PlayerPrefs.GetInt(ThemePaletteIndexKey, MainController.Prefs_ColorIndex_DefaultValue);
    }

    public bool Purchase(int itemIndex)
    {
        if (IsOwned(itemIndex))
        {
            Equip(itemIndex);
            return true;
        }

        int cost = GetCost(itemIndex);
        if (cost < 0) return false;

        int coins = GetCoins();
        if (coins < cost)
        {
            ShowNotEnough();
            return false;
        }

        PlayerPrefs.SetInt(CoinsKey, coins - cost);
        PlayerPrefs.SetInt(OwnedPrefix + itemIndex, 1);
        Equip(itemIndex);
        PlayerPrefs.Save();
        RefreshCoinsUi();
        return true;
    }

    public void Equip(int itemIndex)
    {
        if (!IsOwned(itemIndex)) return;

        PlayerPrefs.SetInt(SelectedCatalogIndexKey, itemIndex);
        ApplyEquippedPaletteToPrefs();
        PlayerPrefs.Save();
        ShopEquipmentEvents.RaiseEquipmentChanged();
    }

    private void ApplyEquippedPaletteToPrefs()
    {
        int idx = PlayerPrefs.GetInt(SelectedCatalogIndexKey, 0);
        int palette = GetPaletteIndexForCatalogIndex(idx);
        PlayerPrefs.SetInt(ThemePaletteIndexKey, palette);
    }

    private void EnsureDefaultOwned()
    {
        PlayerPrefs.SetInt(OwnedPrefix + 0, 1);
        if (!PlayerPrefs.HasKey(SelectedCatalogIndexKey))
        {
            PlayerPrefs.SetInt(SelectedCatalogIndexKey, 0);
        }
        ApplyEquippedPaletteToPrefs();
        PlayerPrefs.Save();
    }

    private void RefreshCoinsUi()
    {
        if (coinsText != null)
        {
            coinsText.text = "Money: $" + GetCoins();
        }
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

    private static int MaxPaletteIndex()
    {
        return ColorEffect.ThemeColors != null && ColorEffect.ThemeColors.Length > 0
            ? ColorEffect.ThemeColors.Length - 1
            : 0;
    }
}

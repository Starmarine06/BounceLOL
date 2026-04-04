using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ThemeButtonInfo : MonoBehaviour
{
    [Header("Item")]
    public int itemIndex;

    [Header("UI")]
    public Text priceText;
    public Text stateText;
    public Image previewImage;
    public Image swatchImage;
    public Button button;

    private ThemeShopManager shop;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        if (shop == null) shop = FindObjectOfType<ThemeShopManager>();
        button.onClick.AddListener(OnClickPurchaseOrEquip);
    }

    private void OnEnable()
    {
        ShopEquipmentEvents.OnEquipmentChanged += OnEquipmentChanged;
        Refresh();
    }

    private void OnDisable()
    {
        ShopEquipmentEvents.OnEquipmentChanged -= OnEquipmentChanged;
    }

    private void OnEquipmentChanged()
    {
        Refresh();
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnClickPurchaseOrEquip);
        }
    }

    public void OnClickPurchaseOrEquip()
    {
        if (shop == null) shop = FindObjectOfType<ThemeShopManager>();
        if (shop == null) return;

        if (shop.IsOwned(itemIndex))
        {
            shop.Equip(itemIndex);
        }
        else
        {
            shop.Purchase(itemIndex);
        }

        Refresh();
    }

    public void Refresh()
    {
        if (shop == null) shop = FindObjectOfType<ThemeShopManager>();
        if (shop == null) return;

        int cost = shop.GetCost(itemIndex);
        if (priceText != null)
        {
            priceText.text = cost >= 0 ? "$" + cost : "-";
        }

        bool owned = shop.IsOwned(itemIndex);
        bool equipped = shop.IsEquipped(itemIndex);
        if (stateText != null)
        {
            if (equipped) stateText.text = "Equipped";
            else if (owned) stateText.text = "Owned";
            else stateText.text = "Buy";
        }

        if (shop.themes != null && itemIndex >= 0 && itemIndex < shop.themes.Length)
        {
            var def = shop.themes[itemIndex];
            if (previewImage != null && def.previewSprite != null)
            {
                previewImage.sprite = def.previewSprite;
            }

            if (swatchImage != null)
            {
                swatchImage.color = def.previewSwatch;
            }
        }
    }
}

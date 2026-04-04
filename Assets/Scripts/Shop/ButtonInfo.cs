using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonInfo : MonoBehaviour
{
    [Header("Item")]
    public ShopItemType itemType = ShopItemType.Skin;
    public int itemIndex = 0;

    [Header("UI")]
    public Text priceText;
    public Text stateText;
    public SpriteRenderer skinPreviewImage;
    public SpriteRenderer trailColorPreviewImage;
    public Button button;

    private ShopManager shop;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        if (shop == null) shop = FindObjectOfType<ShopManager>();
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
        if (shop == null) shop = FindObjectOfType<ShopManager>();
        if (shop == null) return;

        if (shop.IsOwned(itemType, itemIndex))
        {
            shop.Equip(itemType, itemIndex);
        }
        else
        {
            shop.Purchase(itemType, itemIndex);
        }

        Refresh();
    }

    public void Refresh()
    {
        if (shop == null) shop = FindObjectOfType<ShopManager>();
        if (shop == null) return;

        int cost = shop.GetCost(itemType, itemIndex);
        if (priceText != null)
        {
            priceText.text = cost >= 0 ? "$" + cost : "-";
        }

        bool owned = shop.IsOwned(itemType, itemIndex);
        bool equipped = shop.IsEquipped(itemType, itemIndex);
        if (stateText != null)
        {
            if (equipped) stateText.text = "Equipped";
            else if (owned) stateText.text = "Owned";
            else stateText.text = "Buy";
        }

        if (skinPreviewImage != null)
        {
            bool show = itemType == ShopItemType.Skin;
            skinPreviewImage.gameObject.SetActive(show);
            if (show) skinPreviewImage.sprite = shop.GetSkinPreview(itemIndex);
        }

        if (trailColorPreviewImage != null)
        {
            bool show = itemType == ShopItemType.Trail;
            trailColorPreviewImage.gameObject.SetActive(show);
            if (show) trailColorPreviewImage.color = shop.GetTrailPreviewColor(itemIndex);
        }
    }
}
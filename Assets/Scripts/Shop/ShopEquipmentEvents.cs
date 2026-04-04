using System;

public static class ShopEquipmentEvents
{
    public static event Action OnEquipmentChanged;

    public static void RaiseEquipmentChanged()
    {
        OnEquipmentChanged?.Invoke();
    }
}

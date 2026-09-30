using System;
using UnityEngine;

/// <summary>
/// The only place that reads or writes the player's coin balance.
/// </summary>
public static class CoinWallet
{
    public static event Action<int> Changed;

    public static int Balance => PlayerPrefs.GetInt(GameKeys.Coins, 0);

    public static void Add(int amount)
    {
        Set(Balance + amount);
    }

    /// <summary>Spends coins if the player can afford it. Returns false (and changes nothing) if not.</summary>
    public static bool TrySpend(int cost)
    {
        if (cost < 0 || Balance < cost) return false;
        Set(Balance - cost);
        return true;
    }

    private static void Set(int value)
    {
        value = Mathf.Max(0, value);
        PlayerPrefs.SetInt(GameKeys.Coins, value);
        PlayerPrefs.Save();
        Changed?.Invoke(value);
    }
}

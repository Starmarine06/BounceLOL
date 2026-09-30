using UnityEngine;
using UnityEngine.UI;

/// <summary>Shows the coin balance in the UI. All balance changes go through <see cref="CoinWallet"/>.</summary>
public class CoinsManager : MonoBehaviour
{
    public Text coinText;

    public int coins => CoinWallet.Balance;

    // Kept so the serialized field on existing scenes/prefabs is not lost.
    [HideInInspector]
    public int Value;

    void OnEnable()
    {
        CoinWallet.Changed += UpdateVisuals;
        UpdateVisuals(CoinWallet.Balance);
    }

    void OnDisable()
    {
        CoinWallet.Changed -= UpdateVisuals;
    }

    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C))
        {
            adCoins(100);
        }
        #endif
    }

    public void UpdateVisuals(int coinCount)
    {
        if (coinText != null)
        {
            coinText.text = "Money: $" + coinCount;
        }
    }

    public void adCoins(int value)
    {
        CoinWallet.Add(value);
    }

    public void removeCoins(int value)
    {
        CoinWallet.Add(-value);
    }
}

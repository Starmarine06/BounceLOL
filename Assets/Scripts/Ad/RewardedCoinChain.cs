using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;

/// <summary>
/// Plays floor(coinAmount / coinsPerAdStep) rewarded ads in sequence, then grants the full coin amount once.
/// Call <see cref="BeginRewardedChain"/> from a UI button.
/// </summary>
public class RewardedCoinChain : MonoBehaviour
{
    [SerializeField] private string adUnitId;
    [SerializeField] private int coinsPerAdStep = 500;
    [Tooltip("If coinAmount is not a multiple of coinsPerAdStep, grant the remainder together with the last reward.")]
    [SerializeField] private bool includeRemainderInFinalGrant = true;

    public CoinsManager coinsManager;

    [SerializeField] private bool logToConsole = true;

    private RewardedAd rewardedAd;
    private bool isLoading;
    private bool chainActive;

    private int targetCoinAmount;
    private int adsRequired;
    private int adsCompleted;

    public event Action<int, int> OnProgress;
    public event Action<int> OnChainComplete;
    public event Action<string> OnChainFailed;

    private void OnDestroy()
    {
        DestroyLoadedAd();
    }

    public void BeginRewardedChain(int coinAmount)
    {
        bool grantIfBelowStep = true;
        if (chainActive)
        {
            Log("A rewarded chain is already running.");
            return;
        }

        if (string.IsNullOrWhiteSpace(adUnitId))
        {
            OnChainFailed?.Invoke("Ad unit id is empty.");
            return;
        }

        if (coinAmount <= 0)
        {
            OnChainFailed?.Invoke("Coin amount must be positive.");
            return;
        }

        targetCoinAmount = coinAmount;
        adsRequired = coinAmount / coinsPerAdStep;

        if (adsRequired <= 0)
        {
            if (grantIfBelowStep)
            {
                GrantCoinsAndFinish(coinAmount);
            }
            else
            {
                OnChainFailed?.Invoke("Amount is below one ad step; no ads required.");
            }
            return;
        }

        chainActive = true;
        adsCompleted = 0;
        OnProgress?.Invoke(adsCompleted, adsRequired);
        LoadThenShow();
    }

    private void LoadThenShow()
    {
        if (isLoading) return;

        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        isLoading = true;
        var request = new AdRequest();
        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            isLoading = false;
            if (error != null)
            {
                LogError("Rewarded load failed: " + error);
                FailChain("Ad failed to load: " + error);
                return;
            }

            if (ad == null)
            {
                FailChain("Rewarded load returned null ad.");
                return;
            }

            rewardedAd = ad;

            if (rewardedAd.CanShowAd())
            {
                RegisterFailHandler(ad);
                ShowLoadedAd();
            }
            else
            {
                FailChain("Ad loaded but cannot show.");
            }
        });
    }

    private void RegisterFailHandler(RewardedAd ad)
    {
        ad.OnAdFullScreenContentFailed += OnShowFailed;

        void OnShowFailed(AdError error)
        {
            ad.OnAdFullScreenContentFailed -= OnShowFailed;
            if (!chainActive) return;
            FailChain("Ad failed to open: " + error);
        }
    }

    private void ShowLoadedAd()
    {
        if (rewardedAd == null) return;

        var ad = rewardedAd;
        bool earned = false;

        void OnClosed()
        {
            ad.OnAdFullScreenContentClosed -= OnClosed;
            if (!chainActive) return;
            if (!earned)
            {
                FailChain("Ad closed before reward.");
            }
        }

        ad.OnAdFullScreenContentClosed += OnClosed;

        ad.Show(_ =>
        {
            earned = true;
            OnSingleAdRewarded();
        });
    }

    private void OnSingleAdRewarded()
    {
        adsCompleted++;
        OnProgress?.Invoke(adsCompleted, adsRequired);
        Log($"Rewarded ad {adsCompleted}/{adsRequired}");

        if (adsCompleted >= adsRequired)
        {
            int grant = targetCoinAmount;
            if (!includeRemainderInFinalGrant)
            {
                grant = adsRequired * coinsPerAdStep;
            }

            GrantCoinsAndFinish(grant);
            DestroyLoadedAd();
            return;
        }

        LoadThenShow();
    }

    private void GrantCoinsAndFinish(int coins)
    {
        int current = PlayerPrefs.GetInt("Coins", 0);
        PlayerPrefs.SetInt("Coins", current + coins);
        PlayerPrefs.Save();

        if (coinsManager != null && coinsManager.coinText != null)
        {
            coinsManager.coinText.text = "Money: $" + PlayerPrefs.GetInt("Coins", 0);
        }

        chainActive = false;
        targetCoinAmount = 0;
        adsRequired = 0;
        adsCompleted = 0;

        OnChainComplete?.Invoke(coins);
        Log("Chain complete. Granted " + coins + " coins.");
    }

    private void FailChain(string message)
    {
        chainActive = false;
        targetCoinAmount = 0;
        adsRequired = 0;
        adsCompleted = 0;
        DestroyLoadedAd();
        OnChainFailed?.Invoke(message);
        LogError(message);
    }

    private void DestroyLoadedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }
    }

    private void Log(string msg)
    {
        if (logToConsole) Debug.Log("[RewardedCoinChain] " + msg);
    }

    private void LogError(string msg)
    {
        Debug.LogError("[RewardedCoinChain] " + msg);
    }

    public void SceneChange(int i)
    {
        SceneManager.LoadScene(i);
    }
}

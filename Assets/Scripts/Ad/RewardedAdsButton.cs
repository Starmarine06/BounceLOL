using UnityEngine;
using GoogleMobileAds.Api;
public class RewardedAdsButton : MonoBehaviour
{
    [SerializeField] private string _adUnitId;
    [SerializeField] private int rewardCoinsPerView = 500;
    public CoinsManager coinsManager;
    private RewardedAd rewardedAd;
    private bool isLoading;

    private void Start()
    {
        LoadAd();
    }

    public void LoadAd()
    {
        if (string.IsNullOrWhiteSpace(_adUnitId))
        {
            Debug.LogError("RewardedAdsButton: Ad Unit Id is empty.");
            return;
        }

        if (isLoading) return;
        isLoading = true;

        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        var adRequest = new AdRequest();
        RewardedAd.Load(_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                isLoading = false;
                // If the operation failed with a reason.
                if (error != null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                    return;
                }
                // If the operation failed for unknown reasons.
                // This is an unexpected error, please report this bug if it happens.
                if (ad == null)
                {
                    Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                    return;
                }

                // The operation completed successfully.
                Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
                rewardedAd = ad;

                // Register to ad events to extend functionality.
                RegisterEventHandlers(ad);
            });
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                GrantReward();
            });
            return;
        }

        LoadAd();
        Debug.Log("Rewarded ad is not ready yet.");
    }

    // Backward-compatible method name for existing button bindings.
    public void ShowAd(int amt)
    {
        rewardCoinsPerView = amt;
        ShowRewardedAd();
    }

    private void GrantReward()
    {
        int currentCoins = PlayerPrefs.GetInt("Coins", 0);
        PlayerPrefs.SetInt("Coins", currentCoins + rewardCoinsPerView);
        PlayerPrefs.Save();
        if (coinsManager != null)
        {
            coinsManager.coinText.text = "Money: $" + PlayerPrefs.GetInt("Coins", 0);
        }
        LoadAd();
    }

    // Backward-compatible method name kept for older event wiring.
    public void OnUnityAdsShowComplete(string ignoredAdUnitId)
    {
        GrantReward();
    }

    void OnDestroy()
    {
        if (rewardedAd != null)
        {
            Debug.Log("Destroying rewarded ad.");
            rewardedAd.Destroy();
            rewardedAd = null;
        }
    }
    
    private void RegisterEventHandlers(RewardedAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log("Rewarded ad paid " + adValue.Value + " " + adValue.CurrencyCode);
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Rewarded ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                Debug.Log("Rewarded ad was clicked.");
            };
            // Raised when the ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Rewarded ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded ad full screen content closed.");
                LoadAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Rewarded ad failed to open full screen content with error : "
                    + error);
                LoadAd();
            };
        }


}
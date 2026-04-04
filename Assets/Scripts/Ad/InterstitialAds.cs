using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using System;

public class InterstitialAds : MonoBehaviour
{
    [SerializeField] string _adUnitId;
    [SerializeField] int sceneToLoadAfterAd = 1;
    [SerializeField] float chanceToShow = 0.3f;
    private InterstitialAd interstitialAd;
    private Action pendingAfterCloseAction;

    // Load content to the Ad Unit:
    public void LoadAd()
    {
        if (string.IsNullOrWhiteSpace(_adUnitId))
        {
            Debug.LogError("InterstitialAds: Ad Unit Id is empty.");
            return;
        }

        if (interstitialAd != null)
        {
            OnDestroy();
        }
        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        InterstitialAd.Load(_adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                // The ad failed to load.
                Debug.LogError("Interstitial load failed: " + error);
                return;
            }

            // The ad loaded successfully.
            interstitialAd = ad;

            // Register to ad events to extend functionality.
            RegisterEventHandlers(ad);
        });

        

    }

    // Show ad if loaded, otherwise fallback action runs.
    public void ShowAd(Action onAdUnavailable)
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            pendingAfterCloseAction = onAdUnavailable;
            interstitialAd.Show();
            return;
        }

        onAdUnavailable?.Invoke();
    }

    public void ShowAdOrLoadScene()
    {
        bool shouldShow = UnityEngine.Random.value <= chanceToShow;
        if (!shouldShow)
        {
            SceneManager.LoadScene(sceneToLoadAfterAd);
            return;
        }

        ShowAd(() => SceneManager.LoadScene(sceneToLoadAfterAd));
    }
    private void RegisterEventHandlers(InterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log("Interstitial ad paid " + adValue.Value + " " + adValue.CurrencyCode);
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            pendingAfterCloseAction?.Invoke();
            pendingAfterCloseAction = null;
            LoadAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content with error : "
                + error);
            pendingAfterCloseAction?.Invoke();
            pendingAfterCloseAction = null;
        };
    }

    private void Start()
    {
        LoadAd();
    }

    void OnDestroy()
    {
        if (interstitialAd != null)
        {
            Debug.Log("Destroying interstitial ad.");
            interstitialAd.Destroy();
            interstitialAd = null;
        }
    }
 
}
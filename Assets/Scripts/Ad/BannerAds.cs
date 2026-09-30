using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class BannerAds : MonoBehaviour
{
    [SerializeField] string _adUnitId = null;
    private BannerView bannerView;


    void Start()
    {
        if (string.IsNullOrWhiteSpace(_adUnitId))
        {
            Debug.LogError("BannerAds: Ad Unit Id is empty.");
            return;
        }

        if (AdsInitializer.IsInitialized)
        {
            CreateBanner();
            return;
        }

        MobileAds.Initialize(initStatus =>
        {
            CreateBanner();
        });
    }

    private void CreateBanner()
    {
        // Adaptive banners fill far better than a custom 400x120 size.
        AdSize adSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        bannerView = new BannerView(_adUnitId, adSize, AdPosition.Bottom);
        LoadBanner();
    }

    private void LoadBanner()
    {
        bannerView.LoadAd(new AdRequest());
        Debug.Log("Banner load requested.");
    }

    private void OnDestroy()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }
    }
}
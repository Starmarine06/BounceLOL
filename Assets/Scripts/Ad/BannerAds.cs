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

        bannerView = new BannerView(_adUnitId, AdSize.IABBanner, AdPosition.BottomRight);
        LoadBanner();
    }

    private void LoadBanner()
    {
        bannerView.LoadAd(new AdRequest());
        Debug.Log("Banner Loaded");
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
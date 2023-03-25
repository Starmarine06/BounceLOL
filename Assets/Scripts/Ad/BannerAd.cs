using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class BannerAd : MonoBehaviour
{
    // For the purpose of this example, these buttons are for functionality testing:
    //[SerializeField] Button _loadBannerButton;
    //[SerializeField] Button _showBannerButton;
    //[SerializeField] Button _hideBannerButton;

    [SerializeField] BannerPosition _bannerPosition = BannerPosition.TOP_LEFT;

    [SerializeField] string _androidAdUnitId = "Banner_Android";
    [SerializeField] string _iOSAdUnitId = "Banner_iOS";
    string _adUnitId = null; // This will remain null for unsupported platforms.
    public string gameId = "5178212";
    public bool testMode = true;


    void Start()
    {
        // Get the Ad Unit ID for the current platform:
        //_adUnitId = _androidAdUnitId;

        // Disable the button until an ad is ready to show:
        //_showBannerButton.interactable = false;
        //_hideBannerButton.interactable = false;

        // Set the banner position:
        Advertisement.Banner.SetPosition(_bannerPosition);

        // Configure the Load Banner button to call the LoadBanner() method when clicked:
        //_loadBannerButton.onClick.AddListener(LoadBanner);
        //_loadBannerButton.interactable = true;
        Advertisement.Initialize(gameId, testMode);
        InvokeRepeating("LoadBanner", 2f, 4f);
    }

    private void LoadBanner()
    {
        Advertisement.Banner.Show(_androidAdUnitId);
        Debug.Log("Banner Loaded");
    }
}
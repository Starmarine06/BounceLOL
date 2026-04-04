using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class AdsInitializer : MonoBehaviour
{
    public static bool IsInitialized { get; private set; }
    private static bool initRequested;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        InitializeAds();
    }

    public void InitializeAds()
    {
        if (IsInitialized || initRequested)
        {
            return;
        }

        initRequested = true;

        MobileAds.Initialize((InitializationStatus initstatus) =>
        {
            if (initstatus == null)
            {
                Debug.LogError("Google Mobile Ads initialization failed.");
                initRequested = false;
                return;
            }

            IsInitialized = true;
            Debug.Log("Google Mobile Ads initialization complete.");
        });
    }
}
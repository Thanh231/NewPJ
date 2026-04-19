
#if USE_ADMOB

using GoogleMobileAds.Api;

public partial class AdImplementation_admob : IAdImplementation
{
	#region core

	public string AdNetworkNameForAdjustLogging => "admob_sdk";
	public string AdNetworkNameForFirebaseLogging => "Google Admob";

	public AdImplementation_admob()
	{
		MobileAds.RaiseAdEventsOnUnityMainThread = true;
#if UNITY_IOS
        MobileAds.SetiOSAppPauseOnBackground(true); 
#endif

		ShowUMPPopup_startUp(_ =>
		{
			if (AppTrackingTransparencyController.instance)
			{
				AppTrackingTransparencyController.instance.RequestPermission(() => { });
			}
		});

		MobileAds.Initialize(_ =>
		{
			LoadInterstitial();
			LoadRewarded();
		});
	}

	private static float AdValueToUsd(AdValue adValue)
	{
		//The ad's value in micro-units, where 1,000,000 micro-units equal one unit of the currency.
		return adValue.Value / 1000000f;
	}
	
	#endregion
	
	#region ad unit id

	private readonly AdUnitConfig testingAdUnitConfig = new()
	{
		androidBanner = "ca-app-pub-3940256099942544/6300978111",
		androidInterstitial = "ca-app-pub-3940256099942544/1033173712",
		androidRewardedVideo = "ca-app-pub-3940256099942544/5224354917",
		
		iosBanner = "ca-app-pub-3940256099942544/2934735716",
		iosInterstitial = "ca-app-pub-3940256099942544/4411468910",
		iosRewardedVideo = "ca-app-pub-3940256099942544/1712485313",
	};

	private AdUnitConfig _adUnitConfig => GameFrameworkConfig.instance.admobUseTestMode
		? testingAdUnitConfig
		: GameFrameworkConfig.instance.admobUnitCfg;

	private string GetAdUnitID_banner()
	{
#if UNITY_IOS
		return _adUnitConfig.iosBanner;
#else
		return _adUnitConfig.androidBanner;
#endif
	}

	private string GetAdUnitID_interstitial()
	{
#if UNITY_IOS
		return _adUnitConfig.iosInterstitial;
#else
		return _adUnitConfig.androidInterstitial;
#endif
	}
	
	private string GetAdUnitID_rewardedVideo()
	{
#if UNITY_IOS
		return _adUnitConfig.iosRewardedVideo;
#else
		return _adUnitConfig.androidRewardedVideo;
#endif
	}

	#endregion
}

#endif
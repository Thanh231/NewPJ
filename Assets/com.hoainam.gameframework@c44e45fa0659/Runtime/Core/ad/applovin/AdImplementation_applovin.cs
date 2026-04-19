using Cysharp.Threading.Tasks;
using UnityEngine;

#if USE_APPLOVIN

/// <summary>
/// to test applovin ads, package name need to be the same as
/// package name on applovin console
/// </summary>
public partial class AdImplementation_applovin : IAdImplementation
{
	public string AdNetworkNameForAdjustLogging => "applovin_max_sdk";
	public string AdNetworkNameForFirebaseLogging => "AppLovin";
	
	public UniTask Initialize()
	{
		var utcs = new UniTaskCompletionSource();
		
		AdImplementation_applovin_interstitial();
		AdImplementation_applovin_rewarded();

		MaxSdkCallbacks.OnSdkInitializedEvent += configuration =>
		{
			var success = $"IsSuccessfullyInitialized={configuration.IsSuccessfullyInitialized}";
			Debug.Log(
				$"[AppLovin] SDK initialized. {success}");

			LoadInterstitial();
			LoadRewarded();

			utcs.TrySetResult();
		};
		//when initialize sdk, a popup privacy policy will show,
		//so we need to wait until user click ok to continue loading game.
		//that popup close will trigger OnSdkInitializedEvent
		MaxSdk.InitializeSdk();
		
		return utcs.Task;
	}
	
	private static float AdValueToUsd(MaxSdkBase.AdInfo adInfo)
	{
		if (adInfo == null)
		{
			return 0;
		}

		return (float)adInfo.Revenue;
	}

	#region ad unit id

	private AdUnitConfig _adUnitConfig => GameFrameworkConfig.instance.appLovinUnitCfg;

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
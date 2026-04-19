#if USE_APPLOVIN

using UnityEngine;
using UnityEngine.Events;

public partial class AdImplementation_applovin
{
	private UnityAction<float> OnInterstitialClose;
	private UnityAction OnInterstitialClicked;

	private void AdImplementation_applovin_interstitial()
	{
		MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnAdLoadFailedEvent_interstitial;
		MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnAdHiddenEvent_interstitial;
		MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnAdDisplayFailedEvent_interstitial;
		MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnAdClickedEvent_interstitial;
	}

    private void OnAdClickedEvent_interstitial(string adUnitId, MaxSdkBase.AdInfo info)
	{
		OnInterstitialClicked?.Invoke();
	}

    private void OnAdLoadFailedEvent_interstitial(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
	{
		Debug.LogError($"[applovin] load interstitial fail, errorInfo={errorInfo}");
	}
	
	private void OnAdHiddenEvent_interstitial(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		OnInterstitialClose?.Invoke(AdValueToUsd(adInfo));
		LoadInterstitial();
	}
	
	private void OnAdDisplayFailedEvent_interstitial(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
	{
		OnInterstitialClose?.Invoke(AdValueToUsd(adInfo));
		LoadInterstitial();
	}

	private void LoadInterstitial()
	{
		var unitId = GetAdUnitID_interstitial();
		if (string.IsNullOrEmpty(unitId))
		{
			return;
		}
		
		MaxSdk.LoadInterstitial(unitId);
	}
	
	public bool ShowInterstitial(UnityAction<float> callbackClose, UnityAction callbackClick)
	{
		var unitId = GetAdUnitID_interstitial();
		if (!string.IsNullOrEmpty(unitId) && MaxSdk.IsInterstitialReady(unitId))
		{
			OnInterstitialClose = callbackClose;
			OnInterstitialClicked = callbackClick;
			MaxSdk.ShowInterstitial(unitId);
			return true;
		}
		else
		{
			callbackClose?.Invoke(0);
			return false;
		}
	}
}

#endif
#if USE_APPLOVIN
using UnityEngine;
using UnityEngine.Events;

public partial class AdImplementation_applovin
{
	private UnityAction<bool> OnRewardedClose;
	private UnityAction<float> OnRewardedGrant;
	private UnityAction OnRewardedClicked;
	private bool rewardedAdHasRewardedUser;
	
	private void AdImplementation_applovin_rewarded()
	{
		MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnAdLoadFailedEvent_rewarded;
		MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnAdHiddenEvent_rewarded;
		MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnAdDisplayFailedEvent_rewarded;
		MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnAdReceivedRewardEvent_rewarded;
		MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnAdClickedEvent_rewarded;
	}

    private void OnAdClickedEvent_rewarded(string adUnitId, MaxSdkBase.AdInfo info)
	{
		OnRewardedClicked?.Invoke();
	}

    private void OnAdLoadFailedEvent_rewarded(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
	{
		Debug.LogError($"[applovin] load rewarded fail, errorInfo={errorInfo}");
	}

	private void OnAdHiddenEvent_rewarded(string adUnitId, MaxSdkBase.AdInfo adInfo)
	{
		OnRewardedClose?.Invoke(!rewardedAdHasRewardedUser);
		LoadRewarded();
	}

	private void OnAdDisplayFailedEvent_rewarded(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
	{
		OnRewardedClose?.Invoke(false);
		LoadRewarded();
	}

	private void OnAdReceivedRewardEvent_rewarded(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
	{
		rewardedAdHasRewardedUser = true;
		OnRewardedGrant?.Invoke(AdValueToUsd(adInfo));
	}

	private void LoadRewarded()
	{
		var unitId = GetAdUnitID_rewardedVideo();
		if (string.IsNullOrEmpty(unitId))
		{
			return;
		}

		MaxSdk.LoadRewardedAd(unitId);
	}
	
	public bool ShowRewarded(UnityAction<float> callbackReward, UnityAction<bool> callbackClose, UnityAction callbackClick)
	{
		var unitId = GetAdUnitID_rewardedVideo();
		if (!string.IsNullOrEmpty(unitId) && MaxSdk.IsRewardedAdReady(unitId))
		{
			OnRewardedClose = callbackClose;
			OnRewardedGrant = callbackReward;
			OnRewardedClicked = callbackClick;
			rewardedAdHasRewardedUser = false;
			MaxSdk.ShowRewardedAd(unitId);
			return true;
		}
		else
		{
			callbackClose?.Invoke(false);
			return false;
		}
	}
}
#endif
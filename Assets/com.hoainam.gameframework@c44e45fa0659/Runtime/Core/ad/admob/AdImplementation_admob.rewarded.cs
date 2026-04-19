
#if USE_ADMOB

using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

public partial class AdImplementation_admob
{
	private RewardedAd rewardedAd;
	private UnityAction<bool> onRewardedAdClosed;
	private float rewardedAdUsd;
	
	//remember that even if user has cancelled, revenue still be paid,
	//so don't use OnAdPaid event here.
	private bool rewardedAdHasRewardedUser;

	private void LoadRewarded()
	{
		var unitId = GetAdUnitID_rewardedVideo();
		if (string.IsNullOrEmpty(unitId))
		{
			return;
		}

		if (rewardedAd != null)
		{
			rewardedAd.Destroy();
			rewardedAd = null;
		}

		RewardedAd.Load(unitId, new AdRequest(), (ad, error) =>
		{
			if (error != null || ad == null)
			{
				Debug.LogError($"[admob] load rewarded fail: {error}");
			}
			else
			{
				rewardedAd = ad;

				ad.OnAdFullScreenContentClosed += () =>
				{
					onRewardedAdClosed?.Invoke(!rewardedAdHasRewardedUser);
					LoadRewarded();
				};

				ad.OnAdFullScreenContentFailed += _ =>
				{
					onRewardedAdClosed?.Invoke(false);
					LoadRewarded();
				};

				ad.OnAdPaid += adVal =>
				{
					rewardedAdUsd = AdValueToUsd(adVal);
				};
			}
		});
	}

	public bool ShowRewarded(UnityAction<float> callbackReward, UnityAction<bool> callbackClose)
	{
		if (rewardedAd != null && rewardedAd.CanShowAd())
		{
			rewardedAdUsd = 0;
			onRewardedAdClosed = callbackClose;
			rewardedAdHasRewardedUser = false;
			rewardedAd.Show(_ =>
			{
				rewardedAdHasRewardedUser = true;
				callbackReward?.Invoke(rewardedAdUsd);
			});

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
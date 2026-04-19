
#if USE_ADMOB

using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

public partial class AdImplementation_admob
{
	private InterstitialAd interstitialAd;
	private UnityAction<float> onInterstitialAdClosed;
	private float interstitialAdUsd;

	private void LoadInterstitial()
	{
		var unitId = GetAdUnitID_interstitial();
		if (string.IsNullOrEmpty(unitId))
		{
			return;
		}

		if (interstitialAd != null)
		{
			interstitialAd.Destroy();
			interstitialAd = null;
		}

		InterstitialAd.Load(unitId, new AdRequest(), (ad, error) =>
		{
			if (error != null || ad == null)
			{
				Debug.LogError($"[admob] load interstitial fail: {error}");
			}
			else
			{
				interstitialAd = ad;

				ad.OnAdFullScreenContentClosed += () =>
				{
					onInterstitialAdClosed?.Invoke(interstitialAdUsd);
					LoadInterstitial();
				};
				ad.OnAdFullScreenContentFailed += _ =>
				{
					onInterstitialAdClosed?.Invoke(interstitialAdUsd);
					LoadInterstitial();
				};
				ad.OnAdPaid += adVal =>
				{
					interstitialAdUsd = AdValueToUsd(adVal);
				};
			}
		});
	}

	public bool ShowInterstitial(UnityAction<float> callbackClose)
	{
		if (interstitialAd != null && interstitialAd.CanShowAd())
		{
			interstitialAdUsd = 0;
			onInterstitialAdClosed = callbackClose;
			interstitialAd.Show();
			
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
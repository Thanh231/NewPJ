
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class AdController : SingletonMonoBehaviour<AdController>
{
	#region core

	[SerializeField] GameObject coverObj;

	private IAdImplementation impl = null;

	public async UniTask Initialize()
	{
		if (impl != null)
		{
			return;
		}

#if UNITY_EDITOR || UNITY_STANDALONE
		impl = new AdImplementation_dummy();
#elif USE_ADMOB
		impl = new AdImplementation_admob();
#elif USE_APPLOVIN
        impl = new AdImplementation_applovin();
#endif
		
		//the reason await: check applovin implementation.
		await impl.Initialize();
	}

	#endregion

	#region ad

	public void ShowBanner()
	{
		impl.ShowBanner();
	}

	public void HideBanner()
	{
		impl.HideBanner();
	}

	/// <summary>
	/// show interstitial ad
	/// </summary>
	/// <param name="callbackClose">return how many USD gain when view ad</param>
	/// <returns>can show ad or not</returns>
	public bool ShowInterstitial(UnityAction<float> callbackClose)
	{
		coverObj.SetActive(true);
		var canShowAd = impl.ShowInterstitial(callbackClose: usd =>
		{
			coverObj.SetActive(false);
			callbackClose?.Invoke(usd);
			
#if USE_ADJUST_LOGGING
			AdjustLoggingController.instance.LogAd_rewarded(usd);
#endif
		}, () =>
		{
			#if USE_ADJUST_LOGGING
			AdjustLoggingController.instance.LogAd_clicked();
			#endif
		});

		if (canShowAd)
		{
			#if USE_ADJUST_LOGGING
			AdjustLoggingController.instance.LogAd_impression();
			#endif
		}

		return canShowAd;
	}

	/// <summary>
	/// show rewarded ad
	/// </summary>
	/// <param name="callbackReward">return how many USD gain when view ad</param>
	/// <param name="callbackCancel">called when user cancel ads</param>
	/// <returns>can show ad or not</returns>
	public bool ShowRewarded(UnityAction<float> callbackReward, UnityAction callbackCancel = null)
	{
		coverObj.SetActive(true);
		var canShowAd = impl.ShowRewarded(usd =>
		{
			callbackReward?.Invoke(usd);
#if USE_ADJUST_LOGGING
			AdjustLoggingController.instance.LogAd_rewarded(usd);
#endif
		}, hasCancelled =>
		{
			coverObj.SetActive(false);
			if (hasCancelled)
			{
				callbackCancel?.Invoke();
			}
		}, () =>
		{
			#if USE_ADJUST_LOGGING
			AdjustLoggingController.instance.LogAd_clicked();
			#endif
		});

		if (canShowAd)
		{
			#if USE_ADJUST_LOGGING
			AdjustLoggingController.instance.LogAd_impression();
			#endif
		}

		return canShowAd;
	}

	#endregion

	#region UMP

	public bool IsEu()
	{
		return impl.IsEu();
	}

	public void ShowUMPPopup_setting()
	{
		impl.ShowUMPPopup_setting();
	}

	#endregion

	#region log

	public string AdNetworkNameForAdjustLogging => impl.AdNetworkNameForAdjustLogging;
	public string AdNetworkNameForFirebaseLogging => impl.AdNetworkNameForFirebaseLogging;

	#endregion
}
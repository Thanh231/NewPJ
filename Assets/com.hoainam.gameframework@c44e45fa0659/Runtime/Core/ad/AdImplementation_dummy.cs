
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public class AdImplementation_dummy : IAdImplementation
{
	public async UniTask Initialize()
	{
		await UniTask.CompletedTask;
	}
	
	#region ad
	
	public void ShowBanner()
	{
	}
	
	public void HideBanner()
	{
	}
	
	public bool ShowInterstitial(UnityAction<float> callbackClose, UnityAction callbackClick)
	{
		callbackClose?.Invoke(0);
		return true;
	}

	public bool ShowRewarded(UnityAction<float> callbackReward, UnityAction<bool> callbackClose, UnityAction callbackClick)
	{
		callbackReward?.Invoke(0);
		callbackClose?.Invoke(false);
		return true;
	}

	#endregion

	#region UMP

	public bool IsEu()
	{
		return false;
	}
	
	public void ShowUMPPopup_setting()
	{
	}

	#endregion

	#region log

	public string AdNetworkNameForAdjustLogging => "dummy";
	public string AdNetworkNameForFirebaseLogging => "dummy";

	#endregion
}
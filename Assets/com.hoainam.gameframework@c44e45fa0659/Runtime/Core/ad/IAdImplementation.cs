
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public interface IAdImplementation
{
	UniTask Initialize();
	
	#region ad

	void ShowBanner();
	void HideBanner();

	/// <summary>
	/// show interstitial ad
	/// </summary>
	/// <param name="callbackClose">return how many USD gain when view ad</param>
	/// <returns>can show ad or not</returns>
	bool ShowInterstitial(UnityAction<float> callbackClose, UnityAction callbackClick);

	/// <summary>
	/// show rewarded ad
	/// </summary>
	/// <param name="callbackReward">return how many USD gain when view ad</param>
	/// <param name="callbackClose">return if user has cancelled ads</param>
	/// <returns>can show ad or not</returns>
	bool ShowRewarded(UnityAction<float> callbackReward, UnityAction<bool> callbackClose, UnityAction callbackClick);

	#endregion

	#region UMP

	bool IsEu();
	void ShowUMPPopup_setting();

	#endregion

	#region log

	string AdNetworkNameForAdjustLogging { get; }
	string AdNetworkNameForFirebaseLogging { get; }

	#endregion
}
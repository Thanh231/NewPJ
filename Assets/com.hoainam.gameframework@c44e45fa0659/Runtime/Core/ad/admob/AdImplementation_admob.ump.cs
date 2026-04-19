
#if USE_ADMOB

using System.Collections.Generic;
using GoogleMobileAds.Ump.Api;
using UnityEngine;
using UnityEngine.Events;

public partial class AdImplementation_admob
{
	private UnityAction<bool> callbackShowUMPPopupStartup;

	private void ShowUMPPopup_startUp(UnityAction<bool> callback)
	{
		callbackShowUMPPopupStartup = callback;
		var request = new ConsentRequestParameters();

		var testDevice = GameFrameworkConfig.instance.admobUMPTestingDevice;
		if (!string.IsNullOrEmpty(testDevice))
		{
			request.ConsentDebugSettings = new ConsentDebugSettings()
			{
				TestDeviceHashedIds = new List<string> { testDevice },
			};
		}

		ConsentInformation.Update(request, OnConsentInfoUpdated);
	}

	private void OnConsentInfoUpdated(FormError consentError)
	{
		if (consentError != null)
		{
			Debug.LogError($"[UMP] ConsentInformation.Update has error: {consentError.Message}");
			callbackShowUMPPopupStartup?.Invoke(false);
			return;
		}

		ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
		{
			if (formError != null)
			{
				Debug.LogError($"[UMP] ConsentForm.LoadAndShowConsentFormIfRequired has error: {formError.Message}");
				callbackShowUMPPopupStartup?.Invoke(false);
				return;
			}

			callbackShowUMPPopupStartup?.Invoke(ConsentInformation.CanRequestAds());
		});
	}

	public bool IsEu()
	{
		return ConsentInformation.PrivacyOptionsRequirementStatus == PrivacyOptionsRequirementStatus.Required;
	}

	public void ShowUMPPopup_setting()
	{
		ConsentForm.ShowPrivacyOptionsForm((FormError showError) =>
		{
			if (showError != null)
			{
				Debug.LogError($"[UMP] ConsentForm.ShowPrivacyOptionsForm has error: {showError.Message}");
			}
		});
	}
}

#endif
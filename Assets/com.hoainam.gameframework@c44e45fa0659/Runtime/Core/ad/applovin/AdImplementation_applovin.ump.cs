#if USE_APPLOVIN
using Newtonsoft.Json;
using UnityEngine;

public partial class AdImplementation_applovin
{
	//ShowUMPPopup_startUp: enable it in IntegrationManager,
	//and it will be shown when MaxSdk.InitializeSdk is called.
	
	//if after enable tester, but GDPR popup still not be displayed,
	//please enable GDPR in admob dashboard.


	public bool IsEu()
	{
		return MaxSdk.GetSdkConfiguration().ConsentFlowUserGeography == MaxSdkBase.ConsentFlowUserGeography.Gdpr;
	}

	public void ShowUMPPopup_setting()
	{
		MaxSdk.CmpService.ShowCmpForExistingUser(error =>
		{
			if (error != null)
			{
				Debug.LogError($"[UMP] show CMP popup fail error={JsonConvert.SerializeObject(error)}");
			}
		});
	}
}
#endif
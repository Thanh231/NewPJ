
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SocialSignIn_google:ISocialSignIn
{
	private readonly string callbackTargetName;

	private string loginNativeToken;
	private string loginNativeErrMsg;

	// use new google login will got this issue:
	// https://stackoverflow.com/questions/71325279/missing-featurename-auth-api-credentials-begin-sign-in-version-6
	const bool useLegacyLogin = true;

	public SocialSignIn_google(string callbackTargetName)
	{
		this.callbackTargetName = callbackTargetName;
	}

	public async UniTask<object> SignIn()
	{
		StaticUtils.LogFramework("[GoogleSignIn] begin sign in");

		loginNativeToken = null;
		loginNativeErrMsg = null;

		using (var nativeClass = new AndroidNativeClass("com.ironygames.unitygooglesignin.GoogleLogin"))
		{
			var webClient = GameFrameworkConfig.instance.webClientId;
			var currentActivity = nativeClass.CurrentActivity;
			nativeClass.CallStatic("Login", currentActivity, webClient, useLegacyLogin, callbackTargetName,
				nameof(SocialSignInController.NativeCallback_loginSuccess), 
				nameof(SocialSignInController.NativeCallback_loginFail));
		}
		
		await UniTask.WaitUntil(() =>
			!string.IsNullOrEmpty(loginNativeToken) || !string.IsNullOrEmpty(loginNativeErrMsg));

		if (!string.IsNullOrEmpty(loginNativeToken))
		{
			StaticUtils.LogFramework($"[GoogleSignIn] success with token={loginNativeToken}");
			return loginNativeToken;
		}
		else
		{
			throw new System.Exception($"[GoogleSignIn] {loginNativeErrMsg}");
		}
	}

	public async UniTask SignOut()
	{
		await UniTask.CompletedTask;

		using (var nativeClass = new AndroidNativeClass("com.ironygames.unitygooglesignin.GoogleLogin"))
		{
			var webClient = GameFrameworkConfig.instance.webClientId;
			var currentActivity = nativeClass.CurrentActivity;
			nativeClass.CallStatic("Logout", currentActivity, webClient, useLegacyLogin);
		}
	}

	public void Update()
	{
	}

	public void NativeCallback_loginFail(string data)
	{
		loginNativeErrMsg = data;
	}

	public void NativeCallback_loginSuccess(string data)
	{
		loginNativeToken = data;
	}
}

using Cysharp.Threading.Tasks;
using UnityEngine;

public class SocialSignIn_dummy : ISocialSignIn
{
	public void NativeCallback_loginFail(string data)
	{
	}

	public void NativeCallback_loginSuccess(string data)
	{
	}

	public async UniTask<object> SignIn()
	{
		await UniTask.CompletedTask;
		return SystemInfo.deviceUniqueIdentifier;
	}

	public async UniTask SignOut()
	{
		await UniTask.CompletedTask;
	}

	public void Update()
	{
	}
}
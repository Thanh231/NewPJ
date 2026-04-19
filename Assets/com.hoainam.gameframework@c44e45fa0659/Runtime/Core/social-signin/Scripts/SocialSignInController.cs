
using Cysharp.Threading.Tasks;

public class SocialSignInController : SingletonMonoBehaviour<SocialSignInController>
{
	private ISocialSignIn impl;

	private void Update()
	{
		if (impl != null)
		{
			impl.Update();
		}
	}

	public async UniTask<object> SignIn(SocialSignInProviderType providerType)
	{
		impl = CreateLoginImpl(providerType);
		return await impl.SignIn();
	}

	public async UniTask SignOut(SocialSignInProviderType providerType)
	{
		impl = CreateLoginImpl(providerType);
		await impl.SignOut();
	}

	public void NativeCallback_loginSuccess(string data)
	{
		if (impl != null)
		{
			impl.NativeCallback_loginSuccess(data);
		}
	}

	public void NativeCallback_loginFail(string data)
	{
		if (impl != null)
		{
			impl.NativeCallback_loginFail(data);
		}
	}

	private ISocialSignIn CreateLoginImpl(SocialSignInProviderType providerType)
	{
		return providerType switch
		{
			SocialSignInProviderType.Dummy => new SocialSignIn_dummy(),
			SocialSignInProviderType.Google => new SocialSignIn_google(gameObject.name),
			SocialSignInProviderType.GoogleOnIos => new SocialSignIn_googleOnIos(gameObject.name),
			SocialSignInProviderType.Apple => new SocialSignIn_apple(),
			SocialSignInProviderType.Facebook => new SocialSignIn_facebook(),
			_ => null,
		};
	}
}
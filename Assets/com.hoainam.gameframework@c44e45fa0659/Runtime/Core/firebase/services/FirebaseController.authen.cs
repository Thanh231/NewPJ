
#if AUTHENTICATE_USE_FIREBASE

using System;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Firebase.Auth;

public partial class FirebaseController : IServer_authenticate
{
	#region sign in

	public async UniTask<ServerSignInResult> SignIn(SocialSignInProviderType providerType, bool linkAccount)
	{
		if (!linkAccount)
		{
			var user = FirebaseAuth.DefaultInstance.CurrentUser;
			if (user != null)
			{
				//on ios, firebase won't be cleared when reinstall app,
				//so if last time, user sign in with Google/apple/facebook,
				//when user enter game for the first time, user still be Google/apple/facebook user.
				if (providerType == SocialSignInProviderType.Anonymous && !user.IsAnonymous)
				{
					StaticUtils.LogFramework("[FirebaseAuthen] sign out due to: first enter game but not anonymous");
					FirebaseAuth.DefaultInstance.SignOut();
				}
				else
				{
					StaticUtils.LogFramework($"[FirebaseAuthen] cache user: {user.UserId}");
					return new ServerSignInResult(user.UserId, false, false);
				}
			}
		}
		
		StaticUtils.LogFramework($"[FirebaseAuthen] fresh sign in with type {providerType}");
		return await DoSignIn(providerType);
	}

	private async UniTask<ServerSignInResult> DoSignIn(SocialSignInProviderType providerType)
	{
		switch (providerType)
		{
			case SocialSignInProviderType.AppleOnAndroid:
				return await DoSignIn_appleOnAndroid();
			case SocialSignInProviderType.Anonymous:
				return await DoSignIn_anonymous();
		}
		
		var signResult = await SocialSignInController.instance.SignIn(providerType);

		return providerType switch
		{
			SocialSignInProviderType.Dummy => new ServerSignInResult((string)signResult, true, false),
			SocialSignInProviderType.Google => await DoSignIn_google(signResult),
			SocialSignInProviderType.GoogleOnIos => await DoSignIn_google(signResult),
			SocialSignInProviderType.Apple => await DoSignIn_apple(signResult),
			SocialSignInProviderType.Facebook => await DoSignIn_facebook(signResult),
			_ => null,
		};
	}

	private async UniTask<ServerSignInResult> DoSignIn_google(object socialSignInResult)
	{
		var idToken = socialSignInResult as string;
		var auth = FirebaseAuth.DefaultInstance;
		var credential = GoogleAuthProvider.GetCredential(idToken, null);
		var linkSuccess = await LinkCredential(credential);
		if (!linkSuccess)
		{
			StaticUtils.LogFramework("[FirebaseAuthen] link fail, will load another account");

			FirebaseAuth.DefaultInstance.SignOut();
			var firebaseSignInResult = await auth.SignInWithCredentialAsync(credential);
			return new ServerSignInResult(firebaseSignInResult.UserId, true, false);
		}
		else
		{
			StaticUtils.LogFramework("[FirebaseAuthen] link success, will keep current account");

			return new ServerSignInResult(FirebaseAuth.DefaultInstance.CurrentUser.UserId, false, true);
		}
	}

	private async UniTask<ServerSignInResult> DoSignIn_apple(object socialSignInResult)
	{
		var signInWithAppleResult = socialSignInResult as SocialSignIn_apple.SignInWithAppleResult;
		var auth = FirebaseAuth.DefaultInstance;
		var credential = OAuthProvider.GetCredential("apple.com",
			signInWithAppleResult.identityToken, signInWithAppleResult.rawNonce,
			signInWithAppleResult.authorizationCode);
		var linkSuccess = await LinkCredential(credential);
		if(!linkSuccess)
		{
			FirebaseAuth.DefaultInstance.SignOut();
			var firebaseSignInResult = await auth.SignInWithCredentialAsync(credential);
			return new ServerSignInResult(firebaseSignInResult.UserId, true, false);
		}
		else
		{
			return new ServerSignInResult(FirebaseAuth.DefaultInstance.CurrentUser.UserId, false, true);
		}
	}

	private async UniTask<ServerSignInResult> DoSignIn_facebook(object socialSignInResult)
	{
		var accessToken = socialSignInResult as string;
		var auth = FirebaseAuth.DefaultInstance;
		var credential = FacebookAuthProvider.GetCredential(accessToken);
		var linkSuccess = await LinkCredential(credential);
		if (!linkSuccess)
		{
			FirebaseAuth.DefaultInstance.SignOut();
			var firebaseSignInResult = await auth.SignInAndRetrieveDataWithCredentialAsync(credential);
			return new ServerSignInResult(firebaseSignInResult.User.UserId, true, false);
		}
		else
		{
			return new ServerSignInResult(FirebaseAuth.DefaultInstance.CurrentUser.UserId, false, true);
		}
	}

	private async UniTask<ServerSignInResult> DoSignIn_appleOnAndroid()
	{
		var providerData = new FederatedOAuthProviderData()
		{
			ProviderId = "apple.com"
		};
		var provider = new FederatedOAuthProvider();
		provider.SetProviderData(providerData);
		var linkSuccess = await LinkProvider(provider);
		if (!linkSuccess)
		{
			FirebaseAuth.DefaultInstance.SignOut();
			var auth = FirebaseAuth.DefaultInstance;
			var firebaseSignInResult = await auth.SignInWithProviderAsync(provider);
			return new ServerSignInResult(firebaseSignInResult.User.UserId, true, false);
		}
		else
		{
			return new ServerSignInResult(FirebaseAuth.DefaultInstance.CurrentUser.UserId, false, true);
		}
	}
	
	//when you clear data, re-signin game will create another UID
	private async UniTask<ServerSignInResult> DoSignIn_anonymous()
	{
		var auth = FirebaseAuth.DefaultInstance;
		var firebaseSignInResult = await auth.SignInAnonymouslyAsync();
		return new ServerSignInResult(firebaseSignInResult.User.UserId, false, false);
	}

	#endregion

	#region link account

	private async UniTask<bool> LinkCredential(Credential credential)
	{
		try
		{
			var user = FirebaseAuth.DefaultInstance.CurrentUser;
			await user.LinkWithCredentialAsync(credential);

			return true;
		}
		catch (Exception e)
		{
			if (e is not FirebaseAccountLinkException)
			{
				StaticUtils.RethrowException(e);
			}

			return false;
		}
	}

	private async UniTask<bool> LinkProvider(FederatedOAuthProvider provider)
	{
		try
		{
			var user = FirebaseAuth.DefaultInstance.CurrentUser;
			await  user.LinkWithProviderAsync(provider);
		
			return true;
		}
		catch (Exception e)
		{
			if (e is not FirebaseAccountLinkException)
			{
				StaticUtils.RethrowException(e);
			}

			return false;
		}
	}

	#endregion
	
	#region sign out
	
	public async UniTask SignOut(SocialSignInProviderType providerType)
	{
		FirebaseAuth.DefaultInstance.SignOut();
		await SocialSignInController.instance.SignOut(providerType);
	}
	
	#endregion

	#region delete account

	public async UniTask DeleteUser(SocialSignInProviderType providerType)
	{
		//before do any sensitive operations like delete user,
		//sign in first
		await DoSignIn(providerType);
		await DoDeleteUser();
	}

	private async UniTask DoDeleteUser()
	{
		var user = FirebaseAuth.DefaultInstance.CurrentUser;
		if (user != null)
		{
			await user.DeleteAsync();
		}
	}

	#endregion

	#region utils

	private string GetProviderName(FirebaseUser user)
	{
		if (user.IsAnonymous)
		{
			return "anonymous";
		}

		var count = user.ProviderData.Count();
		if (count == 0)
		{
			return "no_provider";
		}

		var sb = new StringBuilder();
		var idx = 0;
		foreach (var info in user.ProviderData)
		{
			sb.Append(info.ProviderId);
			if (idx < count - 1)
			{
				sb.Append(", ");
			}
			idx++;
		}

		return sb.ToString();
	}

	#endregion
}

#endif
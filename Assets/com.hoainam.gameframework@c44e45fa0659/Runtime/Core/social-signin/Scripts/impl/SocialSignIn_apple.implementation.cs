
#if USE_SIGNIN_WITH_APPLE

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public partial class SocialSignIn_apple
{
	#region core

	private IAppleAuthManager appleAuthManager;

	private void Constructor_implementation()
	{
		if (AppleAuthManager.IsCurrentPlatformSupported)
		{
			var deserializer = new PayloadDeserializer();
			appleAuthManager = new AppleAuthManager(deserializer);
		}
	}

	private void Update_implementation()
	{
		if (appleAuthManager != null)
		{
			appleAuthManager.Update();
		}
	}

	private async UniTask<SignInWithAppleResult> SignIn_implementation()
	{
		var rawNonce = GenerateRandomString(32);
		var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);
		var credential = await DoSignIn(nonce);
		var identityToken = Encoding.UTF8.GetString(credential.IdentityToken);
		var authorizationCode = Encoding.UTF8.GetString(credential.AuthorizationCode);
		return new SignInWithAppleResult()
		{
			identityToken = identityToken,
			authorizationCode = authorizationCode,
			rawNonce = rawNonce,
		};
	}

	private UniTask<IAppleIDCredential> DoSignIn(string nonce)
	{
		var utcs = new UniTaskCompletionSource<IAppleIDCredential>();
		var args = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName, nonce);
		appleAuthManager.LoginWithAppleId(args, credential => { utcs.TrySetResult(credential as IAppleIDCredential); },
			error =>
			{
				utcs.TrySetException(
					new Exception($"[SignInWithApple] sign in fail, error={JsonConvert.SerializeObject(error)}"));
			});
		return utcs.Task;
	}

	private async UniTask SignOut_implementation()
	{
		await UniTask.CompletedTask;
	}

	#endregion

	#region random nonce

	private static string GenerateRandomString(int length)
	{
		if (length <= 0)
		{
			throw new Exception("Expected nonce to have positive length");
		}

		const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
		var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
		var result = string.Empty;
		var remainingLength = length;

		var randomNumberHolder = new byte[1];
		while (remainingLength > 0)
		{
			var randomNumbers = new List<int>(16);
			for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
			{
				cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
				randomNumbers.Add(randomNumberHolder[0]);
			}

			for (var randomNumberIndex = 0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
			{
				if (remainingLength == 0)
				{
					break;
				}

				var randomNumber = randomNumbers[randomNumberIndex];
				if (randomNumber < charset.Length)
				{
					result += charset[randomNumber];
					remainingLength--;
				}
			}
		}

		return result;
	}

	private static string GenerateSHA256NonceFromRawNonce(string rawNonce)
	{
		var sha = new SHA256Managed();
		var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
		var hash = sha.ComputeHash(utf8RawNonce);

		var result = string.Empty;
		for (var i = 0; i < hash.Length; i++)
		{
			result += hash[i].ToString("x2");
		}

		return result;
	}

	#endregion
}

#endif
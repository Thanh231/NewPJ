
#if USE_FACEBOOK_LOGIN

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Facebook.Unity;

public partial class SocialSignIn_facebook
{
    private async UniTask<object> SignIn_implementation()
    {
        await InitFB();
        return await DoSignIn();
    }

    private UniTask<string> DoSignIn()
    {
        var utcs = new UniTaskCompletionSource<string>();

        var permission = new List<string>()
        {
            "public_profile"
        };
        FB.LogInWithReadPermissions(permission, result =>
        {
            if (FB.IsLoggedIn)
            {
                var token = AccessToken.CurrentAccessToken.TokenString;
                utcs.TrySetResult(token);
            }
            else
            {
                utcs.TrySetException(
                    new Exception($"[Facebook] fail to sign in\nError={result.Error}\nRawResult={result.RawResult}"));
            }
        });
        
        return utcs.Task;
    }
    
    private UniTask InitFB()
    {
        var utcs = new UniTaskCompletionSource();

        if (FB.IsInitialized)
        {
            utcs.TrySetResult();
        }
        else
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                {
                    FB.ActivateApp();
                    utcs.TrySetResult();
                }
                else
                {
                    utcs.TrySetException(new Exception("[Facebook] cannot initialize FB"));
                }
            });
        }
        
        return utcs.Task;
    }

    private async UniTask SignOut_implementation()
    {
        await InitFB();
        FB.LogOut();
    }
}

#endif
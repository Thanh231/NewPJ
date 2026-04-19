
#if UNITY_IOS || UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
using Cysharp.Threading.Tasks;

public class SocialSignIn_googleOnIos : ISocialSignIn
{
    private readonly string callbackTargetName;

    private string loginNativeToken;
    private string loginNativeErrMsg;
    
#if UNITY_IOS || UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void LoginGoogleInIos(string iosClientId, string gameObjectName, string successCallback, string failCallback);
#endif
    
    public SocialSignIn_googleOnIos(string callbackTargetName)
    {
        this.callbackTargetName = callbackTargetName;
    }
    
    public async UniTask<object> SignIn()
    {
        loginNativeToken = null;
        loginNativeErrMsg = null;
        
#if UNITY_IOS || UNITY_EDITOR
        var clientId = GameFrameworkConfig.instance.iosClientId;
        LoginGoogleInIos(clientId, callbackTargetName,
            nameof(SocialSignInController.NativeCallback_loginSuccess),
            nameof(SocialSignInController.NativeCallback_loginFail));
#endif
        
        await UniTask.WaitUntil(() =>
            !string.IsNullOrEmpty(loginNativeToken) || !string.IsNullOrEmpty(loginNativeErrMsg));

        if (!string.IsNullOrEmpty(loginNativeToken))
        {
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
    }

    public void Update()
    {
    }

    public void NativeCallback_loginSuccess(string data)
    {
        loginNativeToken = data;
    }

    public void NativeCallback_loginFail(string data)
    {
        loginNativeErrMsg = data;
    }
}
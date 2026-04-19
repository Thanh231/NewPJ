
using Cysharp.Threading.Tasks;

public partial class SocialSignIn_apple : ISocialSignIn
{
    public class SignInWithAppleResult
    {
        public string identityToken;
        public string authorizationCode;
        public string rawNonce;
    }

    public SocialSignIn_apple()
    {
#if USE_SIGNIN_WITH_APPLE
        Constructor_implementation();
#endif
    }
    
    public async UniTask<object> SignIn()
    {
#if USE_SIGNIN_WITH_APPLE
        return await SignIn_implementation();
#else
        return null;
#endif
    }

    public async UniTask SignOut()
    {
#if USE_SIGNIN_WITH_APPLE
        await SignOut_implementation();
#endif
    }

    public void Update()
    {
#if USE_SIGNIN_WITH_APPLE
        Update_implementation();
#endif
    }

    public void NativeCallback_loginSuccess(string data)
    {
    }

    public void NativeCallback_loginFail(string data)
    {
    }
}

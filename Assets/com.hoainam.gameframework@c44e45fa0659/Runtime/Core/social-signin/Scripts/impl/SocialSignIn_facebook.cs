
using Cysharp.Threading.Tasks;

public partial class SocialSignIn_facebook : ISocialSignIn
{
    public async UniTask<object> SignIn()
    {
#if USE_FACEBOOK_LOGIN
        return await SignIn_implementation();
#else
        return null;
#endif
    }

    public async UniTask SignOut()
    {
#if USE_FACEBOOK_LOGIN
        await SignOut_implementation();
#endif
    }

    public void Update()
    {
    }

    public void NativeCallback_loginSuccess(string data)
    {
    }

    public void NativeCallback_loginFail(string data)
    {
    }
}

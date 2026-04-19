
using Cysharp.Threading.Tasks;

public interface ISocialSignIn
{
	UniTask<object> SignIn();
	UniTask SignOut();
	void Update();

	void NativeCallback_loginSuccess(string data);
	void NativeCallback_loginFail(string data);
}
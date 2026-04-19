using Cysharp.Threading.Tasks;

public interface IServer_authenticate
{
    UniTask<ServerSignInResult> SignIn(SocialSignInProviderType providerType, bool linkAccount);
    UniTask SignOut(SocialSignInProviderType providerType);
}
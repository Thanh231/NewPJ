
using Cysharp.Threading.Tasks;

public interface INative
{
    void OpenStorePage();
    
    UniTask<string> GetDeviceId();
    void NativeCallback_getDeviceIdSuccess(string data);
    void NativeCallback_getDeviceIdFail(string data);

    string DecryptFacebookReferrerData(string cipher, string nonce);
    UniTask<string> GetInstallSource();
    void NativeCallback_getInstallSourceSuccess(string data);
    void NativeCallback_getInstallSourceFail(string data);

    float GetAppUsageMB();
    float GetSystemFreeMB();
}

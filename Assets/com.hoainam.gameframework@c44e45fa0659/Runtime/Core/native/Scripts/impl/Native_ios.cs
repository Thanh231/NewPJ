
#if UNITY_EDITOR || UNITY_IOS

using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;

public class Native_ios : INative
{
    [DllImport("__Internal")]
    private static extern void _NativeIos_OpenStorePage(string appId);
    
    public void OpenStorePage()
    {
        var appId = GameFrameworkConfig.instance.iosAppId;
        _NativeIos_OpenStorePage(appId);
    }

    public async UniTask<string> GetDeviceId()
    {
        await UniTask.CompletedTask;
        throw new Exception("on ios, there's no reliable way to get device id");
    }

    public void NativeCallback_getDeviceIdSuccess(string data)
    {
    }

    public void NativeCallback_getDeviceIdFail(string data)
    {
    }

    public string DecryptFacebookReferrerData(string cipher, string nonce)
    {
        return null;
    }

    public async UniTask<string> GetInstallSource()
    {
        await UniTask.CompletedTask;
        throw new Exception("cannot get install source in ios");
    }

    public void NativeCallback_getInstallSourceSuccess(string data)
    {
    }

    public void NativeCallback_getInstallSourceFail(string data)
    {
    }

    public float GetAppUsageMB()
    {
        throw new Exception("no need to get app memory usage in ios");
    }

    public float GetSystemFreeMB()
    {
        throw new Exception("no need to get system free memory in ios");
    }
}

#endif
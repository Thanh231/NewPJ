
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Native_editor : INative
{
    public void OpenStorePage()
    {
        
    }

    public async UniTask<string> GetDeviceId()
    {
        await UniTask.CompletedTask;
        return SystemInfo.deviceUniqueIdentifier;
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
        throw new Exception("cannot get install source in editor");
    }

    public void NativeCallback_getInstallSourceSuccess(string data)
    {
    }

    public void NativeCallback_getInstallSourceFail(string data)
    {
    }

    public float GetAppUsageMB()
    {
        throw new Exception("no need to get app memory usage in editor");
    }

    public float GetSystemFreeMB()
    {
        throw new Exception("no need to get system free memory in editor");
    }
}

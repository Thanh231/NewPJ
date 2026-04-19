
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Native_android : INative
{
    private readonly string controllerObjName;

    public Native_android(string controllerObjName)
    {
        this.controllerObjName = controllerObjName;
    }
    
    public void OpenStorePage()
    {
        using (var nativeClass = new AndroidNativeClass("com.hoainam.unitynativeandroid.UnityNativeUtils"))
        {
            var currentActivity = nativeClass.CurrentActivity;
            var packageName = Application.identifier;
            nativeClass.CallStatic("OpenStorePage", currentActivity, packageName);
        }
    }

    #region get device id
    
    private string deviceId;
    private string getDeviceIdError;

    public async UniTask<string> GetDeviceId()
    {
        deviceId = null;
        getDeviceIdError = null;

        using (var nativeClass = new AndroidNativeClass("com.hoainam.unitynativeandroid.UnityNativeUtils"))
        {
            var currentActivity = nativeClass.CurrentActivity;
            nativeClass.CallStatic("GetDeviceId", currentActivity, controllerObjName,
                nameof(NativeController.NativeCallback_getDeviceIdSuccess),
                nameof(NativeController.NativeCallback_getDeviceIdFail));
        }

        await UniTask.WaitUntil(() => !string.IsNullOrEmpty(deviceId) || !string.IsNullOrEmpty(getDeviceIdError));

        if (!string.IsNullOrEmpty(deviceId))
        {
            return deviceId;
        }
        else
        {
            throw new Exception($"[NativeAndroid] {getDeviceIdError}");
        }
    }

    public void NativeCallback_getDeviceIdSuccess(string data)
    {
        deviceId = data;
    }

    public void NativeCallback_getDeviceIdFail(string data)
    {
        getDeviceIdError = data;
    }

    #endregion

    #region get install source

    private string installSource;
    private string getInstallSourceError;
    
    public string DecryptFacebookReferrerData(string cipher, string nonce)
    {
        var key = GameFrameworkConfig.instance.facebookInstallReferrerDecryptKey;
        if (string.IsNullOrEmpty(key))
        {
            throw new Exception("need to configure facebookInstallReferrerDecryptKey in GameFrameworkConfig");
        }
        using (var nativeClass = new AndroidNativeClass("com.hoainam.unitynativeandroid.UnityNativeUtils"))
        {
            return nativeClass.CallStaticWithReturn<string>("DecryptFacebookReferrerData", key, cipher, nonce);
        }
    }
    
    public async UniTask<string> GetInstallSource()
    {
        installSource = null;
        getInstallSourceError = null;
        
        using (var nativeClass = new AndroidNativeClass("com.hoainam.unitynativeandroid.UnityNativeUtils"))
        {
            var currentActivity = nativeClass.CurrentActivity;
            nativeClass.CallStatic("GetInstallSource", currentActivity, controllerObjName,
                nameof(NativeController.NativeCallback_getInstallSourceSuccess),
                nameof(NativeController.NativeCallback_getInstallSourceFail));
        }

        await UniTask.WaitUntil(() => !string.IsNullOrEmpty(installSource) || !string.IsNullOrEmpty(getInstallSourceError));

        if (!string.IsNullOrEmpty(installSource))
        {
            return installSource;
        }
        else
        {
            throw new Exception($"[NativeAndroid] {getInstallSourceError}");
        }
    }

    public void NativeCallback_getInstallSourceSuccess(string data)
    {
        installSource = data;
    }

    public void NativeCallback_getInstallSourceFail(string data)
    {
        getInstallSourceError = data;
    }

    #endregion

    #region get memory info

    public float GetAppUsageMB()
    {
        using (var nativeClass = new AndroidNativeClass("com.hoainam.unitynativeandroid.UnityNativeUtils"))
        {
            return nativeClass.CallStaticWithReturn<float>("GetAppUsageMB");
        }
    }

    public float GetSystemFreeMB()
    {
        using (var nativeClass = new AndroidNativeClass("com.hoainam.unitynativeandroid.UnityNativeUtils"))
        {
            var currentActivity = nativeClass.CurrentActivity;
            return nativeClass.CallStaticWithReturn<float>("GetSystemFreeMB", currentActivity);
        }
    }

    #endregion
}

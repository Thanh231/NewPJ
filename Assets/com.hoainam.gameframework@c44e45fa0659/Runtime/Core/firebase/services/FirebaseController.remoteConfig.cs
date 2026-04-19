
#if USE_FIREBASE_REMOTE_CONFIG

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.RemoteConfig;
using UnityEngine;

public partial class FirebaseController
{
    #region core

    private Dictionary<string, object> defaultRemoteConfigValue;
    public bool isRemoteConfigReady { get; set; }

    private async UniTask InitRemoteConfig(Dictionary<string, object> defaultRemoteConfigValue)
    {
        this.defaultRemoteConfigValue = defaultRemoteConfigValue;
        try
        {
            await FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaultRemoteConfigValue);

#if UNITY_EDITOR
            var cacheTime = TimeSpan.Zero;
#else
            var cacheTime = TimeSpan.FromHours(12);
#endif
        
            await FirebaseRemoteConfig.DefaultInstance.FetchAsync(cacheTime);
            await FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        
        isRemoteConfigReady = true;
    }

    #endregion

    #region get values

    public bool GetRemoteConfigBool(string remoteConfigKey)
    {
        if (!defaultRemoteConfigValue.ContainsKey(remoteConfigKey))
        {
            throw new Exception(
                $"[Firebase] get remote config key {remoteConfigKey}, but it is not exist in default values.");
        }
        
        if (!isRemoteConfigReady)
        {
            Debug.LogException(new Exception(
                $"[Firebase] get remote config key {remoteConfigKey}, but remote config is not ready yet."));
        }

        return FirebaseRemoteConfig.DefaultInstance.GetValue(remoteConfigKey).BooleanValue;
    }
    
    public int GetRemoteConfigInt(string remoteConfigKey)
    {
        if (!defaultRemoteConfigValue.ContainsKey(remoteConfigKey))
        {
            throw new Exception(
                $"[Firebase] get remote config key {remoteConfigKey}, but it is not exist in default values.");
        }
        
        if (!isRemoteConfigReady)
        {
            Debug.LogException(new Exception(
                $"[Firebase] get remote config key {remoteConfigKey}, but remote config is not ready yet."));
        }
        
        return (int)FirebaseRemoteConfig.DefaultInstance.GetValue(remoteConfigKey).LongValue;
    }

    public long GetRemoteConfigLong(string remoteConfigKey)
    {
        if (!defaultRemoteConfigValue.ContainsKey(remoteConfigKey))
        {
            throw new Exception(
                $"[Firebase] get remote config key {remoteConfigKey}, but it is not exist in default values.");
        }
        
        if (!isRemoteConfigReady)
        {
            Debug.LogException(new Exception(
                $"[Firebase] get remote config key {remoteConfigKey}, but remote config is not ready yet."));
        }
        
        return FirebaseRemoteConfig.DefaultInstance.GetValue(remoteConfigKey).LongValue;
    }

    public float GetRemoteConfigFloat(string remoteConfigKey)
    {
        if (!defaultRemoteConfigValue.ContainsKey(remoteConfigKey))
        {
            throw new Exception(
                $"[Firebase] get remote config key {remoteConfigKey}, but it is not exist in default values.");
        }
        
        if (!isRemoteConfigReady)
        {
            Debug.LogException(new Exception(
                $"[Firebase] get remote config key {remoteConfigKey}, but remote config is not ready yet."));
        }
        
        return (float)FirebaseRemoteConfig.DefaultInstance.GetValue(remoteConfigKey).DoubleValue;
    }
    
    public double GetRemoteConfigDouble(string remoteConfigKey)
    {
        if (!defaultRemoteConfigValue.ContainsKey(remoteConfigKey))
        {
            throw new Exception(
                $"[Firebase] get remote config key {remoteConfigKey}, but it is not exist in default values.");
        }
        
        if (!isRemoteConfigReady)
        {
            Debug.LogException(new Exception(
                $"[Firebase] get remote config key {remoteConfigKey}, but remote config is not ready yet."));
        }
        
        return FirebaseRemoteConfig.DefaultInstance.GetValue(remoteConfigKey).DoubleValue;
    }
    
    public string GetRemoteConfigString(string remoteConfigKey)
    {
        if (!defaultRemoteConfigValue.ContainsKey(remoteConfigKey))
        {
            throw new Exception(
                $"[Firebase] get remote config key {remoteConfigKey}, but it is not exist in default values.");
        }
        
        if (!isRemoteConfigReady)
        {
            Debug.LogException(new Exception(
                $"[Firebase] get remote config key {remoteConfigKey}, but remote config is not ready yet."));
        }
        
        return FirebaseRemoteConfig.DefaultInstance.GetValue(remoteConfigKey).StringValue;
    }

    #endregion
}

#endif

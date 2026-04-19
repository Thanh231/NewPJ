
using UnityEngine;

public static class RemoteAssetsPath
{
    //never put this into a MonoBehaviour, because it 
    //will corrupt that MonoBehaviour on iOS.
    
#if UNITY_EDITOR
    public static string remoteAddressableRuntimePath = $"{Application.dataPath}/../RemoteAddressable";
#elif UNITY_STANDALONE
	public static string remoteAddressableRuntimePath = $"{Application.dataPath}/../../RemoteAddressable";
#else
	public static string remoteAddressableRuntimePath = $"{Application.persistentDataPath}/RemoteAddressable";
#endif

    public static string platformName
    {
        get
        {
#if UNITY_STANDALONE_WIN
            var platform = "StandaloneWindows64";
#elif UNITY_ANDROID
            var platform = "Android";
#elif UNITY_IOS
            var platform = "iOS";
#endif
            return platform;
        }
    }

    public const string assetFileName = "remotegroup_assets_all.bundle";

    public static string localAssetFolder => $"{remoteAddressableRuntimePath}/{platformName}";
}

using Cysharp.Threading.Tasks;

public partial class NativeController : SingletonMonoBehaviour<NativeController>
{
    private INative impl;

    protected override void Awake()
    {
        base.Awake();
        
#if UNITY_EDITOR || UNITY_STANDALONE
        impl = new Native_editor();
#elif UNITY_ANDROID
        impl = new Native_android(gameObject.name);
#elif UNITY_IOS
        impl = new Native_ios();
#endif
    }

    public void OpenStorePage()
    {
        impl.OpenStorePage();
    }

    #region get device id

    // cannot use SystemInfo.deviceUniqueIdentifier, because:
    // - on android, it will require permission READ_PHONE_STATE
    public async UniTask<string> GetDeviceId()
    {
        return await impl.GetDeviceId();
    }
    
    public void NativeCallback_getDeviceIdSuccess(string data)
    {
        impl.NativeCallback_getDeviceIdSuccess(data);
    }

    public void NativeCallback_getDeviceIdFail(string data)
    {
        impl.NativeCallback_getDeviceIdFail(data);
    }

    #endregion

    #region get memory info

    public float GetAppUsageMB()
    {
        return impl.GetAppUsageMB();
    }
    
    public float GetSystemFreeMB()
    {
        return impl.GetSystemFreeMB();
    }

    #endregion
}

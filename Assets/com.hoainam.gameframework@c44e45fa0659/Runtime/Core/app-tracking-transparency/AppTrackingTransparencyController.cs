
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif
using UnityEngine.Events;

public class AppTrackingTransparencyController : SingletonMonoBehaviour<AppTrackingTransparencyController>
{
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void RequestAppTrackingTransparencyPermission();
#endif

    private UnityAction callback;

    public void RequestPermission(UnityAction callback)
    {
        this.callback = callback;

#if UNITY_IOS
        RequestAppTrackingTransparencyPermission();
#else
        DoneRequestPermission("Authorized");
#endif
    }

    public void DoneRequestPermission(string result)
    {
        callback?.Invoke();
    }
}
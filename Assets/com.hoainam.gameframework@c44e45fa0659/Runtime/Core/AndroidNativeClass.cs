
using System;
using UnityEngine;

public class AndroidNativeClass : IDisposable
{
    private readonly AndroidJavaClass classUnityPlayer;
    private readonly AndroidJavaClass classNative;
    private readonly AndroidJavaObject objActivity;

    public AndroidJavaObject CurrentActivity => objActivity;

    public AndroidNativeClass(string className)
    {
        classUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        classNative = new AndroidJavaClass(className);
        objActivity = classUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    }

    public void CallStatic(string methodName, params object[] args)
    {
        classNative.CallStatic(methodName, args);
    }

    public T CallStaticWithReturn<T>(string methodName, params object[] args)
    {
        return classNative.CallStatic<T>(methodName, args);
    }

    public void Dispose()
    {
        classUnityPlayer.Dispose();
        classNative.Dispose();
        objActivity.Dispose();
    }
}

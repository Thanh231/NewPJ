using System;

public class SystemNotificationController : SingletonMonoBehaviour<SystemNotificationController>
{
    private ISystemNotificationImplementation impl;
    private ISystemNotificationListener listener;
    
    public void Init(ISystemNotificationListener listener)
    {
        this.listener = listener;
        
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || !USE_SYSTEM_NOTIFICATION
        impl = new SystemNotificationImplementation_editor();
#elif UNITY_ANDROID
		impl = new SystemNotificationImplementation_android();
#else
		impl = new SystemNotificationImplementation_ios();
#endif
        
        impl.CancelAllNotifications();
    }

    private void OnApplicationPause(bool pause)
    {
        if (impl == null)
        {
            return;
        }
        
        if (pause)
        {
            listener.OnSendingAllNotifications();
        }
        else
        {
            impl.CancelAllNotifications();
        }
    }

    private void OnApplicationQuit()
    {
        if (impl == null)
        {
            return;
        }

        impl.CancelAllNotifications();
        listener.OnSendingAllNotifications();
    }

    public void SendNotification(string titleKey, string msgKey, TimeSpan trigger)
    {
        impl.SendNotification(titleKey, msgKey, trigger);
    }

    public void SendNotification(string titleKey, object[] titleParams, string msgKey, object[] msgParams,
        TimeSpan trigger)
    {
        impl.SendNotification(titleKey, titleParams, msgKey, msgParams, trigger);
    }

    public void RequestPermission()
    {
        impl.RequestPermission();
    }
}
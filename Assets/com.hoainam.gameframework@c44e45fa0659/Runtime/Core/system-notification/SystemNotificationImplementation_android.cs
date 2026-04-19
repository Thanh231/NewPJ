#if (UNITY_EDITOR || UNITY_ANDROID) && USE_SYSTEM_NOTIFICATION
using System;
using Unity.Notifications.Android;
using UnityEngine.Android;

public class SystemNotificationImplementation_android : ISystemNotificationImplementation
{
	const string channelId = "default_channel";
	const string permission = "android.permission.POST_NOTIFICATIONS";

	public SystemNotificationImplementation_android()
	{
		var channel = new AndroidNotificationChannel(channelId, "default name", "default description", Importance.Default);
		AndroidNotificationCenter.RegisterNotificationChannel(channel);
	}

	public void RequestPermission()
	{
		if (!Permission.HasUserAuthorizedPermission(permission))
		{
			Permission.RequestUserPermission(permission);
		}
	}

	public void SendNotification(string titleKey, string msgKey, TimeSpan trigger)
	{
		SendNotification(titleKey, null, msgKey, null, trigger);
	}

	public void SendNotification(string titleKey, object[] titleParams, string msgKey, object[] msgParams, TimeSpan trigger)
	{
		var notification = new AndroidNotification()
		{
			FireTime = DateTime.Now.Add(trigger),
			SmallIcon = "icon_0",
			Style = NotificationStyle.BigTextStyle,
		};
		LocalizationController.instance.SetContentAndroidNotification(titleKey, titleParams, msgKey, msgParams,
			ref notification);
		AndroidNotificationCenter.SendNotification(notification, channelId);
	}

	public void CancelAllNotifications()
	{
		AndroidNotificationCenter.CancelAllNotifications();
	}
}
#endif
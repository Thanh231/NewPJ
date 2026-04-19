#if (UNITY_EDITOR || UNITY_IOS) && USE_SYSTEM_NOTIFICATION
using System;
using Unity.Notifications.iOS;

public class SystemNotificationImplementation_ios : ISystemNotificationImplementation
{
	public void RequestPermission()
	{
	}

	public void SendNotification(string titleKey, string msgKey, TimeSpan trigger)
	{
		SendNotification(titleKey, null, msgKey, null, trigger);
	}

	public void SendNotification(string titleKey, object[] titleParams, string msgKey, object[] msgParams, TimeSpan trigger)
	{
		var triggerProp = new iOSNotificationTimeIntervalTrigger()
		{
			TimeInterval = trigger,
			Repeats = false,
		};
		var notification = new iOSNotification()
		{
			Trigger = triggerProp,
			ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
			ShowInForeground = true,
		};
		LocalizationController.instance.SetContentIosNotification(titleKey, titleParams, msgKey, msgParams,
			notification);
		iOSNotificationCenter.ScheduleNotification(notification);
	}

	public void CancelAllNotifications()
	{
		iOSNotificationCenter.RemoveAllDeliveredNotifications();
		iOSNotificationCenter.RemoveAllScheduledNotifications();
	}
}
#endif
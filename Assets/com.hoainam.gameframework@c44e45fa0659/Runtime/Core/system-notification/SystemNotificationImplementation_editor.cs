
using System;

public class SystemNotificationImplementation_editor : ISystemNotificationImplementation
{
	public void RequestPermission()
	{
	}

	public void SendNotification(string titleKey, string msgKey, TimeSpan trigger)
	{
	}

	public void SendNotification(string titleKey, object[] titleParams, string msgKey, object[] msgParams, TimeSpan trigger)
	{
	}

	public void CancelAllNotifications()
	{
	}
}
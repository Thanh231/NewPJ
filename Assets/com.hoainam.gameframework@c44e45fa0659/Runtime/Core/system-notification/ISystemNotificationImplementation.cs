
using System;

public interface ISystemNotificationImplementation
{
	void RequestPermission();
	void SendNotification(string titleKey, string msgKey, TimeSpan trigger);
	void SendNotification(string titleKey, object[] titleParams, string msgKey, object[] msgParams, TimeSpan trigger);
	void CancelAllNotifications();
}
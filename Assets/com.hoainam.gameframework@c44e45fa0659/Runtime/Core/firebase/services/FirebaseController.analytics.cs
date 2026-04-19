
#if USE_FIREBASE_ANALYTICS

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class FirebaseController
{
	#region core

	private string analyticsUserId = "<not-set>";
	private string analyticsSessionId = Guid.NewGuid().ToString();

	public void LogEvent(string eventName)
	{
		if (!isFirebaseAvailable)
		{
			return;
		}

		UpdateProperties();
		FirebaseAnalytics.LogEvent(eventName);
		
		StaticUtils.LogFramework($"[FirebaseLog] log event: {eventName}");
	}

	public void LogEvent(string eventName, params Parameter[] parameters)
	{
		if (!isFirebaseAvailable)
		{
			return;
		}

		UpdateProperties();
		FirebaseAnalytics.LogEvent(eventName, parameters);

		//using reflection is heavy, so check first.
		if (GameFrameworkConfig.instance.enableLogFramework)
		{
			StaticUtils.LogFramework($"[FirebaseLog] begin log event: {eventName} >>>>>>>>>>>>>>>>>");
			foreach (var i in parameters)
			{
				var key = StaticUtils.GetProperty<string>(i, "Name");
				var val = StaticUtils.GetProperty<object>(i, "Value");
				StaticUtils.LogFramework($"{key}={val}");
			}
			StaticUtils.LogFramework($"[FirebaseLog] <<<<<<<<<<<<<<< end log event: {eventName}");
		}
	}

	private void UpdateProperties()
	{
		var properties = listener.GetUserProperties();
		foreach (var i in properties)
		{
			FirebaseAnalytics.SetUserProperty(i.Key, i.Value);
		}
	}

	public async UniTask SetAnalyticsUserId(string firebaseUserId)
	{
		analyticsUserId = firebaseUserId;
		
		await UniTask.WaitUntil(() => isFirebaseAvailable);
		
		FirebaseAnalytics.SetUserId(firebaseUserId);
	}

	#endregion

	#region event game performance
	
	private bool kickOffPerformanceEvent;

	private void AnalyticsAddDefaultRemoteCfg(Dictionary<string, object> dic)
	{
		dic.Add("event_game_performance_count_per_session", -1);
		dic.Add("event_game_performance_interval_by_minutes", -1);
	}

	public async UniTask KickOffEventGamePerformance(float delayInSeconds)
	{
		if (kickOffPerformanceEvent)
		{
			return;
		}
		kickOffPerformanceEvent = true;
		
		if (delayInSeconds > 0)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(delayInSeconds));
		}
		
		#if UNITY_EDITOR || !UNITY_ANDROID
		return;
		#endif
		
		#if USE_FIREBASE_REMOTE_CONFIG

		var logCount = GetRemoteConfigInt("event_game_performance_count_per_session");
		var logInterval = GetRemoteConfigInt("event_game_performance_interval_by_minutes");

		for (var i = 0; i < logCount; i++)
		{
			LogEvent_gamePerformance();
			await UniTask.Delay(TimeSpan.FromMinutes(logInterval));
		}
		
		#endif
	}

	public void LogEvent_gamePerformance()
	{
		var utcTime = StaticUtils.DateTimeToString(DateTime.UtcNow);
		var fps = StaticUtils.GetCurrentFPS();
		var appVersion = Application.version;
		var deviceModel = SystemInfo.deviceModel;
		var unityScene = SceneManager.GetActiveScene().name;
		
		var totalRAMInMB = SystemInfo.systemMemorySize;
		var freeRAMInMB = NativeController.instance.GetSystemFreeMB();
		var appUsedRAMInMB = NativeController.instance.GetAppUsageMB();

		LogEvent("game_performance",
			new Parameter("utc_time", utcTime),
			new Parameter("fps", fps),
			new Parameter("app_version", appVersion),
			new Parameter("device_model", deviceModel),
			new Parameter("user_id", analyticsUserId),
			new Parameter("session_id", analyticsSessionId),
			new Parameter("unity_scene", unityScene),

			new Parameter("total_ram_in_mb", totalRAMInMB),
			new Parameter("free_ram_in_mb", freeRAMInMB),
			new Parameter("app_used_ram_in_mb", appUsedRAMInMB));
	}

	#endregion
}

#endif
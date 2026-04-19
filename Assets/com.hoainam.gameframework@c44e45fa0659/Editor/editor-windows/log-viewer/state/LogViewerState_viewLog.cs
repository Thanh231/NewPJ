
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LogViewerState_viewLog:EditorWindowState
{
    public class CollapseLogFileItem
	{
		public LogFileItem logItem;
		public int count;
		public bool showStacktrace;

		public CollapseLogFileItem(LogFileItem logItem)
		{
			this.logItem = logItem;
			count = 1;
		}
	}

	private List<CollapseLogFileItem> logItems = new List<CollapseLogFileItem>();
	private bool showInfoLog = true;
	private bool showWarningLog = true;
	private bool showErrorLog = true;
	private int nInfoLogs;
	private int nWarningLogs;
	private int nErrorLogs;
	private Vector2 scrollPos;

	private GUIStyle evenInfoStyle = ColorBgStyle(94, 94, 94);
	private GUIStyle oddInfoStyle = ColorBgStyle(67, 67, 67);
	private GUIStyle evenWarningStyle = ColorBgStyle(128, 128, 0);
	private GUIStyle oddWarningStyle = ColorBgStyle(82, 82, 0);
	private GUIStyle evenErrorStyle = ColorBgStyle(188, 20, 20);
	private GUIStyle oddErrorStyle = ColorBgStyle(154, 12, 12);

	private static GUIStyle ColorBgStyle(int r, int g, int b)
	{
		var color = new Color(r / 255f, g / 255f, b / 255f, 1.0f);
		return new GUIStyle()
		{
			normal = new GUIStyleState()
			{
				background = StaticUtils.CreateColorTexture(color)
			}
		};
	}
	
	public LogViewerState_viewLog(string logPath)
	{
		var items = LogFileReader.Read(logPath);
		foreach (var i in items)
		{
			if (logItems.Count == 0)
			{
				logItems.Add(new CollapseLogFileItem(i));
			}
			else
			{
				var lastItem = logItems[logItems.Count - 1];
				if (lastItem.logItem.Equals(i))
				{
					lastItem.count++;
				}
				else
				{
					logItems.Add(new CollapseLogFileItem(i));
				}
			}
		}

		nInfoLogs = GetLogCount(new List<LogType>() { LogType.Log, LogType.Assert }, items);
		nWarningLogs = GetLogCount(new List<LogType>() { LogType.Warning }, items);
		nErrorLogs = GetLogCount(new List<LogType>() { LogType.Error, LogType.Exception }, items);
	}

	public override void OnDraw()
	{
		EditorGUILayout.BeginHorizontal();
		showInfoLog = EditorGUILayoutExtension.FitToggle($"show info ({nInfoLogs})", showInfoLog);
		showWarningLog = EditorGUILayoutExtension.FitToggle($"show warning ({nWarningLogs})", showWarningLog);
		showErrorLog = EditorGUILayoutExtension.FitToggle($"show error ({nErrorLogs})", showErrorLog);
		EditorGUILayout.EndHorizontal();

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		var idx = 1;
		foreach (var i in logItems)
		{
			var logItem = i.logItem;
			if ((!showInfoLog && (logItem.logType == LogType.Log || logItem.logType == LogType.Assert)) ||
				(!showWarningLog && (logItem.logType == LogType.Warning)) ||
				(!showErrorLog && (logItem.logType == LogType.Error || logItem.logType == LogType.Exception)))
			{
				continue;
			}

			var style = GetBackgroundStyle(idx, logItem.logType);
			GUILayout.BeginHorizontal(style);
			if (!string.IsNullOrEmpty(logItem.stacktrace))
			{
				if (EditorGUILayoutExtension.FitButton(i.showStacktrace ? "hide" : "show"))
				{
					i.showStacktrace = !i.showStacktrace;
				}
			}
			var countText = i.count > 1 ? $"({i.count}) " : "";
			GUILayout.Label(i.showStacktrace ? $"{countText}{logItem.message}\n------------------\n{logItem.stacktrace}" : $"{countText}{logItem.message}");
			GUILayout.EndHorizontal();
			idx++;
		}

		EditorGUILayout.EndScrollView();
	}

	private GUIStyle GetBackgroundStyle(int idx, LogType logType)
	{
		if (idx % 2 == 0)
		{
			return logType switch
			{
				LogType.Log => oddInfoStyle,
				LogType.Assert => oddInfoStyle,
				LogType.Warning => evenWarningStyle,
				LogType.Error => evenErrorStyle,
				LogType.Exception => evenErrorStyle,
				_ => throw new Exception($"Unknown log type {logType}"),
			};
		}
		else
		{
			return logType switch
			{
				LogType.Log => evenInfoStyle,
				LogType.Assert => evenInfoStyle,
				LogType.Warning => oddWarningStyle,
				LogType.Error => oddErrorStyle,
				LogType.Exception => oddErrorStyle,
				_ => throw new Exception($"Unknown log type {logType}"),
			};
		}
	}

	private int GetLogCount(List<LogType> filter, List<LogFileItem> l)
	{
		var count = 0;
		foreach (var i in l)
		{
			if (filter.Contains(i.logType))
			{
				count++;
			}
		}
		return count;
	}
}

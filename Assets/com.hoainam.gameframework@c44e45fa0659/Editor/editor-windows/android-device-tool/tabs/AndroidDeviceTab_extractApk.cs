
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;

public partial class AndroidDeviceTab_extractApk : EditorTabInWindow.TabItemWindow
{
	private EditorListView listView;

	public override string tabText => "extract APK";

	public override void OnDraw()
	{
		if (GUILayout.Button("list apps"))
		{
			var packageNames = GetPackageNames();
			var l = new List<EditorListView.ListViewItem>();
			foreach (var i in packageNames)
			{
				GetAppInfo(i, out string appName);
				l.Add(new ListItem(i, appName));
			}
			listView = new EditorListView(l);
		}

		if (listView != null)
		{
			listView.Draw();
		}
	}

	private void GetAppInfo(string packageName, out string appName)
	{
		var folder = $"{StaticUtils.GetProjectPath()}/Temp/{packageName}";
		if (StaticUtils.CheckFolderExist(folder, isAbsolutePath: true))
		{
			appName = StaticUtils.ReadTextFile($"{folder}/appName.txt", isAbsolutePath: true);
		}
		else
		{
			StaticUtils.CreateFolder(folder, isAbsolutePath: true);
			var srcPath = GetBaseApkPath(packageName);
			var destPath = $"{folder}/base.apk";
			StaticUtilsEditor.RunBatchScript(StaticUtilsEditor.ADBFile,
				new List<string>() { "pull", srcPath, destPath });

			GetInfoFromApk(destPath, out appName, out string iconPath);
			StaticUtils.WriteTextFile($"{folder}/appName.txt", appName, isAbsolutePath: true);

			StaticUtilsEditor.RunBatchScript(StaticUtilsEditor._7zFile, 
				new List<string>() { "x", destPath, $"-o{folder}/base", "-y" });
			if (iconPath.EndsWith(".png"))
			{
				StaticUtils.CopyFile($"{folder}/base/{iconPath}", folder, isAbsolutePath: true);
			}
			else
			{
				var filename = Path.GetFileNameWithoutExtension(iconPath);
				var l = StaticUtils.GetFilesInFolder($"{folder}/base", true, $"{filename}.png", null, true);
				if (l.Count > 0)
				{
					StaticUtils.CopyFile(l[0], folder, isAbsolutePath: true);
				}
			}
		}
	}

	private void GetInfoFromApk(string apkPath, out string appName, out string iconPath)
	{
		var result = StaticUtilsEditor.RunBatchScript(StaticUtilsEditor.AAPTFile, 
			new List<string>() { "dump", "badging", apkPath });
		var lines = result.output.Split('\n');
		foreach (var i in lines)
		{
			if (i.StartsWith("application:"))
			{
				ParseAppNameAndIconPath(i, out appName, out iconPath);
				return;
			}
		}
		throw new Exception($"cannot find app name + icon in file {apkPath}, check info above");
	}

	private void ParseAppNameAndIconPath(string line, out string appName, out string iconPath)
	{
		var idx = line.IndexOf(' ') + 1;
		var dic = new Dictionary<string, string>();
		while (idx < line.Length)
		{
			var equalIdx = line.IndexOf('=', idx);
			var openIdx = equalIdx + 1;
			var closeIdx = line.IndexOf('\'', openIdx + 1);

			var key = line.Substring(idx, equalIdx - idx);
			var value = line.Substring(openIdx + 1, closeIdx - openIdx - 1);

			dic.Add(key, value);

			idx = closeIdx + 2;
		}
		appName = dic["label"];
		iconPath = dic["icon"];
	}

	private string GetBaseApkPath(string packageName)
	{
		var l = GetApkPath(packageName);
		foreach (var i in l)
		{
			if (i.EndsWith("base.apk"))
			{
				return i;
			}
		}
		throw new Exception($"{packageName} dont have base.apk, check log above");
	}

	public static List<string> GetApkPath(string packageName)
	{
		var result = StaticUtilsEditor.RunBatchScript(StaticUtilsEditor.ADBFile, 
			new List<string>() { "shell", "pm", "path", packageName });
		var lines = result.output.Split('\n');
		var paths = new List<string>();
		foreach (var i in lines)
		{
			var line = i.Trim();
			if (!string.IsNullOrEmpty(line))
			{
				var colonIdx = line.IndexOf(':');
				paths.Add(line.Substring(colonIdx + 1));
			}
		}
		return paths;
	}

	private List<string> GetPackageNames()
	{
		var result = StaticUtilsEditor.RunBatchScript(StaticUtilsEditor.ADBFile, 
			new List<string>() { "shell", "pm", "list", "packages", "-3" });
		var lines = result.output.Split('\n');
		var packageNames = new List<string>();
		foreach (var i in lines)
		{
			var line = i.Trim();
			if (!string.IsNullOrEmpty(line))
			{
				var colonIdx = line.IndexOf(':');
				var packageName = line.Substring(colonIdx + 1);
				packageNames.Add(packageName);
			}
		}
		return packageNames;
	}
}
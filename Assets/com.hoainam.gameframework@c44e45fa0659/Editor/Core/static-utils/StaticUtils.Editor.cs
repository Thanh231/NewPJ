
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System;
using System.IO;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine;

public static class StaticUtilsEditor
{
	#region other

	public static Vector2 CalculateTextSize(string text, EditorUIComponentType componentType)
	{
		var style = componentType switch
		{
			EditorUIComponentType.label => GUI.skin.label,
			EditorUIComponentType.button => GUI.skin.button,
			_ => null,
		};
		var sz = style.CalcSize(new GUIContent(text));
		return sz;
	}

	//path must be relative, like Assets/..........
	public static void ModifyAsset<T>(string path, UnityAction<T> callback) where T : UnityEngine.Object
	{
		var asset = AssetDatabase.LoadAssetAtPath<T>(path);
		callback?.Invoke(asset);

		AssetDatabase.SaveAssets();

		// Re-import font asset to get the new updated version.
		AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset));

		AssetDatabase.Refresh();
	}

	#endregion

	#region create game object

	public static RectTransform CreateUIGameObject(GameObject parent, string name)
	{
		return CreateGameObject<RectTransform>(parent, name);
	}

	public static T CreateGameObject<T>(GameObject parent, string name) where T : Component
	{
		var o = CreateGameObject(parent, name);
		var t = o.AddComponent<T>();
		return t;
	}

	private static GameObject CreateGameObject(GameObject parent, string name)
	{
		var o = new GameObject(name);
		Undo.RegisterCreatedObjectUndo(o, $"create game object - {name}");
		GameObjectUtility.SetParentAndAlign(o, parent);
		return o;
	}

	#endregion

	#region windows special path

	public static string ProgramFilesFolder => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
	public static string UnityBaseFolder => AppDomain.CurrentDomain.BaseDirectory;
	public static string UnityAndroidFolder => $"{UnityBaseFolder}/Data/PlaybackEngines/AndroidPlayer";
	public static string AndroidSdkFolder => $"{UnityAndroidFolder}/SDK";

	public static string UnityExeFile => $"\"{UnityBaseFolder}/Unity.exe\"";
	public static string ADBFile => $"\"{AndroidSdkFolder}/platform-tools/adb.exe\"";
	public static string SpineFile => $"\"{ProgramFilesFolder}/Spine/Spine.com\"";
	public static string _7zFile => $"\"{ProgramFilesFolder}/7-Zip/7z.exe\"";
	public static string JavaFile => $"\"{UnityAndroidFolder}/OpenJDK/bin/java.exe\"";
	public static string ShFile => $"\"{ProgramFilesFolder}/Git/bin/sh.exe\"";
	public static string JavaFromAndroidStudioFolder => $"{ProgramFilesFolder}/Android/Android Studio/jbr";

	public static string AAPTFile
	{
		get
		{
			var path = $"{AndroidSdkFolder}/build-tools";
			var l = Directory.GetDirectories(path);
			return $"\"{l[0]}/aapt.exe\"";
		}
	}

	public static string RandomATempPath(string fileExtension = null)
	{
		var projectPath = StaticUtils.GetProjectPath();
		var randomPath = FileUtil.GetUniqueTempPathInProject();
		var extension = string.IsNullOrEmpty(fileExtension) ? "" : $".{fileExtension}";
		return $"{projectPath}/{randomPath}{extension}";
	}

	#endregion

	#region open editor dialog

	public static void DisplayDialog(string msg)
	{
		EditorUtility.DisplayDialog("alert", msg, "ok");
	}

	public static string DisplayChooseFolderDialog(string title)
	{
		return EditorUtility.OpenFolderPanel(title, "", "");
	}

	public static string DisplayChooseFileDialog(string title, List<string> lExtension)
	{
		var extensionSB = new StringBuilder("");
		if (lExtension != null)
		{
			for (var i = 0; i < lExtension.Count; i++)
			{
				extensionSB.Append(lExtension[i]);
				if (i < lExtension.Count - 1)
				{
					extensionSB.Append(",");
				}
			}
		}

		return EditorUtility.OpenFilePanel(title, "", extensionSB.ToString());
	}

	public static bool DisplayConfirmDialog(string msg)
	{
		return EditorUtility.DisplayDialog("confirm", msg, "yes", "no");
	}

	#endregion

	#region native app

	//path must be relative, like Assets/..........
	public static void OpenScript(string path, int line)
	{
		var asset = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
		AssetDatabase.OpenAsset(asset, line);
	}
	
	public static void OpenFileExplorer(string path)
	{
		Process.Start("explorer.exe", path);
	}

	/// <summary>
	/// run a batch script file or run a windows program
	/// </summary>
	/// <param name="command">path to batch script file or path to program exe</param>
	/// <param name="args"></param>
	/// <param name="workingDir"></param>
	/// <returns></returns>
	public static RunBatchScriptOutput RunBatchScript(string command, List<string> args = null, string workingDir = null)
	{
		var sbCommand = new StringBuilder(command);
		
		var psi = new ProcessStartInfo()
		{
			FileName = command,
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
		};

		if (args != null)
		{
			var sbArgs = new StringBuilder();
			foreach (var arg in args)
			{
				sbArgs.Append($"{arg} ");
			}
			psi.Arguments = sbArgs.ToString();
			sbCommand.Append($" {psi.Arguments}");
		}

		if (workingDir != null)
		{
			psi.WorkingDirectory = workingDir;
		}

		UnityEngine.Debug.Log($"Executing command: {sbCommand}");
		
		var p = Process.Start(psi);
		var output = p.StandardOutput.ReadToEnd();
		var error = p.StandardError.ReadToEnd();
		p.WaitForExit();

		if (!string.IsNullOrEmpty(output))
		{
			UnityEngine.Debug.Log(output);
		}

		if (!string.IsNullOrEmpty(error))
		{
			UnityEngine.Debug.LogError(error);
		}

		return new RunBatchScriptOutput()
		{
			isSuccess = p.ExitCode == 0,
			output = output,
		};
	}

	public static bool RunShellScript(string scriptPath, List<string> args = null)
	{
		if (!StaticUtils.CheckFileExist(scriptPath, isAbsolutePath: true))
		{
			UnityEngine.Debug.LogError($"file {scriptPath} not found");
			return false;
		}

		var batchScriptArgs = new List<string>() { scriptPath };
		if (args != null)
		{
			batchScriptArgs.AddRange(args);
		}

		var result = RunBatchScript(ShFile, batchScriptArgs);
		return result.isSuccess;
	}

	#endregion
}
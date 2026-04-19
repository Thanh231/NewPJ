
using System.Collections.Generic;
using UnityEngine;

public class AndroidDeviceTab_installApk : EditorTabInWindow.TabItemWindow
{
	private readonly EditorFilePicker pickFile = new(new List<string>() { "apk", "aab" }, "pick an apk/aab:");

	public override string tabText => "install APK";

	public override void OnDraw()
	{
		pickFile.Draw();
		if (GUILayout.Button("install"))
		{
			var path = pickFile.PickedPath;
			var pathInCmd = pickFile.PickedPathInCmd;
			if (path.EndsWith(".apk"))
			{
				InstallApk(pathInCmd);
			}
			else
			{
				InstallAab(pathInCmd);
			}
			
			StaticUtilsEditor.DisplayDialog("install success");
		}
	}

	private void InstallApk(string path)
	{
		StaticUtilsEditor.RunBatchScript(StaticUtilsEditor.ADBFile, new List<string>()
		{
			"install",
			path
		});
	}

	private void InstallAab(string path)
	{
		var bundleToolPath = $"{StaticUtils.GetFrameworkPath()}/Editor/Core/Plugins/bundletool-all-1.18.1.jar";
		var outputApks = StaticUtilsEditor.RandomATempPath("apks");
		StaticUtilsEditor.RunBatchScript(StaticUtilsEditor.JavaFile, new List<string>()
		{
			"-jar",
			bundleToolPath,
			"build-apks",
			$"--bundle={path}",
			$"--output={outputApks}",
			"--mode=universal"
		});

		var unzipFolder = StaticUtilsEditor.RandomATempPath();
		StaticUtilsEditor.RunBatchScript(StaticUtilsEditor._7zFile, new List<string>()
		{
			"x", 
			outputApks, 
			$"-o{unzipFolder}", 
			"-y"
		});

		InstallApk($"{unzipFolder}/universal.apk");
	}
}
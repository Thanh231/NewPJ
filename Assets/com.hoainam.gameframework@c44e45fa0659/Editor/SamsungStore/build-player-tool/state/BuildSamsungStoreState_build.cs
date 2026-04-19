
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildSamsungStoreState_build : EditorWindowState
{
	#region ui

	private BuildEnvironment buildEnvironment = BuildEnvironment.Dev;
	private int buildNumber;

	public override void OnBeginState()
	{
		base.OnBeginState();

		buildNumber = PlayerSettings.Android.bundleVersionCode;
	}

	public override void OnDraw()
	{
		//choose build environment
		buildEnvironment = EditorGUILayoutExtension.EnumDropdownList("choose build environment:",
			BuildEnvironment.Num, buildEnvironment, out _);

		//choose build number
		if (buildEnvironment == BuildEnvironment.Live)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayoutExtension.FitLabelField("build number:");
			EditorGUILayout.LabelField(buildNumber.ToString(), GUI.skin.textField);
			if (EditorGUILayoutExtension.FitButton("increase"))
			{
				buildNumber++;
			}
			EditorGUILayout.EndHorizontal();
		}
		
		//build button
		if (GUILayout.Button("Build"))
		{
			PlayerSettings.Android.bundleVersionCode = buildNumber;
			EditorCoroutineUtility.StartCoroutineOwnerless(OnClickBuild());
		}
	}

	private IEnumerator OnClickBuild()
	{
		FSM.SwitchState(new EditorWindowState_doing());

		//delay for show loading scene
		//not use UniTask here because for some mysterious reason,
		//UniTask.delay not work before build
		yield return new EditorWaitForSeconds(2);

		var cache = new BuildSamsungStoreCacheConfiguration();
		cache.Set();

		var options = PrepareBuild();
		BuildPipeline.BuildPlayer(options);

		cache.Restore();

		var buildReport = GetBuildReport();
		FSM.SwitchState(new BuildPlayerState_report(buildReport));
	}

	#endregion

	#region build

	private BuildPlayerOptions PrepareBuild()
	{
		ConfigManager.PrepareConfigForBuild(isWindows: false);
		
		PlayerSettings.SplashScreen.showUnityLogo = false;
		
		var cfg = GameFrameworkConfig.instance;

		PlayerSettings.Android.keystorePass = cfg.androidKeystorePassword;
		PlayerSettings.Android.keyaliasPass = cfg.androidKeystorePassword;
		
		if (buildEnvironment == BuildEnvironment.Live)
		{
			PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
			PlayerSettings.Android.targetArchitectures = AndroidArchitecture.All;
		}
		else
		{
			PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.Mono2x);
			PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
		}
		EditorUserBuildSettings.buildAppBundle = false;

		var lScenes = new string[EditorBuildSettings.scenes.Length];
		for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
		{
			lScenes[i] = EditorBuildSettings.scenes[i].path;
		}
		
		var parent = "Builds/Android";
		var fileName = $"samsung-{Application.identifier}-{Application.version}-{PlayerSettings.Android.bundleVersionCode}";
		fileName = fileName.Replace('.', '_');

		return new BuildPlayerOptions()
		{
			scenes = lScenes,
			options = BuildOptions.None,
			target = BuildTarget.Android,
			targetGroup = BuildTargetGroup.Android,
			locationPathName = $"{parent}/{fileName}.apk",
			extraScriptingDefines = new[] { "SAMSUNG_STORE" },
		};
	}

	private BuildReport GetBuildReport()
	{
		var projPath = StaticUtils.GetProjectPath();
		StaticUtils.CopyFile(
			$"{projPath}/Library/LastBuild.buildreport",
			$"{projPath}/Assets/BuildReports", isAbsolutePath: true);

		var reportPath = "Assets/BuildReports/LastBuild.buildreport";
		AssetDatabase.ImportAsset(reportPath);
		return AssetDatabase.LoadAssetAtPath<BuildReport>(reportPath);
	}

	#endregion
}
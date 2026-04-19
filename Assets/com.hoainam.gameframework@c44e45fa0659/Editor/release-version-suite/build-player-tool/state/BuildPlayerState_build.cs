using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildPlayerState_build : EditorWindowState
{
	#region ui

	public override void OnDraw()
	{
		var platform = EditorUserBuildSettings.activeBuildTarget;
		EditorGUILayout.LabelField($"you're building platform: {platform}");

		OnDrawPlatform();

		if (GUILayout.Button("Build"))
		{
			EditorCoroutineUtility.StartCoroutineOwnerless(OnClickBuild());
		}
	}

	protected virtual void OnDrawPlatform()
	{
	}

	#endregion

	#region prepare build

	protected virtual BuildPlayerOptions PrepareBuild()
	{
		SetupConfig();
		return ConstructBuildOption();
	}

	private void SetupConfig()
	{
		var platform = EditorUserBuildSettings.activeBuildTarget;
		ConfigManager.PrepareConfigForBuild(platform == BuildTarget.StandaloneWindows64);

		PlayerSettings.SplashScreen.showUnityLogo = false;
	}

	private BuildPlayerOptions ConstructBuildOption()
	{
		var lScenes = new string[EditorBuildSettings.scenes.Length];
		for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
		{
			lScenes[i] = EditorBuildSettings.scenes[i].path;
		}

		var options = BuildOptions.None;
		
		return new BuildPlayerOptions()
		{
			scenes = lScenes,
			options = options,
		};
	}

	#endregion

	#region build

	private IEnumerator OnClickBuild()
	{
		FSM.SwitchState(new EditorWindowState_doing());

		//delay for show loading scene
		//not use UniTask here because for some mysterious reason,
		//UniTask.delay not work before build
		yield return new EditorWaitForSeconds(2);

		var options = PrepareBuild();
		BuildPipeline.BuildPlayer(options);

        var buildReport = GetBuildReport();
		FSM.SwitchState(new BuildPlayerState_report(buildReport));
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
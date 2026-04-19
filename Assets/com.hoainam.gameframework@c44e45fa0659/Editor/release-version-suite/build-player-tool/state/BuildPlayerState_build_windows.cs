
using UnityEditor;
using UnityEngine;

public class BuildPlayerState_build_windows: BuildPlayerState_build
{
	protected override BuildPlayerOptions PrepareBuild()
	{
		SetupConfig();
		return ConstructBuildOption(base.PrepareBuild());
	}

	private void SetupConfig()
	{
		PlayerSettings.runInBackground = true;
		PlayerSettings.fullScreenMode = FullScreenMode.Windowed;

		var sz = GameFrameworkConfig.instance.buildWindowSize;
		PlayerSettings.defaultScreenWidth = sz.x;
		PlayerSettings.defaultScreenHeight = sz.y;
	}

	private BuildPlayerOptions ConstructBuildOption(BuildPlayerOptions baseOption)
	{
		var packageName = Application.identifier;

		baseOption.locationPathName = $"Builds/Windows/{packageName}-{Application.version}/{packageName}.exe";
		baseOption.options |= BuildOptions.Development;
		baseOption.target = BuildTarget.StandaloneWindows64;
		baseOption.targetGroup = BuildTargetGroup.Standalone;

		return baseOption;
	}
}

using UnityEditor;

public class BuildPlayerState_build_mobile_ios : BuildPlayerState_build_mobile
{
	protected override int GetBuildNumber()
	{
		return StaticUtils.StringToInt(PlayerSettings.iOS.buildNumber);
	}

	protected override void SetBuildNumber(int buildNumber)
	{
		PlayerSettings.iOS.buildNumber = buildNumber.ToString();
	}

	protected override BuildPlayerOptions PrepareBuild()
	{
		var option = base.PrepareBuild();

		option.target = BuildTarget.iOS;
		option.targetGroup = BuildTargetGroup.iOS;
		option.locationPathName = "Builds/ios";

		if (buildEnvironment == BuildEnvironment.Dev)
		{
			option.options |= BuildOptions.Development;
		}

		return option;
	}
}
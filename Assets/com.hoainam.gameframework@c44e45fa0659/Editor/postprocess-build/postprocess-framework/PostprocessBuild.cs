
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public partial class PostprocessBuild : IPostprocessBuildWithReport
{
	public int callbackOrder => 0;

	public void OnPostprocessBuild(BuildReport report)
	{
		if (report.summary.result != BuildResult.Succeeded && report.summary.result != BuildResult.Unknown)
		{
			return;
		}

		var path = report.summary.outputPath;
		switch (report.summary.platform)
		{
			case BuildTarget.Android:
				OnPostprocessBuild_android(path);
				break;
			case BuildTarget.iOS:
				OnPostprocessBuild_ios(path);
				break;
		}
	}
}
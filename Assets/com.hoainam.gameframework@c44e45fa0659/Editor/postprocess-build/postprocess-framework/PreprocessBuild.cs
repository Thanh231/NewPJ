
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public partial class PreprocessBuild : IPreprocessBuildWithReport
{
	public int callbackOrder => 0;

	public void OnPreprocessBuild(BuildReport report)
	{
		switch (report.summary.platform)
		{
			case BuildTarget.Android:
				OnPreprocessBuild_android();
				break;
			case BuildTarget.iOS:
				OnPreprocessBuild_ios();
				break;
		}
	}
}

using UnityEditor;
using UnityEditor.Build.Reporting;

public class BuildPlayerState_report : EditorWindowState
{
	protected BuildReport buildReport;
	private string buildResultMsg;

	public BuildPlayerState_report(BuildReport buildReport)
	{
		this.buildReport = buildReport;
		var buildResult = buildReport.summary.result;
		buildResultMsg = $"build done with result: {buildResult}";
	}

	public override void OnDraw()
	{
		EditorGUILayout.LabelField(buildResultMsg);
	}
}
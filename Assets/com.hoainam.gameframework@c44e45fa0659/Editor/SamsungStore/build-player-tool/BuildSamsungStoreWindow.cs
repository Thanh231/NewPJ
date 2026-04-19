
using UnityEditor;

public class BuildSamsungStoreWindow : EditorWindowStateMachine
{
	[MenuItem("\u2726\u2726TOOLS\u2726\u2726/build samsung store")]
	static void OnMenuClicked()
	{
		EditorWindowState state = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android
			? new BuildSamsungStoreState_version()
			: new BuildSamsungStoreState_invalidPlatform();

		OpenWindow<BuildSamsungStoreWindow>(state);
	}
}
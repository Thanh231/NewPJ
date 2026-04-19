
using UnityEditor;

public class BuildPlayerWindow : EditorWindowStateMachine
{
	[MenuItem("\u2726\u2726TOOLS\u2726\u2726/\u2726\u2726RELEASE VERSION\u2726\u2726/build player")]
	static void OnMenuClicked()
	{
		OpenWindow<BuildPlayerWindow>(GetEntryState());
	}

	private static EditorWindowState GetEntryState()
	{
		#if !UNITY_EDITOR_WIN
		return new BuildPlayerState_version();
		#endif

		return GetSentBuildNote() ? new BuildPlayerState_version() : new BuildPlayerState_releaseType();
	}

	public static void GoToBuildState(EditorWindowState currentState)
	{
		EditorWindowState state = EditorUserBuildSettings.activeBuildTarget switch
		{
			BuildTarget.StandaloneWindows64 => new BuildPlayerState_build_windows(),
			BuildTarget.Android => new BuildPlayerState_build_mobile_android(),
			BuildTarget.iOS => new BuildPlayerState_build_mobile_ios(),
			_ => null
		};

		currentState.FSM.SwitchState(state);
	}

	public static bool GetSentBuildNote()
	{
		var path = $"{StaticUtils.GetProjectPath()}/Temp/sent_build_note_flag";
		return StaticUtils.CheckFileExist(path, true);
	}

	public static void SetSentBuildNote()
	{
		var path = $"{StaticUtils.GetProjectPath()}/Temp/sent_build_note_flag";
		StaticUtils.WriteTextFile(path, "hello", true);
	}
}
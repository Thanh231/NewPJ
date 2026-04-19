
using UnityEditor;

public class BuildLocalizationWindow : EditorWindowStateMachine
{
	[MenuItem("\u2726\u2726TOOLS\u2726\u2726/\u2726\u2726RELEASE VERSION\u2726\u2726/build localization")]
	static void OnMenuClicked()
	{
		OpenWindow<BuildLocalizationWindow>(new BuildLocalizationState_main());
	}
}
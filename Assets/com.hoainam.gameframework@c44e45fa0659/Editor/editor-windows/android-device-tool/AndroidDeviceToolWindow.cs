
using UnityEditor;

public class AndroidDeviceToolWindow: EditorWindowStateMachine
{
	[MenuItem("\u2726\u2726TOOLS\u2726\u2726/android device")]
	static void OnMenuClicked()
	{
		OpenWindow<AndroidDeviceToolWindow>(new AndroidDeviceToolState_devices());
	}
}
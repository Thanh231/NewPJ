
using UnityEditor;

public class AndroidManifestExtractorWindow: EditorWindowStateMachine
{
	[MenuItem("\u2726\u2726TOOLS\u2726\u2726/extract AndroidManifest.xml")]
	static void OnMenuClicked()
	{
		OpenWindow<AndroidManifestExtractorWindow>(new AndroidManifestExtractorMainState());
	}
}
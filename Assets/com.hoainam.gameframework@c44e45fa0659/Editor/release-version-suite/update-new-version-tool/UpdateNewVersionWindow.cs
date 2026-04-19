
using UnityEditor;

public class UpdateNewVersionWindow : EditorWindowStateMachine
{
    [MenuItem("\u2726\u2726TOOLS\u2726\u2726/\u2726\u2726RELEASE VERSION\u2726\u2726/update new version")]
    static void OnMenuClicked()
    {
        OpenWindow<UpdateNewVersionWindow>(new UpdateNewVersionState_main());
    }
}

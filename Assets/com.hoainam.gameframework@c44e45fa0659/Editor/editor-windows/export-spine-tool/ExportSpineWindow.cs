
using UnityEditor;

public class ExportSpineWindow:EditorWindowStateMachine
{
    [MenuItem("\u2726\u2726TOOLS\u2726\u2726/export spine")]
    static void OnMenuClicked()
    {
        OpenWindow<ExportSpineWindow>(new ExportSpineState_choosePath());
    }
}

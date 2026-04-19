
using UnityEditor;

public class LogViewerWindow:EditorWindowStateMachine
{
    [MenuItem("\u2726\u2726TOOLS\u2726\u2726/log viewer")]
    static void OnMenuClicked()
    {
        OpenWindow<LogViewerWindow>(new LogViewerState_selectLog());
    }
}

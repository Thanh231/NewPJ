using UnityEditor;

public class ProcessLogcatFileWindow : EditorWindowStateMachine
{
    [MenuItem("\u2726\u2726TOOLS\u2726\u2726/process logcat file")]
    static void OnMenuClicked()
    {
        OpenWindow<ProcessLogcatFileWindow>(new ProcessLogcatFileState_main());
    }
}
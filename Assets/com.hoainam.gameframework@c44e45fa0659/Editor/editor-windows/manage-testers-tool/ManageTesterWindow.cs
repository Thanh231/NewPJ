
using UnityEditor;

public class ManageTesterWindow : EditorWindowStateMachine
{
    [MenuItem("\u2726\u2726TOOLS\u2726\u2726/manage testers")]
    static void OnMenuClicked()
    {
        OpenWindow<ManageTesterWindow>(new ManageTesterState_start());
    }
}
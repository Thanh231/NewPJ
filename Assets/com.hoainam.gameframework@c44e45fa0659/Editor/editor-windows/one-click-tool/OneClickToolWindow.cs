
using UnityEditor;

public class OneClickToolWindow : EditorWindowStateMachine
{
    [MenuItem("\u2726\u2726TOOLS\u2726\u2726/one click tools")]
    static void OnMenuClicked()
    {
        OpenWindow<OneClickToolWindow>(new OneClickToolState_main());
    }
}
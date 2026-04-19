
using UnityEditor;

public class UploadCfgWindow : EditorWindowStateMachine
{
    [MenuItem("\u2726\u2726TOOLS\u2726\u2726/\u2726\u2726RELEASE VERSION\u2726\u2726/upload config")]
    static void OnMenuClicked()
    {
        OpenWindow<UploadCfgWindow>(new UploadCfgState_main());
    }
}

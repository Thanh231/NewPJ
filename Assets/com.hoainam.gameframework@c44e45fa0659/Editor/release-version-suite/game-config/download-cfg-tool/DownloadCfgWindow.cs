
using UnityEditor;

public class DownloadCfgWindow : EditorWindowStateMachine
{
    [MenuItem("\u2726\u2726TOOLS\u2726\u2726/download config")]
    static void OnMenuClicked()
    {
        OpenWindow<DownloadCfgWindow>(new DownloadCfgState_main());
    }
}


using UnityEditor;

public class BuildProjUploadAwsWindow : EditorWindowStateMachine
{
    [MenuItem("\u2726\u2726TOOLS\u2726\u2726/build project upload AWS")]
    static void OnMenuClicked()
    {
        OpenWindow<BuildProjUploadAwsWindow>(new BuildProjUploadAwsState_main());
    }
}

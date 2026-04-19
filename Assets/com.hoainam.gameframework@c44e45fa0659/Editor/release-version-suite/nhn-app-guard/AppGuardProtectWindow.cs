
#if USE_NHN_APP_GUARD

using UnityEditor;

public class AppGuardProtectWindow : EditorWindowStateMachine
{
    [MenuItem("\u2726\u2726TOOLS\u2726\u2726/\u2726\u2726RELEASE VERSION\u2726\u2726/app guard protector")]
    static void OnMenuClicked()
    {
        OpenWindow<AppGuardProtectWindow>(new AppGuardProtectStain_main());
    }
}

#endif

using UnityEditor;

public class ApplyLocalizedSystemWindow : EditorWindowStateMachine
{
    protected override bool windowUseUniTask => true;

    [MenuItem("\u2726\u2726TOOLS\u2726\u2726/apply localized system")]
    static void OnMenuClicked()
    {
        OpenWindow<ApplyLocalizedSystemWindow>(new ApplyLocalizedSystemState_main());
    }
}

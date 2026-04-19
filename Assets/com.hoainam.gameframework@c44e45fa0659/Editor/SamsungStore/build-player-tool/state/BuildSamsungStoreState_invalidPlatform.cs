
using UnityEditor;

public class BuildSamsungStoreState_invalidPlatform : EditorWindowState
{
    public override void OnDraw()
    {
        EditorGUILayout.LabelField("must switch to android platform to build");
    }
}

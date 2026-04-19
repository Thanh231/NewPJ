
using UnityEditor;

[CustomEditor(typeof(UIGrayscale))]
public class UIGrayscaleEditor:Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.HelpBox("make sure parent canvas enable Additional Shader Channels \"TexCoord3\"", MessageType.Warning);
    }
}

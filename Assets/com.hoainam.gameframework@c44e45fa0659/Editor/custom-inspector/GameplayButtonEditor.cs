
using UnityEditor;

[CustomEditor(typeof(GameplayButton))]
public class GameplayButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.HelpBox("this object need to have Collider & have layer mask \"GameplayButton\"",
            MessageType.Warning);
    }
}
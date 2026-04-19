
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectPool))]
public class ObjectPoolEditor : Editor
{
	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		serializedObject.UpdateIfRequiredOrScript();

		var prefabCfgsProp = serializedObject.FindProperty("prefabCfgs");
		for (var i = 0; i < prefabCfgsProp.arraySize; i++)
		{
			DrawItem(prefabCfgsProp.GetArrayElementAtIndex(i));
			EditorGUILayout.Space(3);
		}
		if (GUILayout.Button("Add"))
		{
			if (prefabCfgsProp.arraySize == 0)
			{
				prefabCfgsProp.InsertArrayElementAtIndex(0);
			}
			else
			{
				var lastItem = prefabCfgsProp.GetArrayElementAtIndex(prefabCfgsProp.arraySize - 1);
				lastItem.DuplicateCommand();
			}
		}
		
		serializedObject.ApplyModifiedProperties();
		EditorGUI.EndChangeCheck();
	}

	private void DrawItem(SerializedProperty item)
	{
		EditorGUILayout.BeginVertical(GUI.skin.box);

		EditorGUILayout.PropertyField(item.FindPropertyRelative("name"), includeChildren: false);

		var useAssetRefProp = item.FindPropertyRelative("useAssetRef");
		EditorGUILayout.PropertyField(useAssetRefProp, includeChildren: false);

		if (useAssetRefProp.boolValue)
		{
			EditorGUILayout.PropertyField(item.FindPropertyRelative("assetRef"), includeChildren: false);
		}
		else
		{
			EditorGUILayout.PropertyField(item.FindPropertyRelative("prefab"), includeChildren: false);
		}

		EditorGUILayout.PropertyField(item.FindPropertyRelative("preSpawnedAmount"), includeChildren: false);
		EditorGUILayout.PropertyField(item.FindPropertyRelative("lifeTimeInSecs"), includeChildren: false);

		if (EditorGUILayoutExtension.FitButton("Delete"))
		{
			item.DeleteCommand();
		}

		EditorGUILayout.EndVertical();
	}
}
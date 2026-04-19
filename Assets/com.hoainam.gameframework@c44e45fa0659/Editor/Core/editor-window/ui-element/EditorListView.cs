
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorListView
{
    public abstract class ListViewItem
    {
        public abstract void Draw();
    }

    private List<ListViewItem> items;
    private Vector2 scrollPos;
    private GUIStyle oddStyle = CreateItemStyle(new Color(0.5f, 0.5f, 0.5f, 0.0f));
    private GUIStyle evenStyle= CreateItemStyle(new Color(0.5f, 0.5f, 0.5f, 0.1f));

    public EditorListView(List<ListViewItem> items)
    {
        this.items = items;
    }

    private static GUIStyle CreateItemStyle(Color color)
    {
        var style = new GUIStyle(GUI.skin.label);
        style.normal.background = StaticUtils.CreateColorTexture(color);
        return style;
    }

    public void Draw()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (var i = 0; i < items.Count; i++)
        {
            EditorGUILayout.BeginHorizontal(i % 2 == 0 ? evenStyle : oddStyle);
            items[i].Draw();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }
}

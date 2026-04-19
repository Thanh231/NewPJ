
using UnityEditor;
using UnityEngine;

public static partial class EditorGUILayoutExtension
{
    public static void DrawLine(Vector2 p1, Vector2 p2, Color color, float thickness)
    {
        var oldColor = Handles.color;
        Handles.color = color;

        Handles.DrawLine(p1, p2, thickness);

        Handles.color = oldColor;
    }

    public static void DrawRectangle(Vector2 corner1, Vector2 corner2, Color color, float thickness)
    {
        var p1 = corner1;
        var p2 = new Vector2(corner2.x, corner1.y);
        var p3 = corner2;
        var p4 = new Vector2(corner1.x, corner2.y);

        DrawLine(p1, p2, color, thickness);
        DrawLine(p2, p3, color, thickness);
        DrawLine(p3, p4, color, thickness);
        DrawLine(p4, p1, color, thickness);
    }
}


using UnityEngine;

public static partial class StaticUtils
{
    public static void GizmosDrawBounds(Bounds bounds, Color color)
    {
        var min = bounds.min;
        var max = bounds.max;
        var p1 = new Vector2(min.x, min.y);
        var p2 = new Vector2(max.x, min.y);
        var p3 = new Vector2(max.x, max.y);
        var p4 = new Vector2(min.x, max.y);
        GizmosDrawLine(p1, p2, color);
        GizmosDrawLine(p2, p3, color);
        GizmosDrawLine(p3, p4, color);
        GizmosDrawLine(p4, p1, color);
    }
    
    public static void GizmosDrawRect(Rect rect, Color color)
    {
        var p1 = rect.position + 0.5f * new Vector2(-rect.width, -rect.height);
        var p2 = rect.position + 0.5f * new Vector2(-rect.width, rect.height);
        var p3 = rect.position + 0.5f * new Vector2(rect.width, rect.height);
        var p4 = rect.position + 0.5f * new Vector2(rect.width, -rect.height);
        GizmosDrawLine(p1, p2, color);
        GizmosDrawLine(p2, p3, color);
        GizmosDrawLine(p3, p4, color);
        GizmosDrawLine(p4, p1, color);
    }
    
    public static void GizmosDrawLine(Vector2 p1, Vector2 p2, Color color)
    {
        var cacheColor = Gizmos.color;
        Gizmos.color = color;
        
        Gizmos.DrawLine(p1, p2);
        
        Gizmos.color = cacheColor;
    }
}
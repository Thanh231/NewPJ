using UnityEditor;
using UnityEngine;

public class ImageToolTab_batchResize : BaseTab_batchResize
{
    private bool usePercentage = true;
    private float percentage = 100;
    private int newWidth = 100;
    private int newHeight = 100;
    
    public override string tabText => "batch resize";
    public override void OnDraw()
    {
        usePercentage = EditorGUILayout.Toggle("Use Percentage", usePercentage);
        if (usePercentage)
        {
            percentage = EditorGUILayout.FloatField("Percentage", percentage);
        }
        else
        {
            newWidth = EditorGUILayout.IntField("New Width:", newWidth);
            newHeight = EditorGUILayout.IntField("New Height:", newHeight);
        }

        base.OnDraw();
    }

    protected override Vector2Int GetNewSize(Texture2D texture)
    {
        if (usePercentage)
        {
            var t = percentage / 100f;
            return new Vector2Int((int)(t * texture.width), (int)(t * texture.height));
        }
        else
        {
            return new Vector2Int(newWidth, newHeight);
        }
    }
}


using UnityEngine;

public partial class LocalizedText_tmp
{
    private void StartEditor()
    {
        var mat = new Material(tmp.fontSharedMaterial);
        mat.shaderKeywords = tmp.fontSharedMaterial.shaderKeywords;
        tmp.fontMaterial = mat;
    }

    private void UpdateEditor()
    {
        tmp.fontMaterial.SetColor("_UnderlayColor", underlayColor);
    }
}

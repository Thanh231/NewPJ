
using TMPro;
using UnityEngine;

public class TextOutlineColorSetter : MonoBehaviour
{
    public Color outlineColor;

    private void Start()
    {
        var tmp = GetComponent<TextMeshProUGUI>();
        tmp.fontMaterial.SetColor("_UnderlayColor", outlineColor);
    }
}
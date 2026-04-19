using TMPro;
using UnityEngine;

public partial class TextNotify : MonoBehaviour
{
    public TextMeshProUGUI text;
    public LocalizedText_tmp localizedText;
    public RectTransform rectTransform;

    public void Setup(TextNotifyController.TextCfg textCfg, TextNotifyController.ColorCfg colorCfg,
        TextNotifyController.PositionCfg positionCfg, Camera mainCamera, string textDebug)
    {
        SetupText(textCfg, textDebug);
        SetupColor(colorCfg);
        SetupPosition(positionCfg, mainCamera);
        Setup_anim();
    }

    private void SetupText(TextNotifyController.TextCfg textCfg, string textDebug)
    {
        if (string.IsNullOrEmpty(textDebug))
        {
            localizedText.SetKeyAndParameters(textCfg.key, textCfg.lParams);
        }
        else
        {
            text.text = textDebug;
        }
    }

    private void SetupColor(TextNotifyController.ColorCfg colorCfg)
    {
        localizedText.ChangeUnderlayColor(colorCfg.outline);
        if (colorCfg is TextNotifyController.SolidColorCfg solidColorCfg)
        {
            text.enableVertexGradient = false;
            text.color = solidColorCfg.color;
        }
        else if (colorCfg is TextNotifyController.GradientColorCfg gradientColorCfg)
        {
            text.enableVertexGradient = true;
            text.color = Color.white;
            text.colorGradient = new VertexGradient(gradientColorCfg.colorUp, gradientColorCfg.colorUp,
                gradientColorCfg.colorDown, gradientColorCfg.colorDown);
        }
    }

    private void SetupPosition(TextNotifyController.PositionCfg positionCfg, Camera mainCamera)
    {
        if (positionCfg is TextNotifyController.CustomPositionCfg customPositionCfg)
        {
            if (customPositionCfg.inWorldSpace)
            {
                rectTransform.position = mainCamera.WorldToScreenPoint(customPositionCfg.position);
            }
            else
            {
                transform.position = customPositionCfg.position;
            }
            
            WrapPositionWithinScreen();
        }
        else
        {
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    private void WrapPositionWithinScreen()
    {
        var scrW = Screen.width / rectTransform.lossyScale.x;
        var objW = rectTransform.sizeDelta.x;
        var minX = -scrW / 2 + objW / 2;
        var maxX = scrW / 2 - objW / 2;

        var pos = rectTransform.anchoredPosition;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        rectTransform.anchoredPosition = pos;
    }
}

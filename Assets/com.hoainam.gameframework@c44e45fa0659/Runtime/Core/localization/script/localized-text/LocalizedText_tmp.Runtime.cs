
using System;
using R3;
using TMPro;
using UnityEngine;

public partial class LocalizedText_tmp
{
    private bool isInitializedFont;

    public Func<SystemLanguage, TMP_FontAsset> customGetFontFunc;
    
    private void StartRuntime()
    {
        LocalizationController.instance.languageRx
            .Subscribe(lang =>
            {
                var useCustomFont = SetFontForText(lang);
                LocalizationController.instance.SetupLocalizedText(this);
                if (!useCustomFont)
                {
                    LocalizationController.instance.SetUnderlayColorForTMP(tmp, underlayColor);
                }

                isInitializedFont = true;
            })
            .AddTo(this);
    }

    private bool SetFontForText(SystemLanguage lang)
    {
        var useCustomFont = false;
        if (customGetFontFunc != null)
        {
            var font = customGetFontFunc(lang);
            if (font != null)
            {
                useCustomFont = true;
                tmp.font = font;
            }
        }

        if (!useCustomFont)
        {
            tmp.font = LocalizationController.instance.fontAsset;
        }

        return useCustomFont;
    }

    public void ChangeUnderlayColor(Color color)
    {
        underlayColor = color;
        if (isInitializedFont)
        {
            //re-use font have error here,
            //need to check later
            tmp.fontMaterial.SetColor("_UnderlayColor", underlayColor);
        }
    }
}


using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class ScalePopupCfg
{
    public float scaleDuration = 0.3f;
    public Ease scaleInEase = Ease.OutBack;
    public Ease scaleOutEase = Ease.InBack;
}

public partial class GameFrameworkConfig
{
    [Header("popup")] 
    public ScalePopupCfg popupAnimScaleCfg;
    public string popupAddressablePath;
}


using System;
using DG.Tweening;
using UnityEngine;

public partial class TextNotify
{
    [Serializable]
    public class AnimCfg
    {
        public float appearDuration;
        public float standDuration;
        public float disappearDuration;
        public Ease appearEase;
    }
    
    public AnimCfg animCfg;

    public float AnimTime => animCfg.appearDuration + animCfg.standDuration + animCfg.disappearDuration;

    private void RunAnim()
    {
        RunAnim_scale();
        RunAnim_fade();
    }
    
    private void RunAnim_scale()
    {
        text.transform.localScale = Vector3.zero;
        text.transform.DOScale(Vector3.one, animCfg.appearDuration).SetEase(animCfg.appearEase);
    }

    private void RunAnim_fade()
    {
        var cacheColor = text.color;
        cacheColor.a = 0;
        text.color = cacheColor;
        var seq = DOTween.Sequence();
        seq.Append(text.DOFade(1, animCfg.appearDuration));
        seq.AppendInterval(animCfg.standDuration);
        seq.Append(text.DOFade(0, animCfg.disappearDuration));
    }

    private void Setup_anim()
    {
        RunAnim();
    }
}

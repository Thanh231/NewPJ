 
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoaderOverlay_classic : MonoBehaviour, ILoaderOverlay
{
    public CanvasGroup parentAnim;
    public Image imgBg;
    public float fadeTime = 0.5f;
    
    public float PlayAnim_begin()
    {
        StaticUtils.ScaleBackgroundFullscreen_UI(imgBg);
        parentAnim.alpha = 1;
        return 0;
    }

    public void PlayAnim_idle()
    {
    }

    public float PlayAnim_end()
    {
        parentAnim.DOFade(0, fadeTime);
        return fadeTime;
    }
}

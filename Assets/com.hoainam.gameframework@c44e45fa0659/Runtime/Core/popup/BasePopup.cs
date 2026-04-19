using DG.Tweening;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BasePopup : MonoBehaviour
{
    public List<Button> lCloseBtn;
    public AnimType animType;
    public RectTransform content;
    public bool canCloseByBackKey = true;
    
    public bool allowClose { get; set; } = true;
    public UnityAction onCloseUnexpected; //close by close button OR click back key

    #region open popup

    protected virtual void Start()
    {
        foreach (var i in lCloseBtn)
        {
            i.OnClickAsObservable().Subscribe(_ =>
            {
                if (allowClose)
                {
                    onCloseUnexpected?.Invoke();
                    ClosePopup();
                }
            });
        }

        RunAnimOpen().Forget();
    }
    
    private async UniTask RunAnimOpen()
    {
        //some popup setup list view in Start function,
        //if set scale=0 in the same frame,
        //it will screw items position in list view
        await UniTask.DelayFrame(1);
        
        switch (animType)
        {
            case AnimType.scale:
                AnimScaleIn(AfterRunAnim);
                break;
            default:
                AfterRunAnim();
                break;
        }
    }

    protected virtual void AfterRunAnim()
    {

    }

    #endregion
    
    #region close popup

    protected void ClosePopup()
    {
        PopupManager.instance.ClosePopup(this);
    }
    
    public virtual void OnClosePopup(bool isRunAnim = true)
    {
        if (isRunAnim)
            RunAnimClose();
        else
            AfterRunAnimClose();
    }
    
    private void RunAnimClose()
    {
        switch (animType)
        {
            case AnimType.scale:
                AnimScaleOut(AfterRunAnimClose);
                break;
            default:
                AfterRunAnimClose();
                break;
        }
    }

    protected virtual void AfterRunAnimClose()
    {
        Destroy(gameObject);
    }

    #endregion

    #region anim

    private void AnimScaleIn(UnityAction done)
    {
        var cfg = GameFrameworkConfig.instance.popupAnimScaleCfg;
        content.localScale = Vector3.zero;
        content.DOScale(Vector3.one, cfg.scaleDuration).SetEase(cfg.scaleInEase)
            .OnComplete(() => { done?.Invoke(); });
    }

    private void AnimScaleOut(UnityAction done)
    {
        var cfg = GameFrameworkConfig.instance.popupAnimScaleCfg;
        content.localScale = Vector3.one;
        content.DOScale(Vector3.zero, cfg.scaleDuration).SetEase(cfg.scaleOutEase)
            .OnComplete(() => { done?.Invoke(); });
    }

    #endregion
}
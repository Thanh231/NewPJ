
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupYesNoOk : BasePopup
{
    #region core

    public LocalizedText_tmp txtMessage;
    public LocalizedText_tmp txtYes;
    public LocalizedText_tmp txtNo;
    public LocalizedText_tmp txtOK;
    public Button btnYes;
    public Button btnNo;
    public Button btnOK;
    
    private UnityAction onYes;
    private UnityAction onNo;
    private UnityAction onOK;

    protected override void Start()
    {
        base.Start();

        onCloseUnexpected += () =>
        {
            onNo?.Invoke();
        };

        btnYes.OnClickAsObservable().Subscribe(_ =>
        {
            onYes?.Invoke();
            ClosePopup();
        });
        
        btnNo.OnClickAsObservable().Subscribe(_ =>
        {
            onNo?.Invoke();
            ClosePopup();
        });
        
        btnOK.OnClickAsObservable().Subscribe(_ =>
        {
            onOK?.Invoke();
            ClosePopup();
        });
    }

    #endregion
    
    #region show ok

    public static async UniTask ShowOK(string msgKey, object[] msgParams = null)
    {
        await ShowOK_internal(msgKey, msgParams, null, null);
    }
    
    public static async UniTask ShowOK(string msgKey, UnityAction actionOK, object[] msgParams = null)
    {
        await ShowOK_internal(msgKey, msgParams, null, actionOK);
    }
    
    public static async UniTask ShowOK(string msgKey, string okKey, object[] msgParams = null)
    {
        await ShowOK_internal(msgKey, msgParams, okKey, null);
    }
    
    public static async UniTask ShowOK(string msgKey, string okKey, UnityAction actionOK, object[] msgParams = null)
    {
        await ShowOK_internal(msgKey, msgParams, okKey, actionOK);
    }

    private static async UniTask ShowOK_internal(string msgKey, object[] msgParams, string okKey, UnityAction actionOK)
    {
        var pu = await PopupManager.instance.OpenPopup<PopupYesNoOk>();
        
        pu.txtMessage.SetKeyAndParameters(msgKey, msgParams);
        pu.txtOK.SetKey(string.IsNullOrEmpty(okKey) ? "common_btn_ok" : okKey);
        pu.onOK = actionOK;

        pu.btnOK.gameObject.SetActive(true);
        pu.btnYes.gameObject.SetActive(false);
        pu.btnNo.gameObject.SetActive(false);
    }

    #endregion

    #region show yes/no

    public static async UniTask ShowYesNo(string msgKey, object[] msgParams = null)
    {
        await ShowYesNo_internal(msgKey, msgParams, null, null, null, null);
    }
    
    public static async UniTask ShowYesNo(string msgKey, UnityAction actionYes, object[] msgParams = null)
    {
        await ShowYesNo_internal(msgKey, msgParams, null, null, actionYes, null);
    }
    
    public static async UniTask ShowYesNo(string msgKey, UnityAction actionYes, UnityAction actionNo, object[] msgParams = null)
    {
        await ShowYesNo_internal(msgKey, msgParams, null, null, actionYes, actionNo);
    }
    
    public static async UniTask ShowYesNo(string msgKey, string yesKey, string noKey, object[] msgParams = null)
    {
        await ShowYesNo_internal(msgKey, msgParams, yesKey, noKey, null, null);
    }
    
    public static async UniTask ShowYesNo(string msgKey, string yesKey, string noKey, UnityAction actionYes, object[] msgParams = null)
    {
        await ShowYesNo_internal(msgKey, msgParams, yesKey, noKey, actionYes, null);
    }

    private static async UniTask ShowYesNo_internal(string msgKey, object[] msgParams, string yesKey, string noKey,
        UnityAction actionYes, UnityAction actionNo)
    {
        var pu = await PopupManager.instance.OpenPopup<PopupYesNoOk>();
        
        pu.txtMessage.SetKeyAndParameters(msgKey, msgParams);
        pu.txtYes.SetKey(string.IsNullOrEmpty(yesKey) ? "common_btn_yes" : yesKey);
        pu.txtNo.SetKey(string.IsNullOrEmpty(noKey) ? "common_btn_no" : noKey);
        pu.onYes = actionYes;
        pu.onNo = actionNo;
        
        pu.btnYes.gameObject.SetActive(true);
        pu.btnNo.gameObject.SetActive(true);
        pu.btnOK.gameObject.SetActive(false);
    }

    #endregion
}

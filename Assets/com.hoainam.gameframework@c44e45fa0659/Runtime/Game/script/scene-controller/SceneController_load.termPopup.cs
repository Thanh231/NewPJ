
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public partial class SceneController_load
{
    #region check has opend already

    private const string PREF_KEY_CAN_OPEN_TERM = "CAN_OPEN_TERM";

    private static bool GetCanOpen()
    {
        var val = PlayerPrefs.GetInt(PREF_KEY_CAN_OPEN_TERM, 1);
        return val == 1;
    }

    private static void SetHasOpenedAlready()
    {
        PlayerPrefs.SetInt(PREF_KEY_CAN_OPEN_TERM, 0);
    }

    #endregion

    #region open term popup

    public static UniTask<PopupTermKorean.ClosePopupResult> OpenTermPopup(SystemLanguage language)
    {
        var utcs = new UniTaskCompletionSource<PopupTermKorean.ClosePopupResult>();

        if (!GameFrameworkConfig.instance.alwaysShowTerm && !GetCanOpen())
        {
            utcs.TrySetResult(null);
        }
        else
        {
            if (language == SystemLanguage.Korean)
            {
                OpenTermPopup_korean(result =>
                {
                    utcs.TrySetResult(result);
                }).Forget();
            }
            else
            {
                OpenTermPopup_global(() =>
                {
                    utcs.TrySetResult(null);
                }).Forget();
            }
        }

        return utcs.Task;
    }

    private static async UniTask OpenTermPopup_korean(UnityAction<PopupTermKorean.ClosePopupResult> callback)
    {
        var pu = await PopupManager.instance.OpenPopup<PopupTermKorean>();
        pu.closeEvent += result =>
        {
            SetHasOpenedAlready();
            callback?.Invoke(result);
        };
    }

    private static async UniTask OpenTermPopup_global(UnityAction callback)
    {
        var pu = await PopupManager.instance.OpenPopup<PopupTermGlobal>();
        pu.closeEvent += () =>
        {
            SetHasOpenedAlready();
            callback?.Invoke();
        };
    }

    #endregion
}

using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class EditorWindowState_uploadToServer : EditorWindowState
{
    protected ServerEnvironment serverEnvironment;
    private string txtConfirm = "";
    
    private const string CONFIRM_MSG = "I want to upload to LIVE server";
    
    protected abstract string ActionBtnTxt { get; }

    public override void OnDraw()
    {
        serverEnvironment = EditorGUILayoutExtension.EnumDropdownList(
            "server: ", ServerEnvironment.count, serverEnvironment, out _);
        
        if (serverEnvironment == ServerEnvironment.Live)
        {
            txtConfirm = EditorGUILayoutExtension.FitTextField($"enter \"{CONFIRM_MSG}\" to continue", txtConfirm);
        }
        
        if (serverEnvironment != ServerEnvironment.Live || txtConfirm.Equals(CONFIRM_MSG))
        {
            if (GUILayout.Button(ActionBtnTxt))
            {
                OnActionBtnClick().Forget();
            }
        }
    }

    protected abstract UniTask OnActionBtnClick();
}

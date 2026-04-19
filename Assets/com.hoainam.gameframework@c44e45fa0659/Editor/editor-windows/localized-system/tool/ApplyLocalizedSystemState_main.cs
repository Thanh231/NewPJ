
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ApplyLocalizedSystemState_main : EditorWindowState
{
    private class LocalizeData
    {
        public Dictionary<SystemLanguage, string> dicAppName;
        public Dictionary<SystemLanguage, string> dicTrackingDesc;
    }
    
    public override void OnDraw()
    {
        if (GUILayout.Button("apply localized system"))
        {
            OnClickApplyLocalizedSystem().Forget();
        }
    }

    private async UniTask OnClickApplyLocalizedSystem()
    {
        FSM.SwitchState(new EditorWindowState_doing());
        try
        {
            var data = await GetLocalizedSystemData();
            
            AndroidLocalizeProcessor.Process(data.dicAppName);
            IosLocalizeProcessor.Process(data.dicAppName, data.dicTrackingDesc);
            
            AssetDatabase.Refresh();

            FSM.SwitchState(new EditorWindowState_done());
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            StaticUtilsEditor.DisplayDialog("apply localized system fail, see console for detail");
        }
    }

    private async UniTask<LocalizeData> GetLocalizedSystemData()
    {
        var appNameKey = GameFrameworkConfig.instance.appNameLocalizedKey;
        var trackingDescKey = GameFrameworkConfig.instance.trackingDescLocalizedKey;

        if (string.IsNullOrEmpty(appNameKey))
        {
            throw new Exception("need input appNameLocalizedKey for GameFrameworkConfig");
        }

        if (string.IsNullOrEmpty(trackingDescKey))
        {
            throw new Exception("need input trackingDescLocalizedKey for GameFrameworkConfig");
        }

        var locController = new LocalizationController();
        await locController.LoadAllLocalizationData();

        return new LocalizeData
        {
            dicAppName = locController.GetLocalizationTextAllLangs(appNameKey),
            dicTrackingDesc = locController.GetLocalizationTextAllLangs(trackingDescKey)
        };
    }
}

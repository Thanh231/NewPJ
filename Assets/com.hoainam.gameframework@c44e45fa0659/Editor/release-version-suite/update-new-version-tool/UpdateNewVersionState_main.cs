
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class UpdateNewVersionState_main : EditorWindowState_uploadToServer
{
    private string txtVersion;
    private bool isMandatoryUpdate = true;
    
    protected override string ActionBtnTxt => "update version";

    public override void OnDraw()
    {
        txtVersion = EditorGUILayoutExtension.FitTextField("version:", txtVersion);
        isMandatoryUpdate = EditorGUILayout.Toggle("mandatory update", isMandatoryUpdate);

        base.OnDraw();
    }

    protected override async UniTask OnActionBtnClick()
    {
        FSM.PushState(new EditorWindowState_doing());
        try
        {
            if (!StaticUtils.IsValidVersion(txtVersion))
            {
                throw new Exception("version is invalid");
            }

            await UploadNewVersion();
            
            StaticUtilsEditor.DisplayDialog("upload new version success");
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            StaticUtilsEditor.DisplayDialog("upload new version fail, see console for detail");
        }
        FSM.PopState();
    }
    
    private async UniTask UploadNewVersion()
    {
        await UniTask.CompletedTask;
        
        var cmd = $"{StaticUtils.GetProjectPath()}/ExternalTools/AwsUploader/CSharpProjUploadAWS.exe";
        var result = StaticUtilsEditor.RunBatchScript(cmd, new List<string>()
        {
            "game_version",
            serverEnvironment.ToString(),
            txtVersion,
            isMandatoryUpdate.ToString()
        });

        if (!result.isSuccess)
        {
            throw new Exception("CSharpProjUploadAWS.exe run failed, check console for details");
        }
    }
}

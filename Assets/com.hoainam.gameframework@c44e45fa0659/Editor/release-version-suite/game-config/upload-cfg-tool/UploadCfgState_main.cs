
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UploadCfgState_main : EditorWindowState_uploadToServer
{
    protected override string ActionBtnTxt => "upload configs";
    
    protected override async UniTask OnActionBtnClick()
    {
        FSM.PushState(new EditorWindowState_doing());
        try
        {
            var binaryCfgFolder = ConvertToBinaryCfg();
            ConvertOtherCfgToBinary(binaryCfgFolder);
            await UploadCfg(binaryCfgFolder);
            
            StaticUtilsEditor.DisplayDialog("upload configs success");
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            StaticUtilsEditor.DisplayDialog("upload configs fail, see console for detail");
        }
        FSM.PopState();
    }

    private string ConvertToBinaryCfg()
    {
        var cfgFolder = StaticUtilsEditor.RandomATempPath();
        var cfgFile = $"{cfgFolder}/all_config.bin";
        StaticUtils.OpenFileForWrite(cfgFile, binWriter =>
        {
            var configReadWriteManager = new ConfigReadWriteManager(ConfigManager.GetListConfigsImplementedInClientCode());
            configReadWriteManager.ReadConfig_editor();
            configReadWriteManager.WriteConfigBinary(binWriter);
        }, true);
        return cfgFolder;
    }

    private void ConvertOtherCfgToBinary(string parentFolder)
    {
        var assembly = StaticUtils.GetAssembly(StaticUtils.MainAssemblyEditorName);
        var interfaceTypes = StaticUtils.ListClassImplementOrInherit(assembly, typeof(IBinaryConfigConverter));

        foreach (var interfaceType in interfaceTypes)
        {
            var converter = (IBinaryConfigConverter)Activator.CreateInstance(interfaceType);
            converter.Convert(parentFolder);
        }
    }

    private async UniTask UploadCfg(string binaryCfgFolder)
    {
        var cmd = $"{StaticUtils.GetProjectPath()}/ExternalTools/AwsUploader/CSharpProjUploadAWS.exe";
        var textCfgFolder = $"{StaticUtils.GetProjectPath()}/GameConfig";
        var result = StaticUtilsEditor.RunBatchScript(cmd, new List<string>()
        {
            "config",
            serverEnvironment.ToString(),
            textCfgFolder,
            binaryCfgFolder
        });

        if (!result.isSuccess)
        {
            throw new Exception("CSharpProjUploadAWS.exe run failed, check console for details");
        }
    }
}


using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DownloadCfgState_main : EditorWindowState
{
    #region UI
    
    private ServerEnvironment serverEnvironment;
    
    private readonly EditorFolderPicker outputFolder = new("output folder:");
    
    public override void OnDraw()
    {
        serverEnvironment = EditorGUILayoutExtension.EnumDropdownList(
            "server: ", ServerEnvironment.count, serverEnvironment, out _);
        
        outputFolder.Draw();

        if (GUILayout.Button("download"))
        {
            OnBtnDownloadClick().Forget();
        }
    }
    
    private async UniTask OnBtnDownloadClick()
    {
        FSM.PushState(new EditorWindowState_doing());
        try
        {
            await DownloadCfg();
            StaticUtilsEditor.DisplayDialog("download configs success");
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            StaticUtilsEditor.DisplayDialog("download configs fail, see console for detail");
        }
        FSM.PopState();
    }
    
    #endregion

    #region download cfg
    
    private ServerController _serverController;
    private ServerController serverController
    {
        get
        {
            if (_serverController == null)
            {
                _serverController = new ServerController();
                _serverController.Init(serverEnvironment, true);
            }

            return _serverController;
        }
    }

    private async UniTask DownloadCfg()
    {
        // get cfg version
        var cfgVersion = await GetCfgVersion();
        Debug.Log($"cfg version: {cfgVersion}");
        
        // get list cfg
        var parentPath = $"{serverEnvironment}/game_configs/{cfgVersion}";
        var lCfgNames = await GetListCfg(parentPath);
        
        // download cfg
        var lTasks = new List<UniTask>();
        foreach (var cfgName in lCfgNames)
        {
            var key = $"{parentPath}/text/{cfgName}.csv";
            lTasks.Add(serverController.GameContent_download(key, outputFolder.PickedPath));
        }
        await UniTask.WhenAll(lTasks);
    }

    private async UniTask<string> GetCfgVersion()
    {
        var serverCfgJson = new ServerConfigJson(serverController);
        await serverCfgJson.Download();
        
        return serverCfgJson.gameConfigVersion;
    }

    private async UniTask<List<string>> GetListCfg(string parentPath)
    {
        var text = await serverController.GameContent_get($"{parentPath}/list_cfg.txt");
        return new List<string>(text.Split('\n'));
    }

    #endregion
}

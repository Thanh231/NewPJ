
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ManageTesterState_start : EditorWindowState
{
    public override void OnDraw()
    {
        if (GUILayout.Button("connect"))
        {
            OnClickConnect().Forget();
        }
    }
    
    private async UniTask OnClickConnect()
    {
        try
        {
            var testers =  await GetListTesters();
            FSM.SwitchState(new ManageTesterState_testers(testers));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            StaticUtilsEditor.DisplayDialog("get list testers fail, see console for detail");
        }
    }

    private async UniTask<List<ServerConfigJson.TesterInfo>> GetListTesters()
    {
        FSM.PushState(new EditorWindowState_doing());
        
        var serverController = new ServerController();
        serverController.Init(ServerEnvironment.Dev, true);
        
        var serverCfgJson = new ServerConfigJson(serverController);
        await serverCfgJson.Download();
        
        FSM.PopState();

        return serverCfgJson.listTesters;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class ManageTesterState_testers : EditorWindowState
{
    private List<ServerConfigJson.TesterInfo> listTesters;
    private ServerConfigJson.TesterInfo itemToRemove;
    
    public ManageTesterState_testers(List<ServerConfigJson.TesterInfo> _listTesters)
    {
        listTesters = _listTesters;
        if (listTesters == null)
        {
            listTesters = new List<ServerConfigJson.TesterInfo>();
        }
    }
    
    public override void OnDraw()
    {
        foreach (var tester in listTesters)
        {
            DrawTesterItem(tester);
        }

        if (itemToRemove != null)
        {
            RemoveTester(itemToRemove);
        }
        
        if (GUILayout.Button("add tester"))
        {
            FSM.PushState(new ManageTesterState_addTester(this));
        }
    }
    
    private void DrawTesterItem(ServerConfigJson.TesterInfo testerInfo)
    {
        GUILayout.BeginHorizontal();
        
        GUILayout.Label($"deviceId: {testerInfo.deviceId}", GUI.skin.textField);
        GUILayout.Label($"testerName: {testerInfo.testerName}", GUI.skin.textField);
        if (EditorGUILayoutExtension.FitButton("remove"))
        {
            if (StaticUtilsEditor.DisplayConfirmDialog("are you sure to remove this tester?"))
            {
                itemToRemove = testerInfo;
            }
        }

        GUILayout.EndHorizontal();
    }

    public void AddTester(ServerConfigJson.TesterInfo testerInfo)
    {
        RunUploadToServer(new List<string>()
        {
            "add_tester",
            $"\"{testerInfo.testerName}\"",
            $"\"{testerInfo.deviceId}\""
        });
        
        listTesters.Add(testerInfo);
    }
    
    private void RemoveTester(ServerConfigJson.TesterInfo testerInfo)
    {
        RunUploadToServer(new List<string>()
        {
            "remove_tester",
            $"\"{testerInfo.deviceId}\""
        });
        
        listTesters.Remove(testerInfo);
        itemToRemove = null;
    }
    
    private void RunUploadToServer(List<string> lParams)
    {
        var cmd = $"{StaticUtils.GetProjectPath()}/ExternalTools/AwsUploader/CSharpProjUploadAWS.exe";
        var result = StaticUtilsEditor.RunBatchScript(cmd, lParams);

        if (!result.isSuccess)
        {
            StaticUtilsEditor.DisplayDialog("CSharpProjUploadAWS.exe run failed, check console for details");
        }
    }
}
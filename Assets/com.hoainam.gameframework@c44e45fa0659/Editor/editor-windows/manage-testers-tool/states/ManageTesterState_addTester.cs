
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ManageTesterState_addTester : EditorWindowState
{
    private string testerName = "";
    private string deviceId = "";

    private ManageTesterState_testers stateManageTester;

    public ManageTesterState_addTester(ManageTesterState_testers stateManageTester)
    {
        this.stateManageTester = stateManageTester;
    }
    
    public override void OnDraw()
    {
        testerName = EditorGUILayout.TextField("Tester Name", testerName);
        deviceId = EditorGUILayout.TextField("Device ID", deviceId);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("add"))
        {
            FSM.PopState();
            var testerInfo = new ServerConfigJson.TesterInfo
            {
                testerName = testerName,
                deviceId = deviceId
            };
            stateManageTester.AddTester(testerInfo);
        }
        if (GUILayout.Button("cancel"))
        {
            FSM.PopState();
        }
        EditorGUILayout.EndHorizontal();
    }
}
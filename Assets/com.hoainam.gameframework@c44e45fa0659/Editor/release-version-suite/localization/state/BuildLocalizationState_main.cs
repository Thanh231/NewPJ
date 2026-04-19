
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

//if you got the issue: when build addressable done, one of 3 platforms is not built,
//the solution: make sure when open addressable unity project, no need to switch platform to build
//how to do it: clear all player pref + open addressable project => switch to windows platform

public partial class BuildLocalizationState_main : EditorWindowState_uploadToServer
{
    private readonly List<EditorFilePicker> lPickFiles = new();
    private readonly EditorFolderPicker pickRemoteAddressableProject =
        new("remote addressable project:", "KEY_REMOTE_ADDRESSABLE_PROJECT");
    private bool uploadToo;

    protected override string ActionBtnTxt => "build & upload localization";
    
    public override void OnBeginState()
    {
        base.OnBeginState();

        var lExtension = new List<string>() { "csv" };
        foreach (var i in GameFrameworkConfig.instance.locFileNames)
        {
            lPickFiles.Add(new EditorFilePicker(lExtension, i));
        }
    }
    
    public override void OnDraw()
    {
        foreach (var i in lPickFiles)
        {
            i.Draw();
        }

        uploadToo = EditorGUILayout.Toggle("upload too", uploadToo);
        if (uploadToo)
        {
            pickRemoteAddressableProject.Draw();
            base.OnDraw();
        }
        else
        {
            if (GUILayout.Button("build localization"))
            {
                OnActionBtnClick().Forget();
            }
        }
    }

    protected override async UniTask OnActionBtnClick()
    {
        FSM.SwitchState(new EditorWindowState_doing());
        try
        {
            await UniTask.Delay(100);

            var lPaths = new List<string>();
            foreach (var file in lPickFiles)
            {
                lPaths.Add(file.PickedPath);
            }

            var charSet = BuildLocalizationText(lPaths);
            BuildTextMeshProFont(charSet);

            if (uploadToo)
            {
                UploadRemoteAddressable(serverEnvironment, pickRemoteAddressableProject.PickedPath);
            }

            CopyToLocalLocalization();

            FSM.SwitchState(new EditorWindowState_done());
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            StaticUtilsEditor.DisplayDialog("build localization fail, see console for detail");
        }
    }

    private void CopyToLocalLocalization()
    {
        var srcPath = "_game/localization-data";
        var destPath = "_game/localization-local-data";
        
        StaticUtils.CopyFolder(srcPath, destPath);
    }
}


using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BuildLocalizationState_main
{
    private const string KEY_PREF_ORDER_BUILD_ADDRESSABLE = "KEY_PREF_ORDER_BUILD_ADDRESSABLE";
    
    private void UploadRemoteAddressable(ServerEnvironment serverEnvironment, string projPath)
    {
        StaticUtils.DeleteFolder($"{projPath}/ServerData", true);
        
        CopyAssets(projPath);
        BuildAssets(projPath);
        UploadAssets(serverEnvironment, projPath);
    }

    private void CopyAssets(string projPath)
    {
        var srcPath = $"{Application.dataPath}/_game/localization-data";
        var targetPath = $"{projPath}/Assets/_game/localization-data";
        StaticUtils.CopyFolder(srcPath, targetPath, true);
    }

    private void BuildAssets(string projPath)
    {
        var lPlatforms = new List<string>()
        {
            "StandaloneWindows64", "Android", "iOS"
        };
        var parentShScript = $"{StaticUtils.GetFrameworkPath()}/Editor/release-version-suite/localization/sh-script";
        var order = PlayerPrefs.GetInt(KEY_PREF_ORDER_BUILD_ADDRESSABLE);

        for (var i = 0; i < lPlatforms.Count; i++)
        {
            var platform = order == 0 ? lPlatforms[i] : lPlatforms[lPlatforms.Count - 1 - i];
            StaticUtilsEditor.RunShellScript($"{parentShScript}/switch_platform.sh", new List<string>()
            {
                StaticUtilsEditor.UnityExeFile,
                projPath,
                platform,
            });
            StaticUtilsEditor.RunShellScript($"{parentShScript}/build_addressable.sh", new List<string>()
            {
                StaticUtilsEditor.UnityExeFile,
                projPath,
            });
        }
        
        PlayerPrefs.SetInt(KEY_PREF_ORDER_BUILD_ADDRESSABLE, 1 - order);
    }

    private void UploadAssets(ServerEnvironment serverEnvironment, string projPath)
    {
        var cmd = $"{StaticUtils.GetProjectPath()}/ExternalTools/AwsUploader/CSharpProjUploadAWS.exe";
        var result = StaticUtilsEditor.RunBatchScript(cmd, new List<string>()
        {
            "addressable",
            serverEnvironment.ToString(),
            $"{projPath}/ServerData",
        });

        if (!result.isSuccess)
        {
            throw new Exception("CSharpProjUploadAWS.exe run failed, check console for details");
        }
    }
}

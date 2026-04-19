
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AndroidLocalizeProcessor
{
    public static void Process(Dictionary<SystemLanguage, string> dicAppName)
    {
        //copy android project
        var srcPath = $"{StaticUtils.GetFrameworkPath()}/Editor/editor-windows/localized-system/android/AndroidProject";
        var destPath = StaticUtilsEditor.RandomATempPath();
        StaticUtils.CopyFolder(srcPath, destPath, isAbsolutePath: true);
		
        //process project
        ProcessAndroidProject(destPath, dicAppName);

        //build
        var cmd = $"{destPath}/gradlew.bat";
        var arg = new List<string>() { "localized-system:assembleRelease" };
        var result = StaticUtilsEditor.RunBatchScript(cmd, arg, destPath);
        if (!result.isSuccess)
        {
            throw new Exception($"build android project at {destPath} failed");
        }

        //copy aar back to project
        var aarFile = $"{destPath}/localized-system/build/outputs/aar/localized-system-release.aar";
        var aarTargetFolder = $"{Application.dataPath}/Plugins/Android";
        StaticUtils.CopyFile(aarFile, aarTargetFolder, isAbsolutePath: true);
    }
    
    private static void ProcessAndroidProject(string projectPath, Dictionary<SystemLanguage, string> dicAppName)
    {
        //set java home
        if (!StaticUtils.CheckFolderExist(StaticUtilsEditor.JavaFromAndroidStudioFolder, true))
        {
            throw new Exception("need install android studio");
        }
        var gradlewPath = $"{projectPath}/gradlew.bat";
        var gradlewText = StaticUtils.ReadTextFile(gradlewPath, isAbsolutePath: true);

        var javaPath = StaticUtilsEditor.JavaFromAndroidStudioFolder.Replace('\\', '/');
        gradlewText = gradlewText.Replace("{SET_JAVA_HERE}", javaPath);
        StaticUtils.WriteTextFile(gradlewPath, gradlewText, isAbsolutePath: true);

        //create localize folder
        var template = StaticUtils.GetResourceFileText("AndroidLocalizeTemplate");
        var languageInfo = new LanguageInfoConfig();
        foreach (var i in dicAppName)
        {
            var text = template.Replace("{appName}", i.Value);
            var folderName = $"values-{languageInfo.GetLanguageInfoItem(i.Key).androidIsoCode}";
            var path = $"{projectPath}/localized-system/src/main/res/{folderName}/strings.xml";
            StaticUtils.WriteTextFile(path, text, isAbsolutePath: true);
        }
		
        //set min/target sdk
        var gradlePath = $"{projectPath}/localized-system/build.gradle";
        var gradleText = StaticUtils.ReadTextFile(gradlePath, isAbsolutePath: true);
		
        var minSdk = (int)PlayerSettings.Android.minSdkVersion;
        var targetSdk = (int)PlayerSettings.Android.targetSdkVersion;

        if (targetSdk <= 0)
        {
            throw new Exception("need to set Target API level in android player settings");
        }

        gradleText = gradleText.Replace("**MIN_SDK**", minSdk.ToString())
            .Replace("**TARGET_SDK**", targetSdk.ToString());
		
        StaticUtils.WriteTextFile(gradlePath, gradleText, isAbsolutePath: true);
		
        //set sdk direction
        var propertiesPath = $"{projectPath}/local.properties";
        var propertiesText = StaticUtils.ReadTextFile(propertiesPath, isAbsolutePath: true);
		
        var sdkDir = BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.Android, BuildOptions.None);
        sdkDir = $"{sdkDir}/SDK";

        propertiesText = propertiesText.Replace("**SDK_PATH**", sdkDir);
		
        StaticUtils.WriteTextFile(propertiesPath, propertiesText, isAbsolutePath: true);
    }
}

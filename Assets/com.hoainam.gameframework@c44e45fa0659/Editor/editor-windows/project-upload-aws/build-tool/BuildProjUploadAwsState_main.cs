
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class BuildProjUploadAwsState_main : EditorWindowState
{
    private string accessKey;
    private string secretKey;
    
    public override void OnDraw()
    {
        accessKey = EditorGUILayout.TextField("Access Key:", accessKey);
        secretKey = EditorGUILayout.TextField("Secret Key:", secretKey);
        
        
        if (GUILayout.Button("build project"))
        {
            OnClickBuild().Forget();
        }
    }

    private async UniTask OnClickBuild()
    {
        FSM.PushState(new EditorWindowState_doing());
        try
        {
            var projPath = CopyProject();
            ChangeProjectConstants(projPath);
            await BuildProject(projPath);
            CopyBackExecutable(projPath);
            
            StaticUtilsEditor.DisplayDialog("build project success");
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            StaticUtilsEditor.DisplayDialog("build project fail, see console for detail");
        }
        FSM.PopState();
    }

    private string CopyProject()
    {
        var srcPath = $"{StaticUtils.GetFrameworkPath()}/Editor/editor-windows/project-upload-aws/csharp-project";
        var targetPath = StaticUtilsEditor.RandomATempPath();
        StaticUtils.CopyFolder(srcPath, targetPath, true);
        return targetPath;
    }

    private void ChangeProjectConstants(string projectPath)
    {
        var constFilePath = $"{projectPath}/Const.cs";
        var fileContent = StaticUtils.ReadTextFile(constFilePath, true);
        var cfg = GameFrameworkConfig.instance;

        fileContent = fileContent.Replace("<awsS3BucketName>", cfg.awsS3BucketName);
        fileContent = fileContent.Replace("<awsS3GameContentRoot>", cfg.awsS3GameContentRoot);
        fileContent = fileContent.Replace("<awsCloudFrontDistributionId>", cfg.awsCloudFrontDistributionId);
        fileContent = fileContent.Replace("<awsS3GameContentUrl>", cfg.awsS3GameContentUrl);
        fileContent = fileContent.Replace("<awsRegion>", cfg.awsRegion);
        fileContent = fileContent.Replace("<awsAccessKeyId>", accessKey);
        fileContent = fileContent.Replace("<awsSecretAccessKey>", secretKey);
        
        StaticUtils.WriteTextFile(constFilePath, fileContent, true);
    }

    private async UniTask BuildProject(string projectPath)
    {
        var lFiles = StaticUtils.GetFilesInFolder(projectPath, false, null, "csproj", true);
        var outputPath = $"{projectPath}/publish";
        
        StaticUtilsEditor.RunBatchScript("dotnet", new List<string>()
        {
            "publish",
            lFiles[0],
            "-c",
            "Release",
            "-o",
            outputPath,
        });
    }

    private void CopyBackExecutable(string projectPath)
    {
        var srcPath = $"{projectPath}/publish";
        var targetPath = $"{StaticUtils.GetProjectPath()}/ExternalTools/AwsUploader";
        StaticUtils.CopyFolder(srcPath, targetPath, true);
    }
}

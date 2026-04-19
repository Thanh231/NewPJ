
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

public class ExportSpineState_export : EditorWindowState
{
    #region core

    private readonly string inputFolder;
    private readonly string outputFolder;
    
    private int maxProgress;
    private int currentProgress;
    
    public ExportSpineState_export(string inputFolder, string outputFolder)
    {
        this.inputFolder = inputFolder;
        this.outputFolder = outputFolder;
    }

    public override void OnBeginState()
    {
        base.OnBeginState();
        
        var listSpineFiles = StaticUtils.GetFilesInFolder(inputFolder, true, null, "spine", true);
        maxProgress = listSpineFiles.Count;
        
        EditorCoroutineUtility.StartCoroutineOwnerless(ExportCoroutine(listSpineFiles));
    }
    
    public override void OnDraw()
    {
        var windowSize = FSM.position;
        var text = $"exporting {currentProgress}/{maxProgress}";
        EditorGUILayoutExtension.CenterLabelField(text, windowSize.width, windowSize.height, 60);
    }

    #endregion

    #region export

    private IEnumerator ExportCoroutine(List<string> listSpineFiles)
    {
        var isSuccess = true;

        foreach (var i in listSpineFiles)
        {
            yield return new WaitForSeconds(0.5f);

            isSuccess &= ExportSpine(i);
            currentProgress++;
            FSM.Repaint();

            if (!isSuccess)
            {
                break;
            }
        }

        if (isSuccess)
        {
            StaticUtilsEditor.DisplayDialog("export spine success");
        }
        else
        {
            StaticUtilsEditor.DisplayDialog("export spine failed, please check the console for details");
        }
    }

    private bool ExportSpine(string spineFilePath)
    {
        var spineFileFolder = StandardizePath(Path.GetDirectoryName(spineFilePath));
        var inputPath = StandardizePath(inputFolder);
        
        var outputPath = spineFileFolder.Replace(inputPath, outputFolder);

        var settingsPath =
            $"{StaticUtils.GetFrameworkPath()}/Editor/editor-windows/export-spine-tool/export-template.export.json";
        if (!StaticUtils.CheckFileExist(settingsPath, true))
        {
            throw new Exception($"File does not exist: {settingsPath}");
        }

        var result = StaticUtilsEditor.RunBatchScript(StaticUtilsEditor.SpineFile, new List<string>()
        {
            "-i",
            $"\"{spineFilePath}\"",
            "-o",
            $"\"{outputPath}\"",
            "-e",
            $"\"{settingsPath}\"",
        });

        return result.isSuccess;
    }

    private string StandardizePath(string path)
    {
        if (path.Contains('\\'))
        {
            path = path.Replace('\\', '/');
        }
        return path;
    }

    #endregion
}

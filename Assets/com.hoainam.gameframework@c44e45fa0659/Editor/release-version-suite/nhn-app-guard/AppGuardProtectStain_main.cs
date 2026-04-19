
#if USE_NHN_APP_GUARD

using System.Collections.Generic;
using System.IO;
using DiresuUnity.Editor;
using UnityEngine;

public class AppGuardProtectStain_main : EditorWindowState
{
    private readonly EditorFilePicker inputFilePath = new(
        new List<string> { "apk", "aab" }, "choose apk/aab file:");

    public override void OnDraw()
    {
        inputFilePath.Draw();

        if (GUILayout.Button("protect"))
        {
            var inputPath = inputFilePath.PickedPath;
            var outputPath = CreateOutputPath(inputPath);
            
            AppGuardAndroidProtector.Protect(inputPath, outputPath);
            
            StaticUtilsEditor.DisplayDialog("protect done");
        }
    }
    
    private string CreateOutputPath(string inputPath)
    {
        var directory = Path.GetDirectoryName(inputPath);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputPath);
        var extension = Path.GetExtension(inputPath);
        
        return Path.Combine(directory, fileNameWithoutExtension + "_protectedBy_NHNAppGuard" + extension);
    }
}

#endif

using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class OneClickToolState_main
{
    #region core

    private void OnClick_updateVSCodeHidingFiles()
    {
        var lines = ReadVSCodeSettings(out int idxToInsert);
        var excludedItems = GetExcludedItems();
        WriteVSCodeSettings(lines, idxToInsert, excludedItems);

        StaticUtilsEditor.DisplayDialog("updated vs code settings");
    }

    private List<string> ReadVSCodeSettings(out int idxToInsert)
    {
        var lResult = new List<string>();
        var path = $"{StaticUtils.GetProjectPath()}/.vscode/settings.json";
        var inExcludeRegion = false;
        var inCustomExcludeRegion = false;
        var tIdxToInsert = 0;
        StaticUtils.ReadTextFileIntoLines(path, (lineTxt, lineIdx) =>
        {
            if (lineTxt.Contains("\"files.exclude\": {"))
            {
                inExcludeRegion = true;
            }
            if (inExcludeRegion && lineTxt.Contains("},"))
            {
                inExcludeRegion = false;
                tIdxToInsert = lResult.Count;
            }
            if (lineTxt.Contains("\"begin-my-exclude--------------\": true,"))
            {
                inCustomExcludeRegion = true;
            }
            if (!inCustomExcludeRegion &&
                (!inExcludeRegion || !lineTxt.Contains("\"Library/\": true,")))
            {
                lResult.Add(lineTxt);
            }
            if (inCustomExcludeRegion && lineTxt.Contains("\"end-my-exclude--------------\": true"))
            {
                inCustomExcludeRegion = false;
            }
        }, true);
        idxToInsert = tIdxToInsert;
        return lResult;
    }

    #endregion

    #region get excluded items

    private List<string> GetExcludedItems()
    {
        var lResult = GetExcludedItems_root();
        lResult.AddRange(GetExcludedItems_assets());
        lResult.AddRange(GetExcludedItems_library());

        return lResult;
    }

    private List<string> GetExcludedItems_root()
    {
        var lResult = new List<string>();
        var filenames = StaticUtils.GetAllFileNameInFolder(StaticUtils.GetProjectPath(), true);
        var foldernames = StaticUtils.GetAllFolderNameInFolder(StaticUtils.GetProjectPath(), true);

        foreach (var name in filenames)
        {
            if (name.EndsWith(".csproj"))
            {
                continue;
            }

            lResult.Add(name);
        }

        foreach (var name in foldernames)
        {
            if (name.Equals("Assets") || name.Equals("Library"))
            {
                continue;
            }

            lResult.Add($"{name}/");
        }

        return lResult;
    }

    private List<string> GetExcludedItems_assets()
    {
        var lResult = new List<string>();
        var filenames = StaticUtils.GetAllFileNameInFolder(Application.dataPath, true);
        var foldernames = StaticUtils.GetAllFolderNameInFolder(Application.dataPath, true);

        foreach (var name in filenames)
        {
            if (name.EndsWith(".meta"))
            {
                continue;
            }

            lResult.Add($"Assets/{name}");
        }

        foreach (var name in foldernames)
        {
            if (name.StartsWith('_'))
            {
                continue;
            }

            if (name.Equals("Framework"))
            {
                continue;
            }

            lResult.Add($"Assets/{name}/");
        }

        return lResult;
    }

    private List<string> GetExcludedItems_library()
    {
        var lResult = new List<string>();

        //exclude library folder

        var path = $"{StaticUtils.GetProjectPath()}/Library";
        var filenames = StaticUtils.GetAllFileNameInFolder(path, true);
        var foldernames = StaticUtils.GetAllFolderNameInFolder(path, true); 

        foreach (var name in filenames)
        {
            lResult.Add($"Library/{name}");
        }

        foreach (var name in foldernames)
        {
            if (name.Equals("PackageCache"))
            {
                continue;
            }

            lResult.Add($"Library/{name}/");
        }

        //exclude package folder

        path = $"{path}/PackageCache";
        foldernames = StaticUtils.GetAllFolderNameInFolder(path, true);

        foreach (var name in foldernames)
        {
            if (name.StartsWith("com.hoainam.gameframework"))
            {
                continue;
            }

            lResult.Add($"Library/PackageCache/{name}/");
        }

        return lResult;
    }

    #endregion

    #region write settings

    private void WriteVSCodeSettings(List<string> lines, int idxToInsert, List<string> excludedItems)
    {
        var path = $"{StaticUtils.GetProjectPath()}/.vscode/settings.json";
        StaticUtils.OpenFileForWrite(path, (StreamWriter writer) =>
        {
            for (var i = 0; i < lines.Count; i++)
            {
                if (i == idxToInsert)
                {
                    WriteCustomExcludedItems(excludedItems, writer, GetLinePrefix(lines[idxToInsert - 1]));
                }

                if (i < lines.Count - 1)
                {
                    writer.WriteLine(ProcessLineBeforeWriting(lines[i], i, idxToInsert));
                }
                else
                {
                    writer.Write(lines[i]);
                }
            }
        }, true);
    }

    private void WriteCustomExcludedItems(List<string> excludedItems, StreamWriter writer, string prefix)
    {
        writer.WriteLine($"{prefix}\"begin-my-exclude--------------\": true,");
        foreach (var item in excludedItems)
        {
            var line = $"{prefix}\"{item}\": true,";
            writer.WriteLine(line);
        }
        writer.WriteLine($"{prefix}\"end-my-exclude--------------\": true");
    }

    private string GetLinePrefix(string sampleLine)
    {
        var idx = sampleLine.IndexOf('\"');
        return sampleLine.Substring(0, idx);
    }

    private string ProcessLineBeforeWriting(string lineTxt, int lineIdx, int idxToInsert)
    {
        if (lineIdx != idxToInsert - 1)
        {
            return lineTxt;
        }

        if (lineTxt.EndsWith(','))
        {
            return lineTxt;
        }

        return $"{lineTxt},";
    }

    #endregion
}
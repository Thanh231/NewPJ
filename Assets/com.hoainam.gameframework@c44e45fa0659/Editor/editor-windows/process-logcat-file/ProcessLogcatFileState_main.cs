using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ProcessLogcatFileState_main : EditorWindowState
{
    #region UI

    private EditorFilePicker chooseFile = new(new List<string>() { "log", "txt" }, "choose logcat file: ");
    private string lineContainTextToRemove = "";
    
    public override void OnDraw()
    {
        chooseFile.Draw();
        
        EditorGUILayout.BeginHorizontal();

        lineContainTextToRemove =
            EditorGUILayoutExtension.FitTextField("line contain text: ", lineContainTextToRemove);
        if (EditorGUILayoutExtension.FitButton("remove"))
        {
            RemoveLinesContainText(chooseFile.PickedPath, lineContainTextToRemove);
            StaticUtilsEditor.DisplayDialog("remove done");
        }

        EditorGUILayout.EndHorizontal();
        
        if (GUILayout.Button("view tags summary"))
        {
            ViewTagsSummary(chooseFile.PickedPath);
        }

        if (GUILayout.Button("view unity logs"))
        {
            ViewUnityLogs(chooseFile.PickedPath);
        }
    }

    #endregion

    #region handle logic

    private void RemoveLinesContainText(string filePath, string text)
    {
        var sb = new StringBuilder();
        StaticUtils.ReadTextFileIntoLines(filePath, (line, _) =>
        {
            if (!line.Contains(text))
            {
                sb.AppendLine(line);
            }
        }, true);
        StaticUtils.WriteTextFile(filePath, sb.ToString(), true);
    }

    private void ViewTagsSummary(string filePath)
    {
        //build list of tags and count
        var dicTag = new Dictionary<string, int>();
        StaticUtils.ReadTextFileIntoLines(filePath, (line, _) =>
        {
            var tag = GetTagFromLogcatLine(line);
            if (!string.IsNullOrEmpty(tag))
            {
                if (dicTag.ContainsKey(tag))
                {
                    dicTag[tag]++;
                }
                else
                {
                    dicTag.Add(tag, 1);
                }
            }
        }, true);
        var listTags = dicTag.ToList();
        listTags.Sort((x, y) => y.Value.CompareTo(x.Value));

        //output tags and count
        var sb = new StringBuilder();
        foreach (var kv in listTags)
        {
            sb.AppendLine($"{kv.Key} {kv.Value}");
        }

        ShowResultToUser(sb.ToString());
    }
    
    private void ViewUnityLogs(string filePath)
    {
        var sb = new StringBuilder();
        StaticUtils.ReadTextFileIntoLines(filePath, (line, _) =>
        {
            var tag = GetTagFromLogcatLine(line);
            if (tag != null && tag.Contains("Unity"))
            {
                sb.AppendLine(line);
            }
        }, true);

        ShowResultToUser(sb.ToString());
    }

    #endregion

    #region common utils

    private static string GetTagFromLogcatLine(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return null;
        }

        var priorityCharIndex = line.IndexOfAny(new[] { 'V', 'D', 'I', 'W', 'E', 'F', 'S', '?' });
        if (line[priorityCharIndex - 1] != ' ' || line[priorityCharIndex + 1] != ' ')
        {
            throw new Exception($"line \"{line}\" don't have a valid priority");
        }

        var startTagIndex = priorityCharIndex + 2;
        var endTagIndex = line.IndexOf(':', startTagIndex);
        return line.Substring(startTagIndex, endTagIndex - startTagIndex + 1);
    }
    
    private static void ShowResultToUser(string content)
    {
        var path = StaticUtilsEditor.RandomATempPath("txt");
        StaticUtils.WriteTextFile(path, content, true);
        Process.Start(path);
    }

    #endregion
}
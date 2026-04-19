
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LoadingStepLogger
{
    //in android mono build, time from logo screen to the first scene is very long,
    //this not happen in il2cpp build.
    
    private static Dictionary<string, float> dicBeginTime = new();
    private static Dictionary<string, float> dicEndTime = new();

    public static void BeginStep(string stepName)
    {
        if (!dicBeginTime.ContainsKey(stepName))
        {
            dicBeginTime.Add(stepName, Time.realtimeSinceStartup);
        }
    }

    public static void EndStep(string stepName, bool isEnd = false)
    {
        if (!dicEndTime.ContainsKey(stepName))
        {
            dicEndTime.Add(stepName, Time.realtimeSinceStartup);
        }
        
        if (isEnd)
        {
            Report();
        }
    }

    private static void Report()
    {
        var lEndKeys = dicEndTime.Keys;
        var sb = new StringBuilder();
        foreach (var key in lEndKeys)
        {
            var dt = dicBeginTime.ContainsKey(key) ? dicEndTime[key] - dicBeginTime[key] : dicEndTime[key];
            sb.AppendLine($"{key},{dt}");
        }

        Debug.Log($"[loading step] report:\n{sb}");
    }
}


using UnityEngine;

public static partial class StaticUtils
{
    public static void LogFramework(string msg)
    {
        if (!GameFrameworkConfig.instance.enableLogFramework)
        {
            return;
        }

        Debug.Log($"[LogFramework]{msg}");
    }

    public static void LogErrorFramework(string msg)
    {
        if (!GameFrameworkConfig.instance.enableLogFramework)
        {
            return;
        }

        Debug.LogError($"[LogFramework]{msg}");
    }
}


using System;
using System.Text.RegularExpressions;
using UnityEngine;

public partial class LogFileReader_standalone
{
    private void FinishAnItem()
    {
        processingLogItem.EndProcess();

        if (string.IsNullOrEmpty(processingLogItem.message) && string.IsNullOrEmpty(processingLogItem.stacktrace))
        {
            return;
        }

        DetectLogType();
        logItems.Add(processingLogItem);
    }
    
    private void DetectLogType()
    {
        if (DetectLogType_exception())
        {
            return;
        }
        
        var lLines = processingLogItem.stacktrace.Split('\n');
        if (lLines.Length < 6)
        {
            throw new Exception($"stacktrace too short\nmessage={processingLogItem.message}\nstacktrace={processingLogItem.stacktrace}\n-------------------------------------------");
        }

        var line = lLines[5];
        var match = Regex.Match(line, @"^UnityEngine.Debug:(.*) \(.*\)");
        if (match.Success)
        {
            var funcName = match.Groups[1].Value;
            processingLogItem.logType = funcName switch
            {
                "Log" => LogType.Log,
                "LogFormat" => LogType.Log,
                "LogWarning" => LogType.Warning,
                "LogWarningFormat" => LogType.Warning,
                "LogAssertion" => LogType.Assert,
                "LogAssertionFormat" => LogType.Assert,
                "LogError" => LogType.Error,
                "LogErrorFormat" => LogType.Error,
                "LogException" => LogType.Exception,
            };
        }
        else
        {
            throw new Exception($"stacktrace missing log type info\nmessage={processingLogItem.message}\nstacktrace={processingLogItem.stacktrace}\n-------------------------------------------");
        }
    }

    private bool DetectLogType_exception()
    {
        var lLines = processingLogItem.stacktrace.Split('\n');
        if (lLines.Length == 0)
        {
            throw new Exception(
                $"stacktrace is empty\nmessage={processingLogItem.message}\nstacktrace={processingLogItem.stacktrace}\n-------------------------------------------");
        }

        var line = lLines[0];
        var isException = Regex.IsMatch(line, "^  at .*");
        if (isException)
        {
            processingLogItem.logType = IsDotweenException() ? LogType.Warning : LogType.Error;
        }

        return isException;
    }

    private bool IsDotweenException()
    {
        return Regex.IsMatch(processingLogItem.message, "^<color=.*><b>DOTWEEN.*");
    }
}

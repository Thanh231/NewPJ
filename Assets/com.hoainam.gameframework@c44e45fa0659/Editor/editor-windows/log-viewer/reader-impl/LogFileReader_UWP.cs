
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class LogFileReader_UWP:ILogFileReader
{
    private List<LogFileItem> logItems = new List<LogFileItem>();
    private LogFileItem processingLogItem = null;
    private bool isReadingMessage = true;
    
    public void ProcessLine(string line)
    {
        if (IsStartItem(line))
        {
            StartItem();
        }
        else if (processingLogItem != null)
        {
            ProcessItem(line);
        }
    }

    private void StartItem()
    {
        if (processingLogItem != null)
        {
            if ((int)processingLogItem.logType == -1)
            {
                processingLogItem.logType = LogType.Log;
            }

            processingLogItem.EndProcess();
            logItems.Add(processingLogItem);
        }
        isReadingMessage = true;
        processingLogItem = new LogFileItem();
    }

    private void ProcessItem(string line)
    {
        if (isReadingMessage && IsStartCallStack(line))
        {
            isReadingMessage = false;
        }

        if (isReadingMessage)
        {
            processingLogItem.messageSB.AppendLine(line);
        }
        else
        {
            ProcessLogType(line);
            processingLogItem.stacktraceSB.AppendLine(line);
        }
    }

    private void ProcessLogType(string line)
    {
        var regex=Regex.Match(line, @"^UnityEngine.Debug:(.*)\(.*\)");
        if(regex.Success)
        {
            var logType = regex.Groups[1].Value;
            processingLogItem.logType = logType switch
            {
                "Log" => LogType.Log,
                "LogError" => LogType.Error,
                "LogWarning" => LogType.Warning,
                _ => (LogType)(-1)
            };
        }
    }

    private bool IsStartCallStack(string line)
    {
        var regex = Regex.Match(line, @"^UnityEngine.Logger:(.*)\(.*\)");
        if (regex.Success)
        {
            var logType = regex.Groups[1].Value;
            processingLogItem.logType = logType switch
            {
                "Log" => LogType.Log,
                "LogError" => LogType.Error,
                "LogWarning" => LogType.Warning,
                _ => (LogType)(-1)
            };
            
            return true;
        }

        return false;
    }
    
    private bool IsStartItem(string line)
    {
        line = line.Trim();
        return string.IsNullOrEmpty(line) || Regex.IsMatch(line, @"^Initialize engine version: .* \(.*\)");
    }

    public List<LogFileItem> GetLogItems()
    {
        return logItems;
    }
}

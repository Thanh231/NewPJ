
using System;
using System.Text;
using UnityEngine;

public class LogFileItem
{
    public LogType logType = (LogType)(-1);
    public string message;
    public string stacktrace;

    public StringBuilder messageSB = new StringBuilder();
    public StringBuilder stacktraceSB = new StringBuilder();
	
    public LogFileItem()
    {

    }

    public LogFileItem(string logType, string msg)
    {
        this.logType = StaticUtils.StringToEnum<LogType>(logType);
        messageSB.Append(msg).Append('\n');
    }

    public override bool Equals(object obj)
    {
        var other = (LogFileItem)obj;
        return logType == other.logType && message.Equals(other.message) && stacktrace.Equals(other.stacktrace);
    }

    public void EndProcess()
    {
        message = messageSB.ToString();
        stacktrace = stacktraceSB.ToString();

        message = message.Trim('\n');
        stacktrace = stacktrace.Trim('\n');
    }

    public void EndProcess_exception(string txt)
    {
        var idx = txt.IndexOf("  at ");
        if (idx < 0 || !txt.Contains("Exception"))
        {
            throw new Exception($"this passage is invalid exception:\n{txt}");
        }

        logType = LogType.Error;
        message = txt.Substring(0, idx);
        stacktrace = txt.Substring(idx);
    }
}

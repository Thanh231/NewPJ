
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class LogFileReader_standalone
{
    private bool isReadingMessage = true;

    private List<string> listBeginStacktraceMsg = new List<string>()
    {
        @"UnityEngine.Debug:ExtractStackTraceNoAlloc \(byte\*,int,string\)",
        "^  at .*",
    };
    
    private void ProcessLine_buildItem(string line)
    {
        if (MeetStackTrace(line))
        {
            isReadingMessage = false;
        }
        
        if (isReadingMessage)
        {
            processingLogItem.messageSB.Append(line).Append('\n');
        }
        else
        {
            processingLogItem.stacktraceSB.Append(line).Append('\n');
        }
    }

    private bool MeetStackTrace(string line)
    {
        foreach (var msg in listBeginStacktraceMsg)
        {
            if (Regex.IsMatch(line, msg))
            {
                return true;
            }
        }
        return false;
    }

    private void ResetBuildingItem()
    {
        isReadingMessage = true;
        processingLogItem = new LogFileItem();
    }
}


using System.Collections.Generic;

public interface ILogFileReader
{
    void ProcessLine(string line);
    List<LogFileItem> GetLogItems();
}

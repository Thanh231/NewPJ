using System.Collections.Generic;
using System.Text.RegularExpressions;

public class LogFileReader_mobileConsole : ILogFileReader
{
	private List<LogFileItem> logItems = new List<LogFileItem>();
	private LogFileItem processingLogItem = null;
	private bool isReadingMessage = true;

	public List<LogFileItem> GetLogItems()
	{
		AddItemToLogList();
		return logItems;
	}

	public void ProcessLine(string line)
	{
		var match = Regex.Match(line, "^\\[(.*)\\] \\[.*\\]  (.*)");
		if (match.Success)
		{
			if (processingLogItem != null)
			{
				AddItemToLogList();
			}
			isReadingMessage = true;
			processingLogItem = new LogFileItem(match.Groups[1].Value, match.Groups[2].Value);
		}
		else if (processingLogItem != null)
		{
			if (isReadingMessage)
			{
				if (line.Equals("------------------"))
				{
					isReadingMessage = false;
				}
				else
				{
					processingLogItem.messageSB.Append(line).Append('\n');
				}
			}
			else
			{
				processingLogItem.stacktraceSB.Append(line).Append('\n');
			}
		}
	}

	private void AddItemToLogList()
	{
		processingLogItem.EndProcess();
		logItems.Add(processingLogItem);
	}
}
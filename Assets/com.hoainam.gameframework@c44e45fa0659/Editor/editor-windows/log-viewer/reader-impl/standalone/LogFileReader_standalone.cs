using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class LogFileReader_standalone : ILogFileReader
{
	private List<LogFileItem> logItems = new();
	private LogFileItem processingLogItem = new();

	private List<string> listEndItemMsg = new List<string>()
	{
		@"^\(Filename: .* Line: \d+\)",
		@"DG.Tweening.Core.DOTweenComponent:Update \(\)",
	};
	private const string endReportMsg = "[Physics::Module] Cleanup current backned.";

	private bool endedReport;
	
	public List<LogFileItem> GetLogItems()
	{
		return logItems;
	}

	public void ProcessLine(string line)
	{
		if (endedReport)
		{
			return;
		}
		
		if (SkipLine(line))
		{
			return;
		}

		if (MeetEndItemMsg(line))
		{
			FinishAnItem();
			ResetBuildingItem();
			
			return;
		}

		if (line.Equals(endReportMsg))
		{
			FinishAnItem();
			endedReport = true;
			return;
		}
		
		ProcessLine_buildItem(line);
	}

	private bool MeetEndItemMsg(string line)
	{
		foreach (var pattern in listEndItemMsg)
		{
			if (Regex.IsMatch(line, pattern))
			{
				return true;
			}
		}
		return false;
	}
}
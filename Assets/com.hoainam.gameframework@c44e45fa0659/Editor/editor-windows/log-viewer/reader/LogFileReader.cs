using System.Collections.Generic;
using System.Text.RegularExpressions;

public abstract class LogFileReader
{
	public static List<LogFileItem> Read(string filepath)
	{
		ILogFileReader readerImp = null;

		StaticUtils.ReadTextFileIntoLines(filepath, (line, lineIdx) =>
		{
			if (readerImp == null)
			{
				if (line.Equals("--- App Info ---"))
				{
					readerImp = new LogFileReader_mobileConsole();
				}
				else
				{
					var isUWP = Regex.IsMatch(line, @"^Logging to .*\/TempState\/UnityPlayer.log");
					if (isUWP)
					{
						readerImp = new LogFileReader_UWP();
					}
					else
					{
						readerImp = new LogFileReader_standalone();
					}
				}
			}
			else
			{
				readerImp.ProcessLine(line);
			}
		}, isAbsolutePath: true);

		return readerImp.GetLogItems();
	}
}
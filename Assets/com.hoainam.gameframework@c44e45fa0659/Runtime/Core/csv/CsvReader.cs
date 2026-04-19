using System.Collections.Generic;
using System.IO;
using System.Text;

public class CsvReader
{
    public class ProcessLineResult
    {
        public List<string> finalResult = null;
        public StringBuilder processingCell = null;
    }

    private StreamReader streamReader;

    public CsvReader(StreamReader stream)
    {
        streamReader = stream;
    }

    public List<string> ReadRecord()
    {
        if (streamReader.EndOfStream)
        {
            return null;
        }

        var processLineResult = new ProcessLineResult();
        do
        {
            var lineStr = streamReader.ReadLine();
            if (!string.IsNullOrEmpty(lineStr) || processLineResult.processingCell != null)
            {
                ProcessLine(lineStr, processLineResult);
            }
        } while (processLineResult.processingCell != null);

        return processLineResult.finalResult;
    }

    private void ProcessLine(string line, ProcessLineResult processLineResult)
    {
        if (processLineResult.finalResult == null)
        {
            processLineResult.finalResult = new List<string>();
        }

        var sb = new StringBuilder(
            processLineResult.processingCell != null ? processLineResult.processingCell.ToString() : "");
        var withinQuote = processLineResult.processingCell != null;
        var i = 0;
        while (i < line.Length)
        {
            switch (line[i])
            {
                case ',':
                    if (withinQuote)
                    {
                        sb.Append(line[i]);
                    }
                    else
                    {
                        processLineResult.finalResult.Add(sb.ToString());
                        sb.Clear();
                    }
                    break;
                case '\"':
                    if (withinQuote)
                    {
                        if (i + 1 < line.Length && line[i + 1] == '\"')
                        {
                            sb.Append(line[i]);
                            i++;
                        }
                        else
                        {
                            withinQuote = false;
                        }
                    }
                    else
                    {
                        withinQuote = true;
                    }
                    break;
                default:
                    sb.Append(line[i]);
                    break;
            }
            i++;
        }

        if (withinQuote)
        {
            processLineResult.processingCell = new StringBuilder(sb.ToString());
            processLineResult.processingCell.Append('\n');
        }
        else
        {
            processLineResult.finalResult.Add(sb.ToString());
            processLineResult.processingCell = null;
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Text;

public class CsvWriter
{
    private StreamWriter streamWriter;

    public CsvWriter(StreamWriter stream)
    {
        streamWriter = stream;
    }

    public void WriteRecord(List<string> record)
    {
        for (var i = 0; i < record.Count; i++)
        {
            if (i < record.Count - 1)
            {
                streamWriter.Write($"{ProcessCell(record[i])},");
            }
            else
            {
                streamWriter.WriteLine(ProcessCell(record[i]));
            }
        }
    }

    private string ProcessCell(string cell)
    {
        var needQuote = false;
        var sb = new StringBuilder();

        foreach (var c in cell)
        {
            sb.Append(c);
            if (c == '\"')
            {
                sb.Append(c);
                needQuote = true;
            }
        }

        needQuote |= cell.Contains(',') | cell.Contains('\n');

        if (needQuote)
        {
            return $"\"{sb}\"";
        }
        else
        {
            return sb.ToString();
        }
    }
}

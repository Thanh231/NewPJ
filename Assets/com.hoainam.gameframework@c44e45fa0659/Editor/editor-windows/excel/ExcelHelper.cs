
using System.Collections.Generic;
using System.IO;

public class ExcelHelper
{
    #region internal utils

    private static List<List<string>> ReadExcel(string filename)
    {
        var result = new List<List<string>>();
        using (var streamReader = new StreamReader(filename))
        {
            var csvReader = new CsvReader(streamReader);
            while (true)
            {
                var record = csvReader.ReadRecord();
                if (record == null)
                {
                    break;
                }

                result.Add(record);
            }
        }
        return result;
    }
    
    private static void WriteExcel(List<List<string>> data, string filename)
    {
        using (var streamWriter = new StreamWriter(filename))
        {
            var csvWriter = new CsvWriter(streamWriter);
            foreach (var record in data)
            {
                csvWriter.WriteRecord(record);
            }
        }
    }

    #endregion

    #region sort by date
    
    public enum SortByDateOrder
    {
        EarlierToLater,
        LaterToEarlier,
    }

    public class VietnamDate
    {
        public bool IsValid { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public int ValidAsNumber => IsValid ? 1 : -1;
        public int DateAsNumber => Year * 10000 + Month * 100 + Day;

        public VietnamDate(string str)
        {
            var l = str.Split('/');
            IsValid = l.Length == 3;
            if (IsValid)
            {
                Day = StaticUtils.StringToInt(l[0]);
                Month = StaticUtils.StringToInt(l[1]);
                Year = StaticUtils.StringToInt(l[2]);
            }
        }
    }

    private static int cacheDateColumnIndex;
    private static SortByDateOrder cacheSortByDateOrder;

    public static void SortByDate(string filenameInput, string filenameOutput, int dateColumnIndex,
        SortByDateOrder sortByDateOrder)
    {
        cacheDateColumnIndex = dateColumnIndex;
        cacheSortByDateOrder = sortByDateOrder;
        
        var data = ReadExcel(filenameInput);
        data.Sort(SortByDateCompare);
        WriteExcel(data, filenameOutput);
    }

    private static int SortByDateCompare(List<string> a, List<string> b)
    {
        var dateA = new VietnamDate(a[cacheDateColumnIndex]);
        var dateB = new VietnamDate(b[cacheDateColumnIndex]);

        if (dateA.IsValid && dateB.IsValid)
        {
            return cacheSortByDateOrder == SortByDateOrder.EarlierToLater
                ? dateA.DateAsNumber.CompareTo(dateB.DateAsNumber)
                : dateB.DateAsNumber.CompareTo(dateA.DateAsNumber);
        }
        else
        {
            return dateB.ValidAsNumber.CompareTo(dateA.ValidAsNumber);
        }
    }

    #endregion
    
    
}
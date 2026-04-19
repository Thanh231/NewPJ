
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ObservableCollections;
using R3;

public class FileStream_textWriter : IFileStream
{
    #region core

    private StreamWriter writer;
    private int level = 0;

    public FileStream_textWriter(StreamWriter writer)
    {
        this.writer = writer;
    }

    public FileStream_textWriter(StreamWriter writer, int level)
    {
        this.writer = writer;
        this.level = level;
    }

    private string indent = null;
    private string Indent
    {
        get
        {
            if (indent == null)
            {
                var sb = new StringBuilder();
                for (var i = 0; i < level; i++)
                {
                    sb.Append('\t');
                }
                indent = sb.ToString();
            }
            return indent;
        }
    }

    #endregion

    #region write primitive

    public void ReadOrWriteBool(ref bool val, string name = null)
    {
        WriteSingle(val, name, x => x.ToString());
    }

    public void ReadOrWriteInt(ref int val, string name = null)
    {
        WriteSingle(val, name, x => x.ToString());
    }

    public void ReadOrWriteLong(ref long val, string name = null)
    {
        WriteSingle(val, name, x => x.ToString());
    }

    public void ReadOrWriteFloat(ref float val, string name = null)
    {
        WriteSingle(val, name, StaticUtils.FloatToString);
    }

    public void ReadOrWriteDouble(ref double val, string name = null)
    {
        WriteSingle(val, name, StaticUtils.DoubleToString);
    }

    public void ReadOrWriteString(ref string val, string name = null)
    {
        WriteSingle(val, name, x => x);
    }

    public void ReadOrWriteDateTime(ref DateTime val, string name = null)
    {
        WriteSingle(val, name, StaticUtils.DateTimeToString);
    }

    public void ReadOrWriteEnum<T>(ref T val, string name = null) where T : struct, IConvertible
    {
        WriteSingle(val, name, x => x.ToString());
    }

    public void ReadOrWriteObj<T>(ref T val, string name = null) where T : IFileStreamObject, new()
    {
        WriteSingle_obj(val, name, this, needWriteVersion: true);
    }

    #endregion

    #region write rx property

    public void ReadOrWriteRxBool(ref ReactiveProperty<bool> val, string name = null)
    {
        WriteSingle(val.Value, name, x => x.ToString());
    }

    public void ReadOrWriteRxInt(ref ReactiveProperty<int> val, string name = null)
    {
        WriteSingle(val.Value, name, x => x.ToString());
    }

    public void ReadOrWriteRxLong(ref ReactiveProperty<long> val, string name = null)
    {
        WriteSingle(val.Value, name, x => x.ToString());
    }

    public void ReadOrWriteRxFloat(ref ReactiveProperty<float> val, string name = null)
    {
        WriteSingle(val.Value, name, StaticUtils.FloatToString);
    }

    public void ReadOrWriteRxDouble(ref ReactiveProperty<double> val, string name = null)
    {
        WriteSingle(val.Value, name, StaticUtils.DoubleToString);
    }

    public void ReadOrWriteRxString(ref ReactiveProperty<string> val, string name = null)
    {
        WriteSingle(val.Value, name, x => x);
    }

    public void ReadOrWriteRxDateTime(ref ReactiveProperty<DateTime> val, string name = null)
    {
        WriteSingle(val.Value, name, StaticUtils.DateTimeToString);
    }

    public void ReadOrWriteRxEnum<T>(ref ReactiveProperty<T> val, string name = null) where T : struct, IConvertible
    {
        WriteSingle(val.Value, name, x => x.ToString());
    }

    #endregion

    #region write encrypt

    public void ReadOrWriteEncryptInt(ref EncryptInt val, string name = null)
    {
        WriteSingle((int)val, name, x => x.ToString());
    }

    public void ReadOrWriteEncryptLong(ref EncryptLong val, string name = null)
    {
        WriteSingle((long)val, name, x => x.ToString());
    }

    public void ReadOrWriteRxEncryptInt(ref ReactiveProperty<EncryptInt> val, string name = null)
    {
        WriteSingle((int)val.Value, name, x => x.ToString());
    }

    public void ReadOrWriteRxEncryptLong(ref ReactiveProperty<EncryptLong> val, string name = null)
    {
        WriteSingle((long)val.Value, name, x => x.ToString());
    }

    #endregion

    #region write list

    public void ReadOrWriteListBool(ref List<bool> val, string name = null)
    {
        WriteIList(val, name, x => x.ToString());
    }

    public void ReadOrWriteListInt(ref List<int> val, string name = null)
    {
        WriteIList(val, name, x => x.ToString());
    }

    public void ReadOrWriteListLong(ref List<long> val, string name = null)
    {
        WriteIList(val, name, x => x.ToString());
    }

    public void ReadOrWriteListFloat(ref List<float> val, string name = null)
    {
        WriteIList(val, name, StaticUtils.FloatToString);
    }

    public void ReadOrWriteListDouble(ref List<double> val, string name = null)
    {
        WriteIList(val, name, StaticUtils.DoubleToString);
    }

    public void ReadOrWriteListString(ref List<string> val, string name = null)
    {
        WriteIList(val, name, x => x);
    }

    public void ReadOrWriteListDateTime(ref List<DateTime> val, string name = null)
    {
        WriteIList(val, name, StaticUtils.DateTimeToString);
    }

    public void ReadOrWriteListEnum<T>(ref List<T> val, string name = null) where T : struct, IConvertible
    {
        WriteIList(val, name, x => x.ToString());
    }

    public void ReadOrWriteListObj<T>(ref List<T> val, string name = null) where T : IFileStreamObject, new()
    {
        WriteIListObj(val, name);
    }

    #endregion

    #region write list of rx property

    public void ReadOrWriteListRxBool(ref List<ReactiveProperty<bool>> val, string name = null)
    {
        WriteListRX(val, name, x => x.ToString());
    }

    public void ReadOrWriteListRxInt(ref List<ReactiveProperty<int>> val, string name = null)
    {
        WriteListRX(val, name, x => x.ToString());
    }

    public void ReadOrWriteListRxLong(ref List<ReactiveProperty<long>> val, string name = null)
    {
        WriteListRX(val, name, x => x.ToString());
    }

    public void ReadOrWriteListRxFloat(ref List<ReactiveProperty<float>> val, string name = null)
    {
        WriteListRX(val, name, StaticUtils.FloatToString);
    }

    public void ReadOrWriteListRxDouble(ref List<ReactiveProperty<double>> val, string name = null)
    {
        WriteListRX(val, name, StaticUtils.DoubleToString);
    }

    public void ReadOrWriteListRxString(ref List<ReactiveProperty<string>> val, string name = null)
    {
        WriteListRX(val, name, x => x);
    }

    public void ReadOrWriteListRxDateTime(ref List<ReactiveProperty<DateTime>> val, string name = null)
    {
        WriteListRX(val, name, StaticUtils.DateTimeToString);
    }

    public void ReadOrWriteListRxEnum<T>(ref List<ReactiveProperty<T>> val, string name = null) where T : struct, IConvertible
    {
        WriteListRX(val, name, x => x.ToString());
    }

    #endregion

    #region write rx list

    public void ReadOrWriteRxListBool(ref ObservableList<bool> val, string name = null)
    {
        WriteIList(val, name, x => x.ToString());
    }

    public void ReadOrWriteRxListInt(ref ObservableList<int> val, string name = null)
    {
        WriteIList(val, name, x => x.ToString());
    }

    public void ReadOrWriteRxListLong(ref ObservableList<long> val, string name = null)
    {
        WriteIList(val, name, x => x.ToString());
    }

    public void ReadOrWriteRxListFloat(ref ObservableList<float> val, string name = null)
    {
        WriteIList(val, name, StaticUtils.FloatToString);
    }

    public void ReadOrWriteRxListDouble(ref ObservableList<double> val, string name = null)
    {
        WriteIList(val, name, StaticUtils.DoubleToString);
    }

    public void ReadOrWriteRxListString(ref ObservableList<string> val, string name = null)
    {
        WriteIList(val, name, x => x);
    }

    public void ReadOrWriteRxListDateTime(ref ObservableList<DateTime> val, string name = null)
    {
        WriteIList(val, name, StaticUtils.DateTimeToString);
    }

    public void ReadOrWriteRxListEnum<T>(ref ObservableList<T> val, string name = null) where T : struct, IConvertible
    {
        WriteIList(val, name, x => x.ToString());
    }

    public void ReadOrWriteRxListObj<T>(ref ObservableList<T> val, string name = null) where T : IFileStreamObject, new()
    {
        WriteIListObj(val, name);
    }

    #endregion

    #region write dictionary

    public void ReadOrWriteDicIntObj<T>(ref Dictionary<int, T> val, string name = null) where T : IFileStreamObject, new()
    {
        WriteDictionaryObj(val, name, x => x.ToString());
    }

    public void ReadOrWriteDicStringObj<T>(ref Dictionary<string, T> val, string name = null) where T : IFileStreamObject, new()
    {
        WriteDictionaryObj(val, name, x => x);
    }

    public void ReadOrWriteDicEnumObj<TEnum, TObj>(ref Dictionary<TEnum, TObj> val, string name = null)
        where TEnum : struct, IConvertible
        where TObj : IFileStreamObject, new()
    {
        WriteDictionaryObj(val, name, x => x.ToString());
    }

    #endregion

    #region utils

    private void WriteSingle<T>(T val, string name, Func<T, string> toStringFunc)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new Exception("can't text serialize because name of variable is null");
        }

        var strVal = toStringFunc.Invoke(val);
        writer.WriteLine($"{Indent}{name}={strVal}");
    }

    private void WriteSingle_obj<T>(T val, string name, FileStream_textWriter stream, bool needWriteVersion) where T : IFileStreamObject, new()
    {
        var version = val.ModelVersion;
        writer.WriteLine($"{stream.Indent}{name}");

        var deeperStream = new FileStream_textWriter(stream.writer, stream.level + 1);

        if (needWriteVersion)
        {
            deeperStream.ReadOrWriteInt(ref version, "version");
        }
        val.ReadOrWrite(deeperStream, version);
    }

    private void WriteListRX<T>(List<ReactiveProperty<T>> l, string name, Func<T, string> toStringFunc)
    {
        WriteIList(l, name, x => toStringFunc(x.Value));
    }

    private void WriteIList<T>(IList<T> l, string name, Func<T, string> toStringFunc)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new Exception("can't text serialize because name of variable is null");
        }

        writer.Write($"{Indent}{name}=[");
        for (var i = 0; i < l.Count; i++)
        {
            writer.Write(toStringFunc.Invoke(l[i]));
            if (i < l.Count - 1)
            {
                writer.Write(',');
            }
        }
        writer.WriteLine(']');
    }

    private void WriteIListObj<T>(IList<T> l, string name) where T : IFileStreamObject, new()
    {
        writer.WriteLine($"{Indent}{name}[{l.Count}]");

        var deeperStream = new FileStream_textWriter(writer, level + 1);

        if (l.Count > 0)
        {
            var version = l[0].ModelVersion;
            deeperStream.ReadOrWriteInt(ref version, "version");
        }

        for (var i = 0; i < l.Count; i++)
        {
            WriteSingle_obj(l[i], $"arrayElement_{i}", deeperStream, needWriteVersion:false);
        }
    }

    private void WriteDictionaryObj<TKey, TObj>(Dictionary<TKey, TObj> dic, string name, Func<TKey, string> keyToString) where TObj : IFileStreamObject, new()
    {
        writer.WriteLine($"{Indent}{name}{{{dic.Count}}}");

        var deeperStream = new FileStream_textWriter(writer, level + 1);

        if (dic.Count > 0)
        {
            var version = dic.First().Value.ModelVersion;
            deeperStream.ReadOrWriteInt(ref version, "version");
        }

        foreach (var pair in dic)
        {
            WriteSingle_obj(pair.Value, keyToString.Invoke(pair.Key), deeperStream, needWriteVersion:false);
        }
    }

    #endregion
}

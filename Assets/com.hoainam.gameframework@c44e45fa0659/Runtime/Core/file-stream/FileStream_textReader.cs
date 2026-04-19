
using System;
using System.Collections.Generic;
using System.IO;
using ObservableCollections;
using R3;

public class FileStream_textReader: IFileStream
{
    #region core

    private StreamReader reader;
    public FileStream_textReader(StreamReader reader)
    {
        this.reader = reader;
    }

    #endregion

    #region read primitive

    public void ReadOrWriteBool(ref bool val, string name = null)
    {
        val = ReadSingle(StaticUtils.StringToBool);
    }

    public void ReadOrWriteInt(ref int val, string name = null)
    {
        val = ReadSingle(StaticUtils.StringToInt);
    }

    public void ReadOrWriteLong(ref long val, string name = null)
    {
        val = ReadSingle(StaticUtils.StringToLong);
    }

    public void ReadOrWriteFloat(ref float val, string name = null)
    {
        val = ReadSingle(StaticUtils.StringToFloat);
    }

    public void ReadOrWriteDouble(ref double val, string name = null)
    {
        val = ReadSingle(StaticUtils.StringToDouble);
    }

    public void ReadOrWriteString(ref string val, string name = null)
    {
        val = ReadSingle(x => x);
    }

    public void ReadOrWriteDateTime(ref DateTime val, string name = null)
    {
        val = ReadSingle(StaticUtils.StringToDateTime);
    }

    public void ReadOrWriteEnum<T>(ref T val, string name = null) where T : struct, IConvertible
    {
        val = ReadSingle(StaticUtils.StringToEnum<T>);
    }

    public void ReadOrWriteObj<T>(ref T val, string name = null) where T : IFileStreamObject, new()
    {
        val = ReadSingle_obj<T>(out string _);
    }

    #endregion

    #region read rx property

    public void ReadOrWriteRxBool(ref ReactiveProperty<bool> val, string name = null)
    {
        val = new ReactiveProperty<bool>(ReadSingle(StaticUtils.StringToBool));
    }

    public void ReadOrWriteRxInt(ref ReactiveProperty<int> val, string name = null)
    {
        val = new ReactiveProperty<int>(ReadSingle(StaticUtils.StringToInt));
    }

    public void ReadOrWriteRxLong(ref ReactiveProperty<long> val, string name = null)
    {
        val = new ReactiveProperty<long>(ReadSingle(StaticUtils.StringToLong));
    }

    public void ReadOrWriteRxFloat(ref ReactiveProperty<float> val, string name = null)
    {
        val = new ReactiveProperty<float>(ReadSingle(StaticUtils.StringToFloat));
    }

    public void ReadOrWriteRxDouble(ref ReactiveProperty<double> val, string name = null)
    {
        val = new ReactiveProperty<double>(ReadSingle(StaticUtils.StringToDouble));
    }

    public void ReadOrWriteRxString(ref ReactiveProperty<string> val, string name = null)
    {
        val = new ReactiveProperty<string>(ReadSingle(x => x));
    }

    public void ReadOrWriteRxDateTime(ref ReactiveProperty<DateTime> val, string name = null)
    {
        val = new ReactiveProperty<DateTime>(ReadSingle(StaticUtils.StringToDateTime));
    }

    public void ReadOrWriteRxEnum<T>(ref ReactiveProperty<T> val, string name = null) where T : struct, IConvertible
    {
        val = new ReactiveProperty<T>(ReadSingle(StaticUtils.StringToEnum<T>));
    }

    #endregion

    #region read encrypt

    public void ReadOrWriteEncryptInt(ref EncryptInt val, string name = null)
    {
        val = ReadSingle(StaticUtils.StringToInt);
    }

    public void ReadOrWriteEncryptLong(ref EncryptLong val, string name = null)
    {
        val = ReadSingle(StaticUtils.StringToLong);
    }

    public void ReadOrWriteRxEncryptInt(ref ReactiveProperty<EncryptInt> val, string name = null)
    {
        val = new ReactiveProperty<EncryptInt>(ReadSingle(StaticUtils.StringToInt));
    }

    public void ReadOrWriteRxEncryptLong(ref ReactiveProperty<EncryptLong> val, string name = null)
    {
        val = new ReactiveProperty<EncryptLong>(ReadSingle(StaticUtils.StringToLong));
    }

    #endregion

    #region read list

    public void ReadOrWriteListBool(ref List<bool> val, string name = null)
    {
        val = ReadList(StaticUtils.StringToBool);
    }

    public void ReadOrWriteListInt(ref List<int> val, string name = null)
    {
        val = ReadList(StaticUtils.StringToInt);
    }

    public void ReadOrWriteListLong(ref List<long> val, string name = null)
    {
        val = ReadList(StaticUtils.StringToLong);
    }

    public void ReadOrWriteListFloat(ref List<float> val, string name = null)
    {
        val = ReadList(StaticUtils.StringToFloat);
    }

    public void ReadOrWriteListDouble(ref List<double> val, string name = null)
    {
        val = ReadList(StaticUtils.StringToDouble);
    }

    public void ReadOrWriteListString(ref List<string> val, string name = null)
    {
        val = ReadList(x => x);
    }

    public void ReadOrWriteListDateTime(ref List<DateTime> val, string name = null)
    {
        val = ReadList(StaticUtils.StringToDateTime);
    }

    public void ReadOrWriteListEnum<T>(ref List<T> val, string name = null) where T : struct, IConvertible
    {
        val = ReadList(StaticUtils.StringToEnum<T>);
    }

    public void ReadOrWriteListObj<T>(ref List<T> val, string name = null) where T : IFileStreamObject, new()
    {
        val = ReadList_obj<T>();
    }

    #endregion

    #region read list of rx property

    public void ReadOrWriteListRxBool(ref List<ReactiveProperty<bool>> val, string name = null)
    {
        val = ReadListRX(StaticUtils.StringToBool);
    }

    public void ReadOrWriteListRxInt(ref List<ReactiveProperty<int>> val, string name = null)
    {
        val = ReadListRX(StaticUtils.StringToInt);
    }

    public void ReadOrWriteListRxLong(ref List<ReactiveProperty<long>> val, string name = null)
    {
        val = ReadListRX(StaticUtils.StringToLong);
    }

    public void ReadOrWriteListRxFloat(ref List<ReactiveProperty<float>> val, string name = null)
    {
        val = ReadListRX(StaticUtils.StringToFloat);
    }

    public void ReadOrWriteListRxDouble(ref List<ReactiveProperty<double>> val, string name = null)
    {
        val = ReadListRX(StaticUtils.StringToDouble);
    }

    public void ReadOrWriteListRxString(ref List<ReactiveProperty<string>> val, string name = null)
    {
        val = ReadListRX(x => x);
    }

    public void ReadOrWriteListRxDateTime(ref List<ReactiveProperty<DateTime>> val, string name = null)
    {
        val = ReadListRX(StaticUtils.StringToDateTime);
    }

    public void ReadOrWriteListRxEnum<T>(ref List<ReactiveProperty<T>> val, string name = null) where T : struct, IConvertible
    {
        val = ReadListRX(StaticUtils.StringToEnum<T>);
    }

    #endregion

    #region read rx list

    public void ReadOrWriteRxListBool(ref ObservableList<bool> val, string name = null)
    {
        val = ReadRXList(StaticUtils.StringToBool);
    }

    public void ReadOrWriteRxListInt(ref ObservableList<int> val, string name = null)
    {
        val = ReadRXList(StaticUtils.StringToInt);
    }

    public void ReadOrWriteRxListLong(ref ObservableList<long> val, string name = null)
    {
        val = ReadRXList(StaticUtils.StringToLong);
    }

    public void ReadOrWriteRxListFloat(ref ObservableList<float> val, string name = null)
    {
        val = ReadRXList(StaticUtils.StringToFloat);
    }

    public void ReadOrWriteRxListDouble(ref ObservableList<double> val, string name = null)
    {
        val = ReadRXList(StaticUtils.StringToDouble);
    }

    public void ReadOrWriteRxListString(ref ObservableList<string> val, string name = null)
    {
        val = ReadRXList(x => x);
    }

    public void ReadOrWriteRxListDateTime(ref ObservableList<DateTime> val, string name = null)
    {
        val = ReadRXList(StaticUtils.StringToDateTime);
    }

    public void ReadOrWriteRxListEnum<T>(ref ObservableList<T> val, string name = null) where T : struct, IConvertible
    {
        val = ReadRXList(StaticUtils.StringToEnum<T>);
    }

    public void ReadOrWriteRxListObj<T>(ref ObservableList<T> val, string name = null) where T : IFileStreamObject, new()
    {
        val = ReadRXList_obj<T>();
    }

    #endregion

    #region read dictionary

    public void ReadOrWriteDicIntObj<T>(ref Dictionary<int, T> val, string name = null) where T : IFileStreamObject, new()
    {
        val = ReadDictionary<int, T>(StaticUtils.StringToInt);
    }

    public void ReadOrWriteDicStringObj<T>(ref Dictionary<string, T> val, string name = null) where T : IFileStreamObject, new()
    {
        val = ReadDictionary<string, T>(x => x);
    }

    public void ReadOrWriteDicEnumObj<TEnum, TObj>(ref Dictionary<TEnum, TObj> val, string name = null)
        where TEnum : struct, IConvertible
        where TObj : IFileStreamObject, new()
    {
        val = ReadDictionary<TEnum, TObj>(StaticUtils.StringToEnum<TEnum>);
    }

    #endregion

    #region utils

    private T ReadSingle<T>(Func<string, T> fromStringFunc)
    {
        var line = reader.ReadLine();
        var separatorIdx = line.IndexOf('=');
        return fromStringFunc(line.Substring(separatorIdx + 1));
    }

    private T ReadSingle_obj<T>(out string objName, int version = -1) where T : IFileStreamObject, new()
    {
        objName = reader.ReadLine();
        objName = objName.TrimStart('\t');

        if (version < 0)
        {
            ReadOrWriteInt(ref version);
        }
        
        var result = new T();
        result.ReadOrWrite(this, version);

        return result;
    }

    private List<T> ReadList<T>(Func<string, T> fromStringFunc)
    {
        var result = new List<T>();

        var line = reader.ReadLine();
        var separatorIdx = line.IndexOf('=');
        var payload = line.Substring(separatorIdx + 2, line.Length - separatorIdx - 3);

        if (string.IsNullOrEmpty(payload))
        {
            return result;
        }

        if (!payload.Contains(','))
        {
            result.Add(fromStringFunc(payload));
            return result;
        }

        var l = payload.Split(',');
        foreach (var i in l)
        {
            result.Add(fromStringFunc(i));
        }

        return result;
    }

    private ObservableList<T> ReadRXList<T>(Func<string, T> fromStringFunc)
    {
        var tList = ReadList(fromStringFunc);
        return new ObservableList<T>(tList);
    }

    private List<ReactiveProperty<T>> ReadListRX<T>(Func<string, T> fromStringFunc)
    {
        var tList = ReadList(fromStringFunc);

        var result = new List<ReactiveProperty<T>>();
        foreach (var i in tList)
        {
            result.Add(new ReactiveProperty<T>(i));
        }
        return result;
    }   

    private List<T> ReadList_obj<T>() where T : IFileStreamObject, new()
    {
        var header = reader.ReadLine();
        var openBraceIdx = header.IndexOf('[');
        var closeBraceIdx = header.IndexOf(']');
        var arrCount = StaticUtils.StringToInt(header.Substring(openBraceIdx + 1, closeBraceIdx - openBraceIdx - 1));

        var result = new List<T>();
        if (arrCount > 0)
        {
            var version = -1;
            ReadOrWriteInt(ref version);

            for (var i = 0; i < arrCount; i++)
            {
                var obj = ReadSingle_obj<T>(out string _, version);
                result.Add(obj);
            }
        }

        return result;
    }

    private ObservableList<T> ReadRXList_obj<T>() where T : IFileStreamObject, new()
    {
        var tList = ReadList_obj<T>();
        return new ObservableList<T>(tList);
    }

    private Dictionary<TKey, TObj> ReadDictionary<TKey, TObj>(Func<string, TKey> fromStringFunc) where TObj : IFileStreamObject, new()
    {
        var header = reader.ReadLine();
        var openBraceIdx = header.IndexOf('{');
        var closeBraceIdx = header.IndexOf('}');
        var dicCount = StaticUtils.StringToInt(header.Substring(openBraceIdx + 1, closeBraceIdx - openBraceIdx - 1));

        var result = new Dictionary<TKey, TObj>();
        if (dicCount > 0)
        {
            var version = -1;
            ReadOrWriteInt(ref version);

            for (var i = 0; i < dicCount; i++)
            {
                var obj = ReadSingle_obj<TObj>(out string key, version);
                result.Add(fromStringFunc(key), obj);
            }
        }
        
        return result;
    }

    #endregion
}

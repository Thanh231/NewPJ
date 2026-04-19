using System;
using System.Collections.Generic;
using System.IO;
using ObservableCollections;
using R3;

public class FileStream_csvReader : IFileStream
{
    private List<List<List<string>>> csvData = new List<List<List<string>>>();
    private int currentId;
    private int currentField;
    private int currentItem;

	public List<string> ListHeaders { get; set; }
	public int NumItems { get; set; }

    public FileStream_csvReader(StreamReader streamReader)
    {
        BuildCsvData(streamReader);
	}

    #region read primitive

    public void ReadOrWriteBool(ref bool val, string name = null)
    {
		val = ReadSingle(StaticUtils.StringToBool, defaultVal: false);
    }

    public void ReadOrWriteInt(ref int val, string name = null)
    {
		val = ReadSingle(StaticUtils.StringToInt, defaultVal: 0);
    }

    public void ReadOrWriteLong(ref long val, string name = null)
    {
		val = ReadSingle(StaticUtils.StringToLong, defaultVal: 0);
    }

    public void ReadOrWriteFloat(ref float val, string name = null)
    {
		val = ReadSingle(StaticUtils.StringToFloat, defaultVal: 0);
    }

    public void ReadOrWriteDouble(ref double val, string name = null)
    {
		val = ReadSingle(StaticUtils.StringToDouble, defaultVal: 0);
    }

    public void ReadOrWriteString(ref string val, string name = null)
    {
		val = ReadSingle(s => s, defaultVal: "");
    }

    public void ReadOrWriteDateTime(ref DateTime val, string name = null)
    {
		val = ReadSingle(StaticUtils.StringToDateTime, defaultVal: new DateTime());
    }

    public void ReadOrWriteEnum<T>(ref T val, string name = null) where T : struct, IConvertible
    {
        var defaultEnum = StaticUtils.IntToEnum<T>(-1);
		val = ReadSingle(StaticUtils.StringToEnum<T>, defaultVal: defaultEnum);
    }

	public void ReadOrWriteObj<T>(ref T val, string name = null) where T : IFileStreamObject, new()
	{
		val = new T();
		val.ReadOrWrite(this, -1);
	}

	#endregion

	#region read rx property

	public void ReadOrWriteRxBool(ref ReactiveProperty<bool> val, string name = null)
	{
		val = new ReactiveProperty<bool>(ReadSingle(StaticUtils.StringToBool, defaultVal: false));
	}

	public void ReadOrWriteRxInt(ref ReactiveProperty<int> val, string name = null)
	{
		val = new ReactiveProperty<int>(ReadSingle(StaticUtils.StringToInt, defaultVal: 0));
	}

	public void ReadOrWriteRxLong(ref ReactiveProperty<long> val, string name = null)
	{
		val = new ReactiveProperty<long>(ReadSingle(StaticUtils.StringToLong, defaultVal: 0));
	}

	public void ReadOrWriteRxFloat(ref ReactiveProperty<float> val, string name = null)
	{
		val = new ReactiveProperty<float>(ReadSingle(StaticUtils.StringToFloat, defaultVal: 0));
	}

	public void ReadOrWriteRxDouble(ref ReactiveProperty<double> val, string name = null)
	{
		val = new ReactiveProperty<double>(ReadSingle(StaticUtils.StringToDouble, defaultVal: 0));
	}

	public void ReadOrWriteRxString(ref ReactiveProperty<string> val, string name = null)
	{
		val = new ReactiveProperty<string>(ReadSingle(s => s, defaultVal: ""));
	}

	public void ReadOrWriteRxDateTime(ref ReactiveProperty<DateTime> val, string name = null)
	{
		val = new ReactiveProperty<DateTime>(ReadSingle(StaticUtils.StringToDateTime, defaultVal: new DateTime()));
	}

	public void ReadOrWriteRxEnum<T>(ref ReactiveProperty<T> val, string name = null) where T : struct, IConvertible
	{
		var defaultEnum = StaticUtils.IntToEnum<T>(-1);
		val = new ReactiveProperty<T>(ReadSingle(StaticUtils.StringToEnum<T>, defaultVal: defaultEnum));
	}

	#endregion

	#region read encrypt

	public void ReadOrWriteEncryptInt(ref EncryptInt val, string name = null)
	{
		val = ReadSingle(StaticUtils.StringToInt, defaultVal: 0);
	}

	public void ReadOrWriteEncryptLong(ref EncryptLong val, string name = null)
	{
		val = ReadSingle(StaticUtils.StringToLong, defaultVal: 0);
	}

	public void ReadOrWriteRxEncryptInt(ref ReactiveProperty<EncryptInt> val, string name = null)
	{
		val = new ReactiveProperty<EncryptInt>(ReadSingle(StaticUtils.StringToInt, defaultVal: 0));
	}

	public void ReadOrWriteRxEncryptLong(ref ReactiveProperty<EncryptLong> val, string name = null)
	{
		val = new ReactiveProperty<EncryptLong>(ReadSingle(StaticUtils.StringToLong, defaultVal: 0));
	}

	#endregion

	#region read list

	public void ReadOrWriteListBool(ref List<bool> val, string name = null)
	{
		ReadList(ref val, StaticUtils.StringToBool, defaultVal: false);
	}

	public void ReadOrWriteListInt(ref List<int> val, string name = null)
	{
		ReadList(ref val, StaticUtils.StringToInt, defaultVal: 0);
	}

	public void ReadOrWriteListLong(ref List<long> val, string name = null)
	{
		ReadList(ref val, StaticUtils.StringToLong, defaultVal: 0);
	}

	public void ReadOrWriteListFloat(ref List<float> val, string name = null)
	{
		ReadList(ref val, StaticUtils.StringToFloat, defaultVal: 0);
	}

	public void ReadOrWriteListDouble(ref List<double> val, string name = null)
	{
		ReadList(ref val, StaticUtils.StringToDouble, defaultVal: 0);
	}

	public void ReadOrWriteListString(ref List<string> val, string name = null)
	{
		ReadList(ref val, s => s, defaultVal: "");
	}

	public void ReadOrWriteListDateTime(ref List<DateTime> val, string name = null)
	{
		ReadList(ref val, StaticUtils.StringToDateTime, defaultVal: new DateTime());
	}

	public void ReadOrWriteListEnum<T>(ref List<T> val, string name = null) where T : struct, IConvertible
	{
		var defaultEnum = StaticUtils.IntToEnum<T>(-1);
		ReadList(ref val, StaticUtils.StringToEnum<T>, defaultVal: defaultEnum);
	}

	public void ReadOrWriteListObj<T>(ref List<T> val, string name = null) where T : IFileStreamObject, new()
	{
		val = (List<T>)ReadIListObj(length =>
		{
			var l = new List<T>();
			for (var i = 0; i < length; i++)
			{
				l.Add(default);
			}
			return l;
		});
	}

	#endregion

	#region read list of rx property

	public void ReadOrWriteListRxBool(ref List<ReactiveProperty<bool>> val, string name = null)
	{
		ReadListRx(ref val, StaticUtils.StringToBool, defaultVal: false);
	}

	public void ReadOrWriteListRxInt(ref List<ReactiveProperty<int>> val, string name = null)
	{
		ReadListRx(ref val, StaticUtils.StringToInt, defaultVal: 0);
	}

	public void ReadOrWriteListRxLong(ref List<ReactiveProperty<long>> val, string name = null)
	{
		ReadListRx(ref val, StaticUtils.StringToLong, defaultVal: 0);
	}

	public void ReadOrWriteListRxFloat(ref List<ReactiveProperty<float>> val, string name = null)
	{
		ReadListRx(ref val, StaticUtils.StringToFloat, defaultVal: 0);
	}

	public void ReadOrWriteListRxDouble(ref List<ReactiveProperty<double>> val, string name = null)
	{
		ReadListRx(ref val, StaticUtils.StringToDouble, defaultVal: 0);
	}

	public void ReadOrWriteListRxString(ref List<ReactiveProperty<string>> val, string name = null)
	{
		ReadListRx(ref val, s => s, defaultVal: "");
	}

	public void ReadOrWriteListRxDateTime(ref List<ReactiveProperty<DateTime>> val, string name = null)
	{
		ReadListRx(ref val, StaticUtils.StringToDateTime, defaultVal: new DateTime());
	}

	public void ReadOrWriteListRxEnum<T>(ref List<ReactiveProperty<T>> val, string name = null) where T : struct, IConvertible
	{
		var defaultEnum = StaticUtils.IntToEnum<T>(-1);
		ReadListRx(ref val, StaticUtils.StringToEnum<T>, defaultVal: defaultEnum);
	}

	#endregion

	#region read rx list

	public void ReadOrWriteRxListBool(ref ObservableList<bool> val, string name = null)
	{
		ReadRxList(ref val, StaticUtils.StringToBool, defaultVal: false);
	}

	public void ReadOrWriteRxListInt(ref ObservableList<int> val, string name = null)
	{
		ReadRxList(ref val, StaticUtils.StringToInt, defaultVal: 0);
	}

	public void ReadOrWriteRxListLong(ref ObservableList<long> val, string name = null)
	{
		ReadRxList(ref val, StaticUtils.StringToLong, defaultVal: 0);
	}

	public void ReadOrWriteRxListFloat(ref ObservableList<float> val, string name = null)
	{
		ReadRxList(ref val, StaticUtils.StringToFloat, defaultVal: 0);
	}

	public void ReadOrWriteRxListDouble(ref ObservableList<double> val, string name = null)
	{
		ReadRxList(ref val, StaticUtils.StringToDouble, defaultVal: 0);
	}

	public void ReadOrWriteRxListString(ref ObservableList<string> val, string name = null)
	{
		ReadRxList(ref val, s => s, defaultVal: "");
	}

	public void ReadOrWriteRxListDateTime(ref ObservableList<DateTime> val, string name = null)
	{
		ReadRxList(ref val, StaticUtils.StringToDateTime, defaultVal: new DateTime());
	}

	public void ReadOrWriteRxListEnum<T>(ref ObservableList<T> val, string name = null) where T : struct, IConvertible
	{
		var defaultEnum = StaticUtils.IntToEnum<T>(-1);
		ReadRxList(ref val, StaticUtils.StringToEnum<T>, defaultVal: defaultEnum);
	}

	public void ReadOrWriteRxListObj<T>(ref ObservableList<T> val, string name = null) where T : IFileStreamObject, new()
	{
		val = (ObservableList<T>)ReadIListObj(length =>
		{
			var l = new ObservableList<T>();
			for (var i = 0; i < length; i++)
			{
				l.Add(default);
			}
			return l;
		});
	}

	#endregion

	#region read dictionary

	public void ReadOrWriteDicIntObj<T>(ref Dictionary<int, T> val, string name = null) where T : IFileStreamObject, new()
	{
		val = ReadDicObj<int, T>(StaticUtils.StringToInt, defaultVal: 0);
	}

	public void ReadOrWriteDicStringObj<T>(ref Dictionary<string, T> val, string name = null) where T : IFileStreamObject, new()
	{
		val = ReadDicObj<string, T>(s => s, defaultVal: "");
	}

	public void ReadOrWriteDicEnumObj<TEnum, TObj>(ref Dictionary<TEnum, TObj> val, string name = null)
		where TEnum : struct, IConvertible
		where TObj : IFileStreamObject, new()
	{
		var defaultEnum = StaticUtils.IntToEnum<TEnum>(-1);
		val = ReadDicObj<TEnum, TObj>(StaticUtils.StringToEnum<TEnum>, defaultEnum);
	}

	#endregion

	#region utils

	private void BuildCsvData(StreamReader streamReader)
    {
		var csvReader = new CsvReader(streamReader);

		ListHeaders = csvReader.ReadRecord();

		var prevId = "";
		List<List<string>> currentIdData = null;
		List<string> fields;
		while ((fields = csvReader.ReadRecord()) != null)
		{
			var id = fields[0];

			if (id != "" && id != prevId)
			{
				prevId = id;
				currentIdData = new List<List<string>>();
				for (var i = 0; i < fields.Count; ++i)
				{
					currentIdData.Add(new List<string>());
				}
				csvData.Add(currentIdData);
			}

			for (var fieldIndex = 0; fieldIndex < fields.Count; ++fieldIndex)
			{
				currentIdData[fieldIndex].Add(fields[fieldIndex]);
			}
		}

		NumItems = csvData.Count;
	}

	private void Next()
	{
		currentField++;
		if (currentField >= ListHeaders.Count)
		{
			currentId++;
			currentField = 0;
		}
	}

	private T ReadSingle<T>(Func<string, T> parseFunc, T defaultVal)
	{
		T val;
		var strVal = csvData[currentId][currentField][currentItem];
		if (string.IsNullOrEmpty(strVal))
		{
			val = defaultVal;
		}
		else
		{
			val = parseFunc.Invoke(strVal);
		}
		Next();
		return val;
	}

	private void ReadList<T>(ref List<T> val, Func<string, T> parseFunc, T defaultVal)
	{
		val = (List<T>)ReadIList(length =>
		{
			var l = new List<T>();
			for (var i = 0; i < length; i++)
			{
				l.Add(default);
			}
			return l;
		}, parseFunc, defaultVal);
	}

	private void ReadListRx<T>(ref List<ReactiveProperty<T>> val, Func<string, T> parseFunc, T defaultVal)
	{
		val = (List<ReactiveProperty<T>>)ReadIList(length =>
		{
			var l = new List<ReactiveProperty<T>>();
			for (var i = 0; i < length; i++)
			{
				l.Add(null);
			}
			return l;
		}, str => new ReactiveProperty<T>(parseFunc(str)), new ReactiveProperty<T>(defaultVal));
	}

	private void ReadRxList<T>(ref ObservableList<T> val, Func<string, T> parseFunc, T defaultVal)
	{
		val = (ObservableList<T>)ReadIList(length =>
		{
			var l = new ObservableList<T>();
			for (var i = 0; i < length; i++)
			{
				l.Add(default);
			}
			return l;
		}, parseFunc, defaultVal);
	}

	private IList<T> ReadIList<T>(Func<int, IList<T>> createListFunc, Func<string, T> parseFunc, T defaultVal)
	{
		var strArrVal = csvData[currentId][currentField];
		var val = createListFunc(strArrVal.Count);
		for (var i = 0; i < strArrVal.Count; i++)
		{
			if (string.IsNullOrEmpty(strArrVal[i]))
			{
				val[i] = defaultVal;
			}
			else
			{
				val[i] = parseFunc.Invoke(strArrVal[i]);
			}
		}
		Next();
		return val;
	}

	private IList<T> ReadIListObj<T>(Func<int, IList<T>> createListFunc) where T : IFileStreamObject, new()
	{
		var cacheCurrentId = currentId;
		var cacheCurrentField = currentField;

		var nItems = csvData[currentId][currentField].Count;
		var val = createListFunc(nItems);
		for (var i = 0; i < nItems; i++)
		{
			var obj = new T();
			obj.ReadOrWrite(this, -1);
			val[i] = obj;

			if (i < nItems - 1)
			{
				currentId = cacheCurrentId;
				currentField = cacheCurrentField;
			}
			currentItem++;
		}

		currentItem = 0;
		return val;
	}

	private Dictionary<TKey, TObj> ReadDicObj<TKey, TObj>(Func<string, TKey> parseFunc, TKey defaultVal) where TObj : IFileStreamObject, new()
	{
		List<TKey> keys = null;
		ReadList(ref keys, parseFunc, defaultVal);

		List<TObj> objs = null;
		ReadOrWriteListObj(ref objs);

		var dic = new Dictionary<TKey, TObj>();
		for (var i = 0; i < keys.Count; i++)
		{
			dic.Add(keys[i], objs[i]);
		}
		return dic;
	}

	#endregion
}

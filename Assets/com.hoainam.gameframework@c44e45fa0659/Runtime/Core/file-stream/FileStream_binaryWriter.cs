using System.IO;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using R3;
using System.Linq;
using ObservableCollections;

public class FileStream_binaryWriter : IFileStream
{
    private BinaryWriter writer;

    public FileStream_binaryWriter(BinaryWriter writer)
    {
        this.writer = writer;
    }

    #region write primitive

    public void ReadOrWriteBool(ref bool val, string name = null)
    {
        writer.Write(val);
    }

    public void ReadOrWriteInt(ref int val, string name = null)
    {
        writer.Write(val);
    }

    public void ReadOrWriteLong(ref long val, string name = null)
    {
        writer.Write(val);
    }

    public void ReadOrWriteFloat(ref float val, string name = null)
    {
        writer.Write(val);
    }

    public void ReadOrWriteDouble(ref double val, string name = null)
    {
        writer.Write(val);
    }

    public void ReadOrWriteString(ref string val, string name = null)
    {
        WriteString(val);
    }

    public void ReadOrWriteDateTime(ref DateTime val, string name = null)
    {
        WriteDateTime(val);
    }

    public void ReadOrWriteEnum<T>(ref T val, string name = null) where T : struct, IConvertible
    {
        WriteEnum(val);
    }

	public void ReadOrWriteObj<T>(ref T val, string name = null) where T : IFileStreamObject, new()
	{
		writer.Write(val.ModelVersion);
        val.ReadOrWrite(this, val.ModelVersion);
	}

	#endregion

	#region write rx property

	public void ReadOrWriteRxBool(ref ReactiveProperty<bool> val, string name = null)
	{
		writer.Write(val.Value);
	}

	public void ReadOrWriteRxInt(ref ReactiveProperty<int> val, string name = null)
	{
		writer.Write(val.Value);
	}

	public void ReadOrWriteRxLong(ref ReactiveProperty<long> val, string name = null)
	{
		writer.Write(val.Value);
	}

	public void ReadOrWriteRxFloat(ref ReactiveProperty<float> val, string name = null)
	{
		writer.Write(val.Value);
	}

	public void ReadOrWriteRxDouble(ref ReactiveProperty<double> val, string name = null)
	{
		writer.Write(val.Value);
	}

	public void ReadOrWriteRxString(ref ReactiveProperty<string> val, string name = null)
	{
		WriteString(val.Value);
	}

	public void ReadOrWriteRxDateTime(ref ReactiveProperty<DateTime> val, string name = null)
	{
		WriteDateTime(val.Value);
	}

	public void ReadOrWriteRxEnum<T>(ref ReactiveProperty<T> val, string name = null) where T : struct, IConvertible
	{
		WriteEnum(val.Value);
	}

	#endregion

	#region write encrypt

	public void ReadOrWriteEncryptInt(ref EncryptInt val, string name = null)
	{
		writer.Write(val);
	}

	public void ReadOrWriteEncryptLong(ref EncryptLong val, string name = null)
	{
		writer.Write(val);
	}

	public void ReadOrWriteRxEncryptInt(ref ReactiveProperty<EncryptInt> val, string name = null)
	{
		writer.Write(val.Value);
	}

	public void ReadOrWriteRxEncryptLong(ref ReactiveProperty<EncryptLong> val, string name = null)
	{
		writer.Write(val.Value);
	}

	#endregion

	#region write list

	public void ReadOrWriteListBool(ref List<bool> val, string name = null)
	{
		WriteIList(val, writer.Write);
	}

	public void ReadOrWriteListInt(ref List<int> val, string name = null)
	{
		WriteIList(val, writer.Write);
	}

	public void ReadOrWriteListLong(ref List<long> val, string name = null)
	{
		WriteIList(val, writer.Write);
	}

	public void ReadOrWriteListFloat(ref List<float> val, string name = null)
	{
		WriteIList(val, writer.Write);
	}

	public void ReadOrWriteListDouble(ref List<double> val, string name = null)
	{
		WriteIList(val, writer.Write);
	}

	public void ReadOrWriteListString(ref List<string> val, string name = null)
	{
		WriteIList(val, WriteString);
	}

	public void ReadOrWriteListDateTime(ref List<DateTime> val, string name = null)
	{
		WriteIList(val, WriteDateTime);
	}

	public void ReadOrWriteListEnum<T>(ref List<T> val, string name = null) where T : struct, IConvertible
	{
		WriteIList(val, WriteEnum);
	}

	public void ReadOrWriteListObj<T>(ref List<T> val, string name = null) where T : IFileStreamObject, new()
	{
		WriteIListObj(val);
	}

	#endregion

	#region write list of rx property

	public void ReadOrWriteListRxBool(ref List<ReactiveProperty<bool>> val, string name = null)
	{
		WriteIListRx(val, writer.Write);
	}

	public void ReadOrWriteListRxInt(ref List<ReactiveProperty<int>> val, string name = null)
	{
		WriteIListRx(val, writer.Write);
	}

	public void ReadOrWriteListRxLong(ref List<ReactiveProperty<long>> val, string name = null)
	{
		WriteIListRx(val, writer.Write);
	}

	public void ReadOrWriteListRxFloat(ref List<ReactiveProperty<float>> val, string name = null)
	{
		WriteIListRx(val, writer.Write);
	}

	public void ReadOrWriteListRxDouble(ref List<ReactiveProperty<double>> val, string name = null)
	{
		WriteIListRx(val, writer.Write);
	}

	public void ReadOrWriteListRxString(ref List<ReactiveProperty<string>> val, string name = null)
	{
		WriteIListRx(val, WriteString);
	}

	public void ReadOrWriteListRxDateTime(ref List<ReactiveProperty<DateTime>> val, string name = null)
	{
		WriteIListRx(val, WriteDateTime);
	}

	public void ReadOrWriteListRxEnum<T>(ref List<ReactiveProperty<T>> val, string name = null) where T : struct, IConvertible
	{
		WriteIListRx(val, WriteEnum);
	}

	#endregion

	#region write rx list

	public void ReadOrWriteRxListBool(ref ObservableList<bool> val, string name = null)
	{
		WriteIList(val, writer.Write);
	}

	public void ReadOrWriteRxListInt(ref ObservableList<int> val, string name = null)
	{
		WriteIList(val, writer.Write);
	}

	public void ReadOrWriteRxListLong(ref ObservableList<long> val, string name = null)
	{
		WriteIList(val, writer.Write);
	}

	public void ReadOrWriteRxListFloat(ref ObservableList<float> val, string name = null)
	{
		WriteIList(val, writer.Write);
	}

	public void ReadOrWriteRxListDouble(ref ObservableList<double> val, string name = null)
	{
		WriteIList(val, writer.Write);
	}

	public void ReadOrWriteRxListString(ref ObservableList<string> val, string name = null)
	{
		WriteIList(val, WriteString);
	}

	public void ReadOrWriteRxListDateTime(ref ObservableList<DateTime> val, string name = null)
	{
		WriteIList(val, WriteDateTime);
	}

	public void ReadOrWriteRxListEnum<T>(ref ObservableList<T> val, string name = null) where T : struct, IConvertible
	{
		WriteIList(val, WriteEnum);
	}

	public void ReadOrWriteRxListObj<T>(ref ObservableList<T> val, string name = null) where T : IFileStreamObject, new()
	{
		WriteIListObj(val);
	}

	#endregion

	#region write dictionary

	public void ReadOrWriteDicIntObj<T>(ref Dictionary<int, T> val, string name = null) where T : IFileStreamObject, new()
	{
		WriteDicObj(val, writer.Write);
	}

	public void ReadOrWriteDicStringObj<T>(ref Dictionary<string, T> val, string name = null) where T : IFileStreamObject, new()
	{
		WriteDicObj(val, WriteString);
	}

	public void ReadOrWriteDicEnumObj<TEnum, TObj>(ref Dictionary<TEnum, TObj> val, string name = null)
		where TEnum : struct, IConvertible
		where TObj : IFileStreamObject, new()
	{
		WriteDicObj(val, WriteEnum);
	}

	#endregion

	#region utils

	private void WriteString(string s)
    {
		writer.Write(StaticUtils.XorString(s, FileStreamConst.xorEncryptionKey));
	}

    private void WriteDateTime(DateTime dt)
    {
		writer.Write(StaticUtils.DateTimeToUnixEpoch(dt));
	}

    private void WriteEnum<T>(T e) where T : struct, IConvertible
    {
        writer.Write(Convert.ToInt32(e));
    }

	private void WriteIList<T>(IList<T> l, UnityAction<T> writeAction)
    {
        writer.Write(l.Count);
        foreach (var i in l)
        {
            writeAction.Invoke(i);
        }
    }

	private void WriteIListRx<T>(IList<ReactiveProperty<T>> l, UnityAction<T> writeAction)
	{
		WriteIList(l, x =>
		{
			writeAction(x.Value);
		});
	}

    private void WriteIListObj<T>(IList<T> l) where T : IFileStreamObject, new()
    {
        var version = l.Count > 0 ? l[0].ModelVersion : -1;
        writer.Write(version);
		WriteIList(l, obj =>
        {
            obj.ReadOrWrite(this, version);
        });
    }

	private void WriteDicObj<TKey, TObj>(Dictionary<TKey, TObj> dic, UnityAction<TKey> writeAction) where TObj : IFileStreamObject, new()
	{
		var version = dic.Count > 0 ? dic.First().Value.ModelVersion : -1;
		writer.Write(version);
		writer.Write(dic.Count);
		foreach (var i in dic)
		{
			writeAction(i.Key);
			i.Value.ReadOrWrite(this, version);
		}
	}

	#endregion
}

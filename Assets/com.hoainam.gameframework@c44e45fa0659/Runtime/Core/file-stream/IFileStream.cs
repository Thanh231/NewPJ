using System;
using System.Collections.Generic;
using ObservableCollections;
using R3;

public interface IFileStream
{
	#region single

	void ReadOrWriteBool(ref bool val, string name = null);
	void ReadOrWriteInt(ref int val, string name = null);
	void ReadOrWriteLong(ref long val, string name = null);
	void ReadOrWriteFloat(ref float val, string name = null);
	void ReadOrWriteDouble(ref double val, string name = null);
	void ReadOrWriteString(ref string val, string name = null);
	void ReadOrWriteDateTime(ref DateTime val, string name = null);
	void ReadOrWriteEnum<T>(ref T val, string name = null) where T : struct, IConvertible;
	void ReadOrWriteObj<T>(ref T val, string name = null) where T : IFileStreamObject, new();

	void ReadOrWriteRxBool(ref ReactiveProperty<bool> val, string name = null);
	void ReadOrWriteRxInt(ref ReactiveProperty<int> val, string name = null);
	void ReadOrWriteRxLong(ref ReactiveProperty<long> val, string name = null);
	void ReadOrWriteRxFloat(ref ReactiveProperty<float> val, string name = null);
	void ReadOrWriteRxDouble(ref ReactiveProperty<double> val, string name = null);
	void ReadOrWriteRxString(ref ReactiveProperty<string> val, string name = null);
	void ReadOrWriteRxDateTime(ref ReactiveProperty<DateTime> val, string name = null);
	void ReadOrWriteRxEnum<T>(ref ReactiveProperty<T> val, string name = null) where T : struct, IConvertible;

	#endregion

	#region encrypt single

	void ReadOrWriteEncryptInt(ref EncryptInt val, string name = null);
	void ReadOrWriteEncryptLong(ref EncryptLong val, string name = null);

	void ReadOrWriteRxEncryptInt(ref ReactiveProperty<EncryptInt> val, string name = null);
	void ReadOrWriteRxEncryptLong(ref ReactiveProperty<EncryptLong> val, string name = null);

	#endregion

	#region list

	void ReadOrWriteListBool(ref List<bool> val, string name = null);
	void ReadOrWriteListInt(ref List<int> val, string name = null);
	void ReadOrWriteListLong(ref List<long> val, string name = null);
	void ReadOrWriteListFloat(ref List<float> val, string name = null);
	void ReadOrWriteListDouble(ref List<double> val, string name = null);
	void ReadOrWriteListString(ref List<string> val, string name = null);
	void ReadOrWriteListDateTime(ref List<DateTime> val, string name = null);
	void ReadOrWriteListEnum<T>(ref List<T> val, string name = null) where T : struct, IConvertible;
	void ReadOrWriteListObj<T>(ref List<T> val, string name = null) where T : IFileStreamObject, new();

	void ReadOrWriteListRxBool(ref List<ReactiveProperty<bool>> val, string name = null);
	void ReadOrWriteListRxInt(ref List<ReactiveProperty<int>> val, string name = null);
	void ReadOrWriteListRxLong(ref List<ReactiveProperty<long>> val, string name = null);
	void ReadOrWriteListRxFloat(ref List<ReactiveProperty<float>> val, string name = null);
	void ReadOrWriteListRxDouble(ref List<ReactiveProperty<double>> val, string name = null);
	void ReadOrWriteListRxString(ref List<ReactiveProperty<string>> val, string name = null);
	void ReadOrWriteListRxDateTime(ref List<ReactiveProperty<DateTime>> val, string name = null);
	void ReadOrWriteListRxEnum<T>(ref List<ReactiveProperty<T>> val, string name = null) where T : struct, IConvertible;

	void ReadOrWriteRxListInt(ref ObservableList<int> val, string name = null);
	void ReadOrWriteRxListLong(ref ObservableList<long> val, string name = null);
	void ReadOrWriteRxListFloat(ref ObservableList<float> val, string name = null);
	void ReadOrWriteRxListDouble(ref ObservableList<double> val, string name = null);
	void ReadOrWriteRxListString(ref ObservableList<string> val, string name = null);
	void ReadOrWriteRxListDateTime(ref ObservableList<DateTime> val, string name = null);
	void ReadOrWriteRxListEnum<T>(ref ObservableList<T> val, string name = null) where T : struct, IConvertible;
	void ReadOrWriteRxListObj<T>(ref ObservableList<T> val, string name = null) where T : IFileStreamObject, new();

	#endregion

	#region dictionary

	void ReadOrWriteDicIntObj<T>(ref Dictionary<int, T> val, string name = null) where T : IFileStreamObject, new();
	void ReadOrWriteDicStringObj<T>(ref Dictionary<string, T> val, string name = null) where T : IFileStreamObject, new();
	void ReadOrWriteDicEnumObj<TEnum, TObj>(ref Dictionary<TEnum, TObj> val, string name = null) 
		where TEnum : struct, IConvertible 
		where TObj : IFileStreamObject, new();

	#endregion
}


using System.Collections.Generic;

public class BaseConfig<T> : IBaseConfig where T : BaseConfigItem, new()
{
	public List<T> listConfigItems = new List<T>();

	public void Read(IFileStream stream, int numItems)
	{
		listConfigItems.Clear();
		for (var i = 0; i < numItems; i++)
		{
			var item = new T();
			item.ReadOrWrite(stream);
			listConfigItems.Add(item);
		}
	}

	public void Write(IFileStream stream)
	{
		var numItems = listConfigItems.Count;
		stream.ReadOrWriteInt(ref numItems);
		foreach (var item in listConfigItems)
		{
			item.ReadOrWrite(stream);
		}
	}
}
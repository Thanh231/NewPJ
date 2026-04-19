
using System.IO;
using UnityEngine;

public class PlayerModelFile_binary : IPlayerModelFile
{
	public string Extension => "bin";

	private const int magicLengthMin = 20;
	private const int magicLengthMax = 60;

	#region read

	public void ReadModel(string filepath, BasePlayerModel model)
	{
		StaticUtils.OpenFileForRead(filepath, stream =>
		{
			ReadModel(stream, model);
		});
	}

	public void ReadModel(Stream inputStream, BasePlayerModel model)
	{
		//read file
		byte[] headMagic = null;
		byte[] tailMagic = null;
		byte[] modelBytes = null;
		
		var binaryReader = new BinaryReader(inputStream);
		
		var length = binaryReader.ReadInt32();
		headMagic = binaryReader.ReadBytes(length);

		length = binaryReader.ReadInt32();
		modelBytes = binaryReader.ReadBytes(length);

		length = binaryReader.ReadInt32();
		tailMagic = binaryReader.ReadBytes(length);

		//decrypt model
		modelBytes = StaticUtils.XorByteArray(modelBytes, tailMagic);
		modelBytes = StaticUtils.XorByteArray(modelBytes, headMagic);

		//read model
		using (var stream = new MemoryStream(modelBytes))
		{
			var reader = new FileStream_binaryReader(new BinaryReader(stream));
			var version = 0;
			reader.ReadOrWriteInt(ref version);
			model.ReadOrWrite(reader, version);
		}
	}

	#endregion

	#region write

	public void WriteModel(string filepath, BasePlayerModel model)
	{
		StaticUtils.OpenFileForWrite(filepath, (BinaryWriter binaryWriter) =>
		{
			WriteModel(binaryWriter, model);
		});
	}

	public void WriteModel(BinaryWriter binaryWriter, BasePlayerModel model)
	{
		//create magic bytes
		var headMagic = StaticUtils.RandomAByteArray(Random.Range(magicLengthMin, magicLengthMax));
		var tailMagic = StaticUtils.RandomAByteArray(Random.Range(magicLengthMin, magicLengthMax));

		//create model bytes
		byte[] modelBytes;
		using (var stream = new MemoryStream())
		{
			var writer = new FileStream_binaryWriter(new BinaryWriter(stream));
			var version = model.ModelVersion;
			writer.ReadOrWriteInt(ref version);
			model.ReadOrWrite(writer, version);

			modelBytes = stream.ToArray();
		}

		//encrypt model bytes
		modelBytes = StaticUtils.XorByteArray(modelBytes, headMagic);
		modelBytes = StaticUtils.XorByteArray(modelBytes, tailMagic);

		//write to file
		binaryWriter.Write(headMagic.Length);
		binaryWriter.Write(headMagic);

		binaryWriter.Write(modelBytes.Length);
		binaryWriter.Write(modelBytes);

		binaryWriter.Write(tailMagic.Length);
		binaryWriter.Write(tailMagic);
	}

	#endregion
}
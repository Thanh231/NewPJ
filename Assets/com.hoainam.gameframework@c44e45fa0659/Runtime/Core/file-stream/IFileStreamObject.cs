
public interface IFileStreamObject
{
	public abstract int ModelVersion { get; }
	void ReadOrWrite(IFileStream stream, int version);
}
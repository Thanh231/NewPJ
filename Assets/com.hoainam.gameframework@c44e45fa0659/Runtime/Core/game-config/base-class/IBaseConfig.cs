public interface IBaseConfig
{
	void Read(IFileStream stream, int numItems);
	void Write(IFileStream stream);
}
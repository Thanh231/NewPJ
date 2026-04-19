
public interface IPlayerModelFile
{
	string Extension { get; }
	void ReadModel(string filepath, BasePlayerModel model);
	void WriteModel(string filepath, BasePlayerModel model);
}
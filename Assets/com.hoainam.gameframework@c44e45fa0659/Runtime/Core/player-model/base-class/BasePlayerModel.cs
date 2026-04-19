
public abstract class BasePlayerModel
{
	public abstract int ModelVersion { get; }
	public abstract void ReadOrWrite(IFileStream stream, int version);

	public virtual void OnModelInitializing() { }
	public virtual void OnModelLoaded() { }

	public void Save()
	{
		PlayerModelManager.SaveModel(this);
	}
}
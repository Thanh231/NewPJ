
using UnityEditor;

public class BuildSamsungStoreCacheConfiguration
{
	private string packageName;
	private bool splitAssets;

    public BuildSamsungStoreCacheConfiguration()
	{
		packageName = PlayerSettings.applicationIdentifier;
		splitAssets = PlayerSettings.Android.splitApplicationBinary;
	}

	public void Set()
	{
		PlayerSettings.applicationIdentifier = $"{packageName}sgs";
		PlayerSettings.Android.splitApplicationBinary = false;
	}

	public void Restore()
	{
		PlayerSettings.applicationIdentifier = packageName;
		PlayerSettings.Android.splitApplicationBinary = splitAssets;
	}
}
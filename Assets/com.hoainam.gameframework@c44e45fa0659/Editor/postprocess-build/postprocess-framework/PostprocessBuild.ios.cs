
using UnityEditor.Build;

public partial class PostprocessBuild : IPostprocessBuildWithReport
{
	private void OnPostprocessBuild_ios(string path)
	{
		var file = new IosProjectFile(path);
		file.AddFramework(IosProjectFile.TargetType.UnityMain, "AdServices.framework");
		file.Save();
	}
}
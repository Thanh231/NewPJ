using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public static class IosPostProcessLocalization
{
	[PostProcessBuild]
	public static void OnPostProcessBuild(BuildTarget target, string path)
	{
		if (target != BuildTarget.iOS)
		{
			return;
		}
		
		var project = new IosProjectFile(path);
		var languageInfo = new LanguageInfoConfig();
		foreach (var i in languageInfo.languageInfoItems)
		{
			var languageCode = i.Value.iosIsoCode;
			var filePath = $"{Application.dataPath}/_game/localized-system-data/IosLocalization/{languageCode}.lproj/InfoPlist.strings";
			project.AddLocalization(languageCode, filePath);
		}
		project.Save();
	}
}
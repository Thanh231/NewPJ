using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AndroidManifestExtractorMainState : EditorWindowState
{
	private EditorFilePicker pickFile = new EditorFilePicker(new List<string>() { "apk" }, "pick an apk:");

	public override void OnDraw()
	{
		pickFile.Draw();
		if (GUILayout.Button("extract AndroidManifest.xml"))
		{
			var toolPath = $"{StaticUtils.GetFrameworkPath()}/Editor/Core/Plugins/apktool_2.9.3.jar";
			var outPath = StaticUtilsEditor.RandomATempPath();
			StaticUtilsEditor.RunBatchScript(StaticUtilsEditor.JavaFile, new List<string>()
			{
				"-jar", toolPath, "d", "-s", "-o", outPath, pickFile.PickedPath,
			});

			Process.Start($"{outPath}/AndroidManifest.xml");

			StaticUtilsEditor.DisplayDialog("extract success");
		}
	}
}
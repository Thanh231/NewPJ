
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Events;

public class ProcessManifestForBilling : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
	public int callbackOrder => 100;
	
	private static void ProcessXml(UnityAction<XmlFile> callback)
	{
		var path = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
		if (!StaticUtils.CheckFileExist(path, true))
		{
			return;
		}
		var xml = new XmlFile(path);
		callback?.Invoke(xml);
		xml.Save();
	}

	#region pre-process

	public void OnPreprocessBuild(BuildReport report)
	{
		if (report.summary.platform != BuildTarget.Android)
		{
			return;
		}

		var permission = report.summary.outputPath.Contains("samsung") ?
			"com.android.vending.BILLING" : "com.samsung.android.iap.permission.BILLING";

		AddRemoveTag(permission);
	}
	
	public static void AddRemoveTag(string permission)
	{
		ProcessXml(xml =>
		{
			xml.AddElement(xml.Root, "uses-permission", tag =>
			{
				xml.SetAttribute(tag, "name", permission, "android");
				xml.SetAttribute(tag, "node", "remove", "tools");
			});
		});
	}

	#endregion

	#region post-process

	public void OnPostprocessBuild(BuildReport report)
	{
		if (report.summary.platform != BuildTarget.Android)
		{
			return;
		}

		DeleteRemoveTag();
	}
	
	public static void DeleteRemoveTag()
	{
		ProcessXml(xml =>
		{
			var tag = xml.GetChildElementWithAttribute(xml.Root, "uses-permission", "tools:node", "remove");
			xml.Root.RemoveChild(tag);
		});
	}

	#endregion
}
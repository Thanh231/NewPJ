using System.IO;
using System.Xml;
using UnityEditor.Build;
using UnityEngine;

public partial class PreprocessBuild : IPreprocessBuildWithReport
{
	private void OnPreprocessBuild_android()
	{
		ProcessManifest();
	}

	#region process manifest

	public static void ProcessManifest()
	{
		var path = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
		if (!StaticUtils.CheckFileExist(path))
		{
			return;
		}
		var xml = new XmlFile(path);

		var applicationTag = xml.GetChildElement(xml.Root, "application");

		//add tools namespace
		xml.AddNamespace("tools", "http://schemas.android.com/tools");

		//disable debuggable
		xml.SetAttribute(applicationTag, "debuggable", "false", "android");

		//add allow_multiple_resumed_activities
		var activityTag = xml.GetChildElementWithAttribute(applicationTag, "activity", "android:name",
			"com.unity3d.player.UnityPlayerActivity");
		var allowMultipleActivityTag = GetAllowMultipleActivityTag(activityTag, xml);
		if (allowMultipleActivityTag != null)
		{
			xml.SetAttribute(allowMultipleActivityTag, "value", "true", "android");
		}
		else
		{
			xml.AddElement(activityTag, "meta-data", element =>
			{
				xml.SetAttribute(element, "name", "android.allow_multiple_resumed_activities", "android");
				xml.SetAttribute(element, "value", "true", "android");
			});
		}
		
		//enable usesCleartextTraffic
		xml.SetAttribute(applicationTag, "usesCleartextTraffic", "true", "android");

		xml.Save();
	}

	private static XmlElement GetAllowMultipleActivityTag(XmlElement activityTag, XmlFile xmlFile)
	{
		var l = xmlFile.GetListChildrenElement(activityTag, "meta-data");
		foreach (var i in l)
		{
			if (HasAttributeMultipleActivity(i))
			{
				return i;
			}
		}
		return null;
	}

	private static bool HasAttributeMultipleActivity(XmlElement element)
	{
		foreach (XmlAttribute i in element.Attributes)
		{
			if (i.Name.Equals("android:name") && i.Value.Equals("android.allow_multiple_resumed_activities"))
			{
				return true;
			}
		}
		return false;
	}

	#endregion
}
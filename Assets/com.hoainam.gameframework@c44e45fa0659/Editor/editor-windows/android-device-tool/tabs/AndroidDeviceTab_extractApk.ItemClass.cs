
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class AndroidDeviceTab_extractApk : EditorTabInWindow.TabItemWindow
{
	public class ListItem : EditorListView.ListViewItem
	{
		private Texture2D texture = null;
		private string appName;
		private string packageName;

		const float imageSize = 64;
		const string emptyAppIconPath = "Assets/_framework/android-device-tool[OLD]/Editor/no-image-icon.png";

		public ListItem(string packageName, string appName)
		{
			var folder = $"{StaticUtils.GetProjectPath()}/Temp/{packageName}";
			var pngs = StaticUtils.GetFilesInFolder(folder, false, null, "png", true);

			if (pngs.Count == 0)
			{
				texture = AssetDatabase.LoadAssetAtPath<Texture2D>(emptyAppIconPath);
			}
			else if (pngs.Count == 1)
			{
				byte[] fileData = File.ReadAllBytes($"{folder}/{pngs[0]}.png");
				texture = new Texture2D(2, 2);
				texture.LoadImage(fileData);
			}
			else if (pngs.Count > 1)
			{
				throw new Exception($"{packageName} has more than 1 icon");
			}

			this.appName = appName;
			this.packageName = packageName;
		}

		public override void Draw()
		{
			if (texture)
			{
				GUILayout.Label(texture, GUILayout.Width(imageSize), GUILayout.Height(imageSize));
			}
			EditorGUILayout.LabelField($"{appName} ({packageName})");
			if (GUILayout.Button("export apk"))
			{
				var folder = StaticUtilsEditor.DisplayChooseFolderDialog("choose a folder to export");
				var l = GetApkPath(packageName);
				foreach (var i in l)
				{
					StaticUtilsEditor.RunBatchScript(StaticUtilsEditor.ADBFile, 
						new List<string>() { "pull", i, folder });
				}

				StaticUtilsEditor.DisplayDialog("export success");
			}
		}
	}
}
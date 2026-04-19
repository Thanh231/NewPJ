
using System.Collections.Generic;
using UnityEditor;

public class AndroidDeviceToolState_tab: EditorWindowState_tab
{
	private string deviceName;

	public AndroidDeviceToolState_tab(string deviceName)
		: base(new List<EditorBaseTab.TabItem>() {
			new AndroidDeviceTab_extractApk(),
			new AndroidDeviceTab_installApk(),
		})
	{
		this.deviceName = deviceName;
	}

	public override void OnDraw()
	{
		EditorGUILayout.LabelField($"running device: {deviceName}");

		base.OnDraw();
	}
}
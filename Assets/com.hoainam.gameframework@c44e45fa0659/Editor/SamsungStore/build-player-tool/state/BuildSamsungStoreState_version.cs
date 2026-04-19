
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildSamsungStoreState_version : EditorWindowState
{
	private VersionComponent howToIncrease = VersionComponent.None;

	public override void OnDraw()
	{
		//current version
		var ver = PlayerSettings.bundleVersion;
		EditorGUILayout.LabelField($"current version: {ver}");
		var dotCount = ver.Count(x => x == '.');
		if (dotCount != 2)
		{
			ver = "0.0.1";
		}

		//next version
		howToIncrease = EditorGUILayoutExtension.EnumDropdownList("how to increase version:",
			VersionComponent.Count, howToIncrease, out _);
		if (howToIncrease != VersionComponent.None)
		{
			ver = StaticUtils.IncreaseVersion(ver, howToIncrease);
		}
		EditorGUILayout.LabelField($"version will build: {ver}");

		//button next state
		if (GUILayout.Button("start build with this version"))
		{
			PlayerSettings.bundleVersion = ver;
			FSM.SwitchState(new BuildSamsungStoreState_build());
		}
	}
}
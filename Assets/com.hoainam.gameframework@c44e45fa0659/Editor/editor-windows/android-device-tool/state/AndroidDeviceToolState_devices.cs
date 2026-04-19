
using System.Collections.Generic;
using UnityEngine;

public class AndroidDeviceToolState_devices : EditorWindowState
{
	public override void OnDraw()
	{
		if (GUILayout.Button("list devices"))
		{
			var devices = RunDevicesCommand();
			if (devices.Count == 0)
			{
				StaticUtilsEditor.DisplayDialog("there're no android device running");
			}
			else if (devices.Count == 1)
			{
				FSM.SwitchState(new AndroidDeviceToolState_tab(devices[0]));
			}
			else
			{
				StaticUtilsEditor.DisplayDialog("there're more than 1 android device running");
			}
		}
	}

	private List<string> RunDevicesCommand()
	{
		var devices = new List<string>();
		var result = StaticUtilsEditor.RunBatchScript(StaticUtilsEditor.ADBFile,
			new List<string>() { "devices" });
		var lines = result.output.Split('\n');
		foreach (var line in lines)
		{
			var trimmedLine = line.Trim();
			if (!string.IsNullOrEmpty(trimmedLine) && !trimmedLine.Equals("List of devices attached"))
			{
				var l = trimmedLine.Split('\t');
				devices.Add(GetDeviceName(l[0]));
			}
		}
		return devices;
	}

	private string GetDeviceName(string id)
	{
		if (id.StartsWith("emulator-"))
		{
			return "EMULATOR";
		}

		var result = StaticUtilsEditor.RunBatchScript(StaticUtilsEditor.ADBFile,
			new List<string>() { "shell", "getprop", "ro.product.brand" });
		var brand = result.output.Trim();

		result = StaticUtilsEditor.RunBatchScript(StaticUtilsEditor.ADBFile,
			new List<string>() { "shell", "getprop", "ro.product.model" });
		var model = result.output.Trim();

		return $"{brand} {model}";
	}
}
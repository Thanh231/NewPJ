
using UnityEngine;

public class ExportSpineState_choosePath : EditorWindowState
{
    private readonly EditorFolderPicker inputFolder = new("input folder:");
    private readonly EditorFolderPicker outputFolder = new("output folder:");

    public override void OnDraw()
    {
        inputFolder.Draw();
        outputFolder.Draw();

        if (GUILayout.Button("export spine"))
        {
            FSM.SwitchState(new ExportSpineState_export(inputFolder.PickedPath, outputFolder.PickedPath));
        }
    }
}

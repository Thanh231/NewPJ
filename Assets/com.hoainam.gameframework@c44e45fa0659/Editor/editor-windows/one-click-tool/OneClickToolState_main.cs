using UnityEngine;

public partial class OneClickToolState_main : EditorWindowState
{
    public override void OnDraw()
    {
        if (GUILayout.Button("clear player model"))
        {
            OnClick_clearPlayerModel();
        }

        if (GUILayout.Button("update gitignore"))
        {
            OnClick_updateGitignore();
        }

        if (GUILayout.Button("update vs code hiding files"))
        {
            OnClick_updateVSCodeHidingFiles();
        }
    }

    private void OnClick_clearPlayerModel()
    {
        StaticUtils.DeleteFolder(PlayerModelManager.GetModelFolderPath());
        StaticUtilsEditor.DisplayDialog("cleared player model");
    }

    private void OnClick_updateGitignore()
    {
        var srcPath = $"{StaticUtils.GetFrameworkPath()}/Editor/editor-windows/one-click-tool/gitignore_template.txt";
        var destPath = $"{StaticUtils.GetProjectPath()}/.gitignore";

        var text = StaticUtils.ReadTextFile(srcPath, true);
        StaticUtils.WriteTextFile(destPath, text, true);

        StaticUtilsEditor.DisplayDialog("updated gitignore");
    }
}
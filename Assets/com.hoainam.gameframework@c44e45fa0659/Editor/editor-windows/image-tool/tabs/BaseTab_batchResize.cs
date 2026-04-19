using System.IO;
using UnityEditor;
using UnityEngine;

public abstract class BaseTab_batchResize : EditorTabInWindow.TabItemWindow
{
    private DefaultAsset folder;
    
    public override void OnDraw()
    {
        folder = (DefaultAsset)EditorGUILayout.ObjectField("Image Folder:", folder, typeof(DefaultAsset), false);
        if (GUILayout.Button("resize"))
        {
            OnResizeClicked();
        }
    }

    private void OnResizeClicked()
    {
        var path = AssetDatabase.GetAssetPath(folder);
        path = path.Substring("Assets/".Length);

        var imgFiles = StaticUtils.GetFilesInFolder(path, false, null, "png");
        foreach (var imgPath in imgFiles)
        {
            var imgName = Path.GetFileNameWithoutExtension(imgPath);
            ResizeImage(imgName);
        }
        
        AssetDatabase.Refresh();

        StaticUtilsEditor.DisplayDialog("resize complete");
    }

    private void ResizeImage(string imgName)
    {
        var folderPath = AssetDatabase.GetAssetPath(folder);
        var imgPath = $"{folderPath}/{imgName}.png";
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(imgPath);
        var needRevertIsReadable = ImageToolWindow.SetImageIsReadable(texture, true);
        var newSz = GetNewSize(texture);
        var newTexture = ImageToolWindow.ResizeImage(texture, newSz.x, newSz.y);
        ImageToolWindow.SaveTexture(newTexture, imgPath, false);
        if (needRevertIsReadable)
        {
            ImageToolWindow.SetImageIsReadable(texture, false);
        }
    }

    protected abstract Vector2Int GetNewSize(Texture2D texture);
}
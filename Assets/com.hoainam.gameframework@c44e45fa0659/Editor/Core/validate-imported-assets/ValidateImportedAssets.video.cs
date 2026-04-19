
using System.Collections.Generic;
using System.IO;

public partial class ValidateImportedAssets
{
    private static readonly List<string> videoExtensions = new() { ".mp4", ".mov", ".avi", ".webm", ".m4v", ".dv" };

    private static void ValidateImportedVideo(string assetPath)
    {
        var extension = Path.GetExtension(assetPath).ToLower();
        if (!videoExtensions.Contains(extension))
        {
            return;
        }

        if (!assetPath.Contains("/StreamingAssets/"))
        {
            StaticUtilsEditor.DisplayDialog("you have to put video asset into StreamingAssets folder");
        }
    }
}

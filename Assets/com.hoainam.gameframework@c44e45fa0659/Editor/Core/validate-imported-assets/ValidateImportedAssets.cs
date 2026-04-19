
using UnityEditor;

public partial class ValidateImportedAssets : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        foreach (var asset in importedAssets)
        {
            ValidateImportedVideo(asset);
        }

        foreach (var asset in movedAssets)
        {
            ValidateImportedVideo(asset);
        }
    }
}

using UnityEditor.iOS.Xcode;

public partial class IosProjectFile
{
    private bool enableLocalization = false;

    public void AddLocalization(string languageCode, string srcPath)
    {
        EnableLocalization();
        
        var destFolder = $"{buildPath}/{languageCode}.lproj";
        StaticUtils.CopyFile(srcPath, destFolder, true);
        
        var projRelativePath = $"{languageCode}.lproj/InfoPlist.strings";
        var targetId = project.GetUnityMainTargetGuid();
        var guid = project.AddFile(projRelativePath, projRelativePath, PBXSourceTree.Source);
        project.AddFileToBuild(targetId, guid);
    }

    private void EnableLocalization()
    {
        if (enableLocalization)
        {
            return;
        }

        var targetId = project.GetUnityMainTargetGuid();
        project.SetBuildProperty(targetId, "INFOPLIST_KEY_CFBundleDisplayName", "");

        enableLocalization = true;
    }
}
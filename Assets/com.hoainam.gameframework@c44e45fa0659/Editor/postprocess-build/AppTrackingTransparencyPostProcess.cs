
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class AppTrackingTransparencyPostProcess
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS)
        {
            return;
        }

        //add framework
        var project = new IosProjectFile(path);
        project.AddFramework(IosProjectFile.TargetType.UnityFramework, "AppTrackingTransparency.framework");
        project.Save();

        //plist
        var plist = new IosPlistFile(path);
        var userTrackingDescription = "This identifier will be used to deliver personalized ads to you.";
        plist.SetString("NSUserTrackingUsageDescription", userTrackingDescription);
        plist.Save();
    }
}
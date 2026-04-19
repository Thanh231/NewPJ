
using UnityEditor;
using UnityEditor.Callbacks;

public class SocialSignInPostProcess
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS)
        {
            return;
        }

        //add capabilities to xcode project
        var project = new IosProjectFile(path);

#if USE_GAME_CENTER_SIGNIN
        project.AddCapabilityGameCenter();
#endif
        
#if USE_SIGNIN_WITH_APPLE
        project.AddCapabilitySignInWithApple();
#endif
        
        project.Save();
        
        //modify plist file
        var urlScheme = GameFrameworkConfig.instance.iosUrlScheme;
        if (!string.IsNullOrEmpty(urlScheme))
        {
            var plist = new IosPlistFile(path);
            plist.AddUrlScheme(urlScheme);
            plist.Save();
        }
    }
}

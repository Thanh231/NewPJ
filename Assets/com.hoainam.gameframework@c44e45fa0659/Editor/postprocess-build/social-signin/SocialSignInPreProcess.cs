
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class SocialSignInPreProcess : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
    
    public void OnPreprocessBuild(BuildReport report)
    {
#if USE_FACEBOOK_LOGIN
        PreProcessFacebookSdk();
#endif
    }

    public static void PreProcessFacebookSdk()
    {
        var path = $"{Application.dataPath}/FacebookSDK/Plugins/Editor/Dependencies.xml";
        var xml = new XmlFile(path);

        var iosPodsTag = xml.GetChildElement(xml.Root, "iosPods");
        var lPodsTag = xml.GetListChildrenElement(iosPodsTag, "iosPod");

        foreach (var lPod in lPodsTag)
        {
            xml.SetAttribute(lPod, "addToAllTargets", "true");
        }
        
        xml.Save();
    }
}

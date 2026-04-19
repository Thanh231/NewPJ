using UnityEditor.iOS.Xcode;

#if USE_SIGNIN_WITH_APPLE && UNITY_IOS
using AppleAuth.Editor;
#endif

public partial class IosProjectFile
{
    const string mainEntitlementsFileName = "Unity-iPhone.entitlements";
    const string subEntitlementsFileName = "Entitlements.entitlements";
    
    private ProjectCapabilityManager _projectCapability_main;
    private ProjectCapabilityManager projectCapability_main
    {
        get
        {
            if (_projectCapability_main == null)
            {
                var targetId = project.GetUnityMainTargetGuid();
                _projectCapability_main =
                    new ProjectCapabilityManager(projectPath, mainEntitlementsFileName, null, targetId);
            }

            return _projectCapability_main;
        }
    }
    
    private ProjectCapabilityManager _projectCapability_sub;
    private ProjectCapabilityManager projectCapability_sub
    {
        get
        {
            if (_projectCapability_sub == null)
            {
                var targetId = project.GetUnityMainTargetGuid();
                _projectCapability_sub =
                    new ProjectCapabilityManager(projectPath, subEntitlementsFileName, null, targetId);
            }

            return _projectCapability_sub;
        }
    }

    public void AddCapabilityGameCenter()
    {
        projectCapability_main.AddGameCenter();

        var unityMainTargetId = project.GetUnityMainTargetGuid();
        project.AddCapability(unityMainTargetId, PBXCapabilityType.GameCenter, mainEntitlementsFileName);
    }

    public void AddCapabilitySignInWithApple()
    {
#if USE_SIGNIN_WITH_APPLE && UNITY_IOS
        projectCapability_sub.AddSignInWithAppleWithCompatibility();
#endif
    }

    /// <summary>
    /// </summary>
    /// <returns>true if saved</returns>
    private bool SaveCapability()
    {
        var saved = false;

        if (_projectCapability_main != null)
        {
            _projectCapability_main.WriteToFile();
            saved = true;
        }
        
        if (_projectCapability_sub != null)
        {
            _projectCapability_sub.WriteToFile();
            saved = true;
        }

        return saved;
    }
}
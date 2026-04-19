using UnityEditor.iOS.Xcode;

public partial class IosProjectFile
{
    public enum TargetType
    {
        UnityFramework,
        UnityMain,
        UnityMainTest,
    }

    private string projectPath;
    private string buildPath;
    private PBXProject project;
    
    public IosProjectFile(string buildPath)
    {
        this.buildPath = buildPath;
        projectPath = PBXProject.GetPBXProjectPath(buildPath);
        project = new PBXProject();
        project.ReadFromFile(projectPath);
    }

    public void AddFramework(TargetType targetType, string frameworkName)
    {
        var targetId = targetType switch
        {
            TargetType.UnityFramework => project.GetUnityFrameworkTargetGuid(),
            TargetType.UnityMain => project.GetUnityMainTargetGuid(),
            TargetType.UnityMainTest => project.GetUnityMainTestTargetGuid(),
            _ => null,
        };

        project.AddFrameworkToProject(targetId, frameworkName, weak: true);
    }

    public void Save()
    {
        var savedProj = SaveCapability();
        if (!savedProj)
        {
            project.WriteToFile(projectPath);
        }
    }
}
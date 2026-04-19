
using UnityEngine;

public enum GameScreenOrientation
{
    Portrait,
    Landscape,
}

public partial class GameFrameworkConfig
{
    [Header("build player")] 
    public int androidTargetSDK = 35;
    public GameScreenOrientation screenOrientation;
    // public string androidKeystorePassword = "Hopper#123";
    public string androidKeystorePassword = "";
    public string slackChannelNameForBuildNote ="";
    public string slackBotTokenForBuildNote = "";
    
    private static readonly Vector2Int landscapeWindowSize = new(1802, 942);
    private static readonly Vector2Int portraitWindowSize = new(520, 968);
    public Vector2Int buildWindowSize
    {
        get
        {
            return screenOrientation switch
            {
                GameScreenOrientation.Portrait => portraitWindowSize,
                GameScreenOrientation.Landscape => landscapeWindowSize,
                _ => Vector2Int.zero,
            };
        }
    }
}

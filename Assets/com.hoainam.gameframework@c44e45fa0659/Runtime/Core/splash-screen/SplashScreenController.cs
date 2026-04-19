using UnityEngine;
using UnityEngine.UI;

public partial class SplashScreenController : MonoBehaviour
{
    [Header("others")]
    public Text txtVersion;
    public Image imgBackground;

    private void Start()
    {
        txtVersion.text = $"v{Application.version}";
        StaticUtils.ScaleBackgroundFullscreen_UI(imgBackground);
        
        Start_progress();
        SetLanguage_title(Application.systemLanguage);
    }

    private void Update()
    {
        Update_progress();
    }

    public void SetLanguage(SystemLanguage language)
    {
        SetLanguage_title(language);
    }
}

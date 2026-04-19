
using UnityEngine;
using UnityEngine.UI;

public partial class SplashScreenController
{
    [Header("progress")]
    public Text txtProgress;
    public Image imgProgress;
    public float startingProgress;
    public float endingProgress;
    public float increaseProgressSpeed = 10f;
    
    private float progress;

    private void Start_progress()
    {
        progress = startingProgress;
        SetProgress(startingProgress);
    }

    private void Update_progress()
    {
        progress += increaseProgressSpeed * Time.deltaTime;
        if (progress > endingProgress)
        {
            progress = endingProgress;
        }
        SetProgress(progress);
    }
    
    private void SetProgress(float progress)
    {
        txtProgress.text = $"{(int)progress}%";
        imgProgress.fillAmount = progress / 100f;
    }
}

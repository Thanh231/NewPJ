using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController_entry : MonoBehaviour
{
    public string nextSceneName;

    void Start()
    {
        StaticUtils.SetFPS(60);
        SceneManager.LoadScene(nextSceneName);
    }
}
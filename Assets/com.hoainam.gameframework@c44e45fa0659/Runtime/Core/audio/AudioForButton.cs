using R3;
using UnityEngine;
using UnityEngine.UI;

public class AudioForButton : MonoBehaviour
{
    public string audioName;

    private void Start()
    {
        var btn = GetComponent<Button>();
        btn.OnClickAsObservable().Subscribe(_ => { AudioController.instance.PlaySound(audioName); });
    }
}

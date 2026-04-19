
using UnityEngine;
using UnityEngine.Events;

public class GameplayButton : MonoBehaviour
{
    private UnityAction OnClick;

    public void AddListener(UnityAction action)
    {
        OnClick += action;
    }

    public void RemoveListener(UnityAction action)
    {
        OnClick -= action;
    }

    public void OnClicked()
    {
        OnClick?.Invoke();
    }
}
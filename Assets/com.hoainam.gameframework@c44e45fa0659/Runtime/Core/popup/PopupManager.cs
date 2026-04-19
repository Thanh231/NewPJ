
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupManager : SingletonMonoBehaviour<PopupManager>
{
    #region data members

    [SerializeField] private Canvas screenCanvas;
    [SerializeField] private Canvas worldCanvas;

    private List<BasePopup> lPopups = new List<BasePopup>();

    public UnityAction OnBackKeyWhenNoPopup;

    #endregion

    #region life cycle

    private void Update()
    {
        if (StaticUtils.IsPressBackKey())
        {
            if (lPopups.Count > 0)
            {
                var popup = lPopups[^1];
                if (popup.canCloseByBackKey && popup.allowClose)
                {
                    popup.onCloseUnexpected?.Invoke();
                    ClosePopup(popup);
                }
            }
            else
            {
                OnBackKeyWhenNoPopup?.Invoke();
            }
        }
    }

    #endregion

    #region open popup

    public async UniTask<T> OpenPopupWorld<T>() where T : BasePopup
    {
        var prefabName = typeof(T).Name;
        return await OpenPopup<T>(worldCanvas.transform, prefabName);
    }

    public async UniTask<T> OpenPopup<T>() where T : BasePopup
    {
        var prefabName = typeof(T).Name;
        return await OpenPopup<T>(screenCanvas.transform, prefabName);
    }

    public async UniTask<T> OpenPopup<T>(string prefabName)
    {
        var pu = await OpenPopup(screenCanvas.transform, prefabName);
        return pu.GetComponent<T>();
    }

    public async UniTask<GameObject> OpenPopup(string prefabName)
    {
        return await OpenPopup(screenCanvas.transform, prefabName);
    }

    private async UniTask<T> OpenPopup<T>(Transform parent, string prefabName) where T : BasePopup
    {
        var pu = await OpenPopup(parent, prefabName);
        return pu.GetComponent<T>();
    }

    private async UniTask<GameObject> OpenPopup(Transform parent, string prefabName)
    {
        var path = GameFrameworkConfig.instance.popupAddressablePath;
        var prefab = await AssetManager.instance.LoadPrefab(path, prefabName);
        var popup = Instantiate(prefab, parent);
        lPopups.Add(popup.GetComponent<BasePopup>());
        return popup;
    }

    #endregion

    #region close popup

    public void ClosePopup(BasePopup popup, bool closeImmediately = false)
    {
        lPopups.Remove(popup);
        popup.OnClosePopup(!closeImmediately);
    }

    public void CloseAllPopup()
    {
        foreach (var i in lPopups)
        {
            i.OnClosePopup(isRunAnim: false);
        }
        lPopups.Clear();
    }

    #endregion

    #region public

    public bool IsAnyPopupOpening()
    {
        return lPopups.Count > 0;
    }

    public T GetPopup<T>() where T : BasePopup
    {
        T result = null;
        var typeT = typeof(T);
        foreach (var i in lPopups)
        {
            if (i.GetType() == typeT)
            {
                result = (T)i;
                break;
            }
        }
        return result;
    }

    public void SetCameraForWorldCanvas(Camera camera)
    {
        worldCanvas.worldCamera = camera;
    }

    #endregion
}
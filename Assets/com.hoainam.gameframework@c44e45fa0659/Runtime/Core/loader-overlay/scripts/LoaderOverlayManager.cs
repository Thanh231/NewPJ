using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoaderOverlayManager : SingletonMonoBehaviour<LoaderOverlayManager>
{
    public Transform parentCanvas;
    public GameObject prefabOverlayClassic;
    public GameObject prefabOverlayUnityAnim;
    public LoaderOverlayType overlayType;
    
    private GameObject objOverlay;
    private GameObject prefabOverlay => overlayType switch
    {
        LoaderOverlayType.Classic => prefabOverlayClassic,
        LoaderOverlayType.UnityAnimation => prefabOverlayUnityAnim,
        _ => null
    };

    public static bool canSwitchScene = true;

    public void LoadScene(string sceneName)
    {
        if (!canSwitchScene)
        {
            return;
        }

        if (!objOverlay)
        {
            objOverlay = Instantiate(prefabOverlay, parentCanvas);
        }
        objOverlay.SetActive(true);

        var Iloader = objOverlay.GetComponent<ILoaderOverlay>();
        var duration = Iloader.PlayAnim_begin();

        DOVirtual.DelayedCall(duration, () =>
        {
            Iloader.PlayAnim_idle();
            SceneManager.LoadScene(sceneName);
        });
    }

    public void EndOverlay()
    {
        if (objOverlay && objOverlay.activeSelf)
        {
            var Iloader = objOverlay.GetComponent<ILoaderOverlay>();
            var duration = Iloader.PlayAnim_end();
            DOVirtual.DelayedCall(duration, () => { objOverlay.SetActive(false); });
        }
    }
}

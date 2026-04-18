using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;

public class TutorialViewManager : MonoBehaviour, ICanvasRaycastFilter
{
    public Sprite hiddenSprite;
    public Sprite linkedSprite;
    public RectTransform target;
    public float radiusPadding = 40f;
    public float edgeSoftness = 0.005f;
    public Material _mat;
    private Canvas _canvas;
    private Camera _cam;

    // Resolution mà các normalizedPos đang được calibrate
    private static readonly Vector2 ReferenceResolution = new Vector2(1080f, 1920f);

    private IDisposable _disposable;

    void Awake()
    {
        var image = GetComponent<Image>();
        image.material = _mat;

        _canvas = GetComponentInParent<Canvas>();
        _cam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null
            : _canvas.worldCamera;

        EventManager.OnStartGame += HandleStartGame;
        EventManager.OnUseHand += ProcessUseHand;
        EventManager.OnEndHand += ProcessEndHand;
        EventManager.OnUseSuperCat += ProcessUseSuperCat;
        EventManager.OnClickBlock += ProcessClickBlock;
        EventManager.onFullSlotTutorial += OnFullSlotTutorial;
    }

    void OnDisable()
    {
        EventManager.OnStartGame -= HandleStartGame;
        EventManager.OnUseHand -= ProcessUseHand;
        EventManager.OnEndHand -= ProcessEndHand;
        EventManager.OnUseSuperCat -= ProcessUseSuperCat;
        EventManager.OnClickBlock -= ProcessClickBlock;
        EventManager.onFullSlotTutorial -= OnFullSlotTutorial;
    }

    void Update()
    {
        if (target == null) return;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(_cam, target.position);
        Vector2 viewportPos = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);

        float halfWidth  = (target.rect.width  * target.lossyScale.x * 0.5f + radiusPadding) / Screen.width;
        float halfHeight = (target.rect.height * target.lossyScale.y * 0.5f + radiusPadding) / Screen.height;

        _mat.SetVector("_HoleCenter", new Vector4(viewportPos.x, viewportPos.y, 0, 0));
        _mat.SetVector("_HoleSize",   new Vector4(halfWidth, halfHeight, 0, 0));
        _mat.SetFloat("_EdgeSoftness", edgeSoftness);
    }

    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        if (target == null) return true;

        Vector2 targetScreenPos = RectTransformUtility.WorldToScreenPoint(_cam, target.position);
        float halfW = target.rect.width  * target.lossyScale.x * 0.5f + radiusPadding;
        float halfH = target.rect.height * target.lossyScale.y * 0.5f + radiusPadding;

        bool isInside = screenPoint.x >= targetScreenPos.x - halfW &&
                        screenPoint.x <= targetScreenPos.x + halfW &&
                        screenPoint.y >= targetScreenPos.y - halfH &&
                        screenPoint.y <= targetScreenPos.y + halfH;
        return !isInside;
    }

    // ─── Set Size Methods ──────────────────────────────────────────────────────

    public void SetTargetFullSize()
    {
        if (target == null) return;
        target.anchorMin  = Vector2.zero;
        target.anchorMax  = Vector2.one;
        target.offsetMin  = Vector2.zero;
        target.offsetMax  = Vector2.zero;
        target.localScale = Vector3.one;
    }

    public void SetTargetZeroSize()
    {
        if (target == null) return;
        target.sizeDelta        = Vector2.zero;
        target.localScale       = Vector3.zero;
        target.anchoredPosition = new Vector2(2000f, 2000f);
    }

    /// <summary>
    /// Đặt target theo normalized position trong screen space.
    /// normalizedPos (0,0) = góc dưới-trái màn hình, (1,1) = góc trên-phải.
    /// size tính theo canvas units của reference resolution 1080x1920, tự động scale.
    /// </summary>
    public void SetTargetCustomSize(Vector2 normalizedPos, Vector2 size)
    {
        if (target == null || _canvas == null) return;

        RectTransform canvasRect = _canvas.GetComponent<RectTransform>();

        // 1. Chuyển normalizedPos từ screen fraction → pixel position thực
        //    (0.5f, 0.15f) luôn = 50% chiều ngang, 15% từ đáy màn hình trên mọi thiết bị
        Vector2 screenPoint = new Vector2(
            normalizedPos.x * Screen.width,
            normalizedPos.y * Screen.height
        );

        // 2. Convert screen pixel → canvas local position
        //    ScreenPointToLocalPointInRectangle tự xử lý đúng mọi Canvas Scaler mode
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPoint, _cam, out Vector2 localPoint);

        // 3. Scale size từ reference 1080x1920 → canvas thực tế của thiết bị
        float widthScale  = canvasRect.rect.width  / ReferenceResolution.x;
        float heightScale = canvasRect.rect.height / ReferenceResolution.y;
        Vector2 scaledSize = new Vector2(size.x * widthScale, size.y * heightScale);

        target.anchorMin  = new Vector2(0.5f, 0.5f);
        target.anchorMax  = new Vector2(0.5f, 0.5f);
        target.pivot      = new Vector2(0.5f, 0.5f);
        target.sizeDelta  = scaledSize;
        target.anchoredPosition = localPoint;
        target.localScale = Vector3.one;
    }

    /// <summary>
    /// Đặt target khớp chính xác với một UI RectTransform trong scene.
    /// Dùng cách này thay cho SetTargetCustomSize khi có object tham chiếu cụ thể.
    /// </summary>
    public void SetTargetToObject(RectTransform sourceRect, Vector2 sizeOverride = default)
    {
        if (target == null || sourceRect == null) return;

        RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_cam, sourceRect.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPoint, _cam, out Vector2 localPoint);

        target.anchorMin  = new Vector2(0.5f, 0.5f);
        target.anchorMax  = new Vector2(0.5f, 0.5f);
        target.pivot      = new Vector2(0.5f, 0.5f);
        target.sizeDelta  = sizeOverride != default ? sizeOverride : sourceRect.rect.size;
        target.anchoredPosition = localPoint;
        target.localScale = Vector3.one;
    }

    // ─── Event Handlers ────────────────────────────────────────────────────────

    private void OnFullSlotTutorial()
    {
        if (TutorialController.GetTutorialItem(GuideTutorialType.Full_slot.ToString()).isCompleted.Value) return;

        SetTargetCustomSize(new Vector2(0.5f, 0.31f), new Vector2(800, 100));
        PopupManager.instance.OpenPopup<PopupFullSlot>().Forget();
        WatchhStepChange(GuideTutorialType.Full_slot.ToString());
        Time.timeScale = 0;
    }

    private void ProcessUseHand()
    {
        TutorialController.AdvanceStep(BoosterTutorialType.Booster_Balloon.ToString());
        SetTargetFullSize();
        PopupManager.instance.OpenPopup<PopupPickAnyCat>().Forget();
    }

    private void ProcessEndHand()
    {
        TutorialController.AdvanceStep(BoosterTutorialType.Booster_Balloon.ToString());
        SceneGameplayUI.instance.ResetButton();
        var popup = PopupManager.instance.GetPopup<PopupPickAnyCat>();
        if (popup != null) PopupManager.instance.ClosePopup(popup, true);
    }

    private void ProcessUseSuperCat()
    {
        PopupManager.instance.OpenPopup<PopupClickBlock>().Forget();
        TutorialController.AdvanceStep(BoosterTutorialType.Booster_Super.ToString());
        SetTargetCustomSize(new Vector2(0.5f, 0.65f), new Vector2(800, 900));
    }

    private void ProcessClickBlock(string s)
    {
        TutorialController.AdvanceStep(BoosterTutorialType.Booster_Super.ToString());
        SceneGameplayUI.instance.ResetButton();
        var popup = PopupManager.instance.GetPopup<PopupClickBlock>();
        if (popup != null) PopupManager.instance.ClosePopup(popup, true);
        SetTargetFullSize();
    }

    private void HandleStartGame()
    {
        int level = LevelController.GetMaxLevelUnlock();
        string tutorialKey;

        switch (level)
        {
            case 1:  tutorialKey = GuideTutorialType.Level_1.ToString(); break;
            case 2:  tutorialKey = GuideTutorialType.Level_2.ToString(); break;
            case 6:  tutorialKey = BoosterTutorialType.Booster_AddTray.ToString(); break;
            case 7:  tutorialKey = MechanicTutorialType.Mechanic_Hidden.ToString(); break;
            case 12: tutorialKey = BoosterTutorialType.Booster_Balloon.ToString(); break;
            case 13: tutorialKey = MechanicTutorialType.Mechanic_Link.ToString(); break;
            case 15: tutorialKey = BoosterTutorialType.Booster_Shuffle.ToString(); break;
            case 18: tutorialKey = BoosterTutorialType.Booster_Super.ToString(); break;
            default: tutorialKey = GuideTutorialType.Full_slot.ToString(); break;
        }

        if (!string.IsNullOrEmpty(tutorialKey) && !TutorialController.IsCompleted(tutorialKey))
        {
            TutorialController.ResetTutorialStep(tutorialKey);
            ExecuteTutorialLogic(level, tutorialKey);
        }
        else
        {
            SetTargetFullSize();
        }
    }

    private void ExecuteTutorialLogic(int level, string key)
    {
        switch (level)
        {
            case 1:
                WatchhStepChange(GuideTutorialType.Level_1.ToString());
                return;
            case 2:
                PopupManager.instance.OpenPopup<PopupLevel2>().Forget();
                SetTargetCustomSize(new Vector2(0.1f, 0.41f), new Vector2(200, 200));
                break;
            case 6:
                SetTargetZeroSize();
                PopupManager.instance.OpenPopup<PopupAddTray>().Forget();
                SceneGameplayUI.instance.InTutorial(BoosterTutorialType.Booster_AddTray);
                break;
            case 7:
                SetTargetFullSize();
                PopupManager.instance.OpenPopup<PopupHiddenMechanic>().Forget();
                break;
            case 12:
                SetTargetZeroSize();
                PopupManager.instance.OpenPopup<PopupBalloon>().Forget();
                SceneGameplayUI.instance.InTutorial(BoosterTutorialType.Booster_Balloon);
                break;
            case 13:
                SetTargetFullSize();
                PopupManager.instance.OpenPopup<PopupLinkMechanic>().Forget();
                break;
            case 15:
                SetTargetZeroSize();
                PopupManager.instance.OpenPopup<PopupShuffle>().Forget();
                SceneGameplayUI.instance.InTutorial(BoosterTutorialType.Booster_Shuffle);
                break;
            case 18:
                SetTargetZeroSize();
                PopupManager.instance.OpenPopup<PopupSuperCat>().Forget();
                SceneGameplayUI.instance.InTutorial(BoosterTutorialType.Booster_Super);
                break;
            default: 
                SetTargetFullSize();
                break;
        }

        WatchTutorialStatus(key);
    }

    // ─── Tutorial Watch ────────────────────────────────────────────────────────

    public void WatchTutorialStatus(string key)
    {
        _disposable?.Dispose();

        var isCompletedRx = TutorialController.IsCompletedRx(key);
        if (isCompletedRx == null) return;

        _disposable = isCompletedRx
            .Where(done => done)
            .Subscribe(_ =>
            {
                Debug.Log($"Tutorial {key} finished!");
                SetTargetFullSize();
            })
            .AddTo(this);
    }

    private void WatchhStepChange(string key)
    {
        _disposable?.Dispose();

        var item = TutorialController.GetTutorialItem(key);
        if (item == null || item.isCompleted.Value) return;

        item.currentStep
            .Subscribe(step =>
            {
                Debug.Log($"Step changed: {step} for {key}");
                HandleStepChanged(key, step);
            })
            .AddTo(this);

        item.isCompleted
            .Where(done => done)
            .Subscribe(_ =>
            {
                Debug.Log($"Tutorial {key} finished!");
                SetTargetFullSize();
            })
            .AddTo(this);
    }

    private void HandleStepChanged(string key, int step)
    {
        if (key == GuideTutorialType.Level_1.ToString())
        {
            switch (step)
            {
                case 0:
                    PopupManager.instance.OpenPopup<PopupLevel1>().Forget();
                    SetTargetCustomSize(new Vector2(0.5f, 0.15f), new Vector2(250, 600));
                    break;
                case 1:
                    SetTargetCustomSize(new Vector2(0.5f, 0.65f), new Vector2(800, 900));
                    break;
                case 2:
                    SetTargetCustomSize(new Vector2(0.5f, 0.31f), new Vector2(800, 100));
                    break;
                default:
                    var popup = PopupManager.instance.GetPopup<PopupLevel1>();
                    if (popup != null) PopupManager.instance.ClosePopup(popup, true);
                    SetTargetFullSize();
                    break;
            }
        }
    }
}
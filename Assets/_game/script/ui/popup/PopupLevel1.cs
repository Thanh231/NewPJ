using TMPro;
using UnityEngine;
using R3;
using Spine.Unity;

public class PopupLevel1 : BasePopup
{
    public TextMeshProUGUI des;
    private RectTransform _rect;
    private RectTransform _rectHand;
    public GameObject skeletonAnimation;

    private Canvas _canvas;
    private static readonly Vector2 ReferenceResolution = new Vector2(1080f, 1920f);

    protected override void Start()
    {
        base.Start();

        _canvas   = GetComponentInParent<Canvas>();
        _rectHand = skeletonAnimation.GetComponent<RectTransform>();
        _rect     = GetComponent<RectTransform>();

        if (des == null)
        {
            Debug.LogError($"[PopupLevel1] Biến 'des' chưa được kéo vào Inspector trên {gameObject.name}!");
            return;
        }

        skeletonAnimation.SetActive(true);

        var key  = GuideTutorialType.Level_1.ToString();
        var item = TutorialController.GetTutorialItem(key);

        if (item != null && !item.isCompleted.Value)
        {
            item.currentStep
                .Subscribe(step =>
                {
                    if (des == null || _rect == null) return;

                    Debug.Log($"Step changed: {step} for {key}");

                    switch (step)
                    {
                        case 0:
                            des.text = "Pick cat and start collecting yarn!";
                            _rect.anchoredPosition     = ScalePos(new Vector2(0f,    250f));
                            _rectHand.anchoredPosition = ScalePos(new Vector2(-25f,   -800f));
                            skeletonAnimation.SetActive(true);
                            break;
                        case 1:
                            des.text = "Wait for the cat to travel!";
                            _rect.anchoredPosition = ScalePos(new Vector2(0f, -500f));
                            skeletonAnimation.SetActive(false);
                            break;
                        case 2:
                            des.text = "The cat isn't full yet. Send it out again!";
                            _rect.anchoredPosition     = ScalePos(new Vector2(0f,    250f));
                            _rectHand.anchoredPosition = ScalePos(new Vector2(-285f, -700f));
                            skeletonAnimation.SetActive(true);
                            break;
                        default:
                            break;
                    }
                })
                .AddTo(this);
        }
    }

    /// <summary>
    /// Scale anchoredPosition từ reference 1080x1920 sang canvas thực tế của thiết bị.
    /// </summary>
    private Vector2 ScalePos(Vector2 refPos)
    {
        if (_canvas == null) return refPos;
        RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
        float scaleX = canvasRect.rect.width  / ReferenceResolution.x;
        float scaleY = canvasRect.rect.height / ReferenceResolution.y;
        return new Vector2(refPos.x * scaleX, refPos.y * scaleY);
    }
}
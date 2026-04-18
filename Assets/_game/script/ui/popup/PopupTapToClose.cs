using R3;
using UnityEngine;
using UnityEngine.UI;

public class PopupLevel2 : BasePopup
{
    public Button _button;

    protected override void Start()
    {
        base.Start();
        _button.OnClickAsObservable()
            .Subscribe(_ =>
            {
                TutorialController.AdvanceStep(GuideTutorialType.Level_2.ToString());
                ClosePopup();
                HapticController.PlayHaptic(HapticType.valid_button);
            }
            )
            .AddTo(this);
    }
}

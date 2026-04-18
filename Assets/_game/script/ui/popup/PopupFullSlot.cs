using UnityEngine;
using R3; // Đảm bảo dùng R3

public class PopupFullSlot : BasePopup
{
    protected override void Start()
    {
        base.Start();

        var key = GuideTutorialType.Full_slot.ToString();
        var item = TutorialController.GetTutorialItem(key);

        if (item != null)
        {
            item.currentStep.Where(step => step >= item.totalSteps)
                .Subscribe(_ =>
                {
                    ClosePopup(); 
                })
                .AddTo(this);
        }
    }
}
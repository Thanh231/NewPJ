
using R3;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupTermGlobal : BasePopup
{
    public TextAsLink linkPolicy;
    public Button btnClose;

    public UnityAction closeEvent;

    protected override void Start()
    {
        base.Start();

        linkPolicy.OnLinkClicked += _ =>
        {
            MobirixStaticUtils.OpenPrivacyPolicy(false);
        };

        btnClose.OnClickAsObservable().Subscribe(_ =>
        {
            closeEvent?.Invoke();
            ClosePopup();
        });
    }

    protected override void AfterRunAnimClose()
    {
        base.AfterRunAnimClose();

        SystemNotificationController.instance.RequestPermission();
    }
}

using System;
using Cysharp.Threading.Tasks;

public class PopupResultEnableSystemNotice : BasePopup
{
    public LocalizedText_tmp txtMsg;

    private bool isAgree;

    public static async UniTask OpenPopup(bool isAgree, bool isNoticeAtNight)
    {
        var pu = await PopupManager.instance.OpenPopup<PopupResultEnableSystemNotice>();
        var time = DateTime.Now;
        var gameTitle = new LocalizedTextParameter(GameFrameworkConfig.instance.appNameLocalizedKey);
        var msgParam = new object[] {time.Year, time.Month, time.Day, gameTitle};
        var msgKey = GetMsgKey(isAgree, isNoticeAtNight);

        pu.txtMsg.SetKeyAndParameters(msgKey, msgParam);
        pu.isAgree = isAgree;
    }

    private static string GetMsgKey(bool isAgree, bool isNoticeAtNight)
    {
        if (isAgree)
        {
            return isNoticeAtNight ? "msg_night_notice_agree" : "msg_notice_agree";
        }
        else
        {
            return isNoticeAtNight ? "msg_night_notice_disagree" : "msg_notice_disagree";
        }
    }

    protected override void AfterRunAnimClose()
    {
        base.AfterRunAnimClose();

        if (isAgree)
        {
            SystemNotificationController.instance.RequestPermission();
        }
    }
}
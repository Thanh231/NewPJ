
using UnityEngine;

public static class MobirixStaticUtils
{
    public static void OpenCustomerService()
    {
        var uid = ServerUsersManager.instance.userUID.Value;
        var gameCode = GameFrameworkConfig.instance.gameIdForCustomerCare;
        var url = $"https://help.mobirix.com/lang/inquiry/write?game_idx={gameCode}&uid={uid}";
        Application.OpenURL(url);
    }

    public static void OpenRefundPolicy()
    {
        var url = "http://www.mobirix.com/refundkr.html";
        Application.OpenURL(url);
    }

    public static void OpenFacebookFanPage()
    {
        var url = "https://www.facebook.com/mobirixplayen";
        Application.OpenURL(url);
    }

    public static void OpenPlayStorePage()
    {
        var packageName = Application.identifier;
        var url = $"https://play.google.com/store/apps/details?id={packageName}";
        Application.OpenURL(url);
    }

    public static void OpenMobirixTermPage()
    {
        var url = "https://policy.mobirix.com/terms?lang=ko";
        Application.OpenURL(url);
    }

    public static void OpenPrivacyPolicy(bool isKorean)
    {
        var packageName = Application.identifier;
        var platform = "";
        var lang = isKorean ? "ko" : "en";

#if UNITY_ANDROID
		platform = "aos";
#elif UNITY_IOS
		platform = "ios";
#endif

        var url = $"https://policy.mobirix.com/personal?lang={lang}&game={packageName}&os={platform}";
        Application.OpenURL(url);
    }
}

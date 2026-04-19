
using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.CrashReportHandler;

public partial class ServerUsersManager
{
    public ReactiveProperty<string> userUID = new();
    public bool IsLoggedIn => !string.IsNullOrEmpty(userUID.Value);
    
    /// <summary>
    /// login or link account
    /// </summary>
    /// <param name="providerType">is login using Google, apple or something else</param>
    /// <param name="linkAccount">true: from settings popup; false: login when start game</param>
    /// <returns>login success or not</returns>
    public async UniTask<bool> Login(SocialSignInProviderType providerType, bool linkAccount)
    {
        try
        {
            var testUserId = GameFrameworkConfig.instance.testUserId;
            var needDownloadData = false;
            var needUploadData = false;
            if (string.IsNullOrEmpty(testUserId))
            {
                var signInResult = await ServerController.instance.SignIn(providerType, linkAccount);
                userUID.Value = signInResult.uid;
                needDownloadData = signInResult.needDownloadData;
                needUploadData = signInResult.needUploadData;
            }
            else
            {
                userUID.Value = testUserId;
                needDownloadData = linkAccount;
            }
            
            Debug.Log($"[Server] login successful with id={userUID.Value}");
            
            await OnPostLogin(needDownloadData, needUploadData, !linkAccount);
            
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }
    }

    private async UniTask OnPostLogin(bool needDownloadData, bool needUploadData, bool loginWhenStartGame)
    {
        UpdatePropertiesAfterLogin().Forget();
        CrashReportHandler.SetUserMetadata("user_id", userUID.Value);
        
#if DETECT_MULTI_DEVICES_LOGIN
        if (loginWhenStartGame)
        {
            MultiDevicesLoginController.instance.Init(userUID.Value).Forget();
        }
#endif
        
#if USE_FIREBASE_ANALYTICS
        FirebaseController.instance.SetAnalyticsUserId(userUID.Value).Forget();
#endif
        
#if USE_FIREBASE_CRASHLYTICS
        FirebaseController.instance.SetCrashlyticsUserId(userUID.Value).Forget();
#endif

        if (needDownloadData)
        {
            await PlayerModelManager.instance.DownloadPlayerModel();
        }

        if (needUploadData)
        {
            await PlayerModelManager.instance.UploadPlayerModel();
        }
    }

    public async UniTask Logout(SocialSignInProviderType providerType)
    {
        await PlayerModelManager.instance.UploadPlayerModel();
        await ServerController.instance.SignOut(providerType);
        PlayerModelManager.instance.ClearAllModels();
        GameReloader.instance.Reload();
    }
}
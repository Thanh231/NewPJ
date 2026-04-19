
#if USE_INGAME_UPDATE

using Cysharp.Threading.Tasks;
using Google.Play.AppUpdate;
using UnityEngine;

public class UpdateGameImpl_android : IUpdateGame
{
    private AppUpdateManager appUpdateManager;

    public UpdateGameImpl_android()
    {
        appUpdateManager = new AppUpdateManager();
    }

    public async UniTask OptionalUpdate()
    {
        var appUpdateInfo = await HasUpdateAsync();
        if (appUpdateInfo == null)
        {
            return;
        }
        var appUpdateOptions = AppUpdateOptions.FlexibleAppUpdateOptions();
        var startUpdateOp = appUpdateManager.StartUpdate(appUpdateInfo, appUpdateOptions);
        await startUpdateOp.ToUniTask();
    }

    private async UniTask<AppUpdateInfo> HasUpdateAsync()
    {
        var appUpdateInfoOp = appUpdateManager.GetAppUpdateInfo();
        await appUpdateInfoOp.ToUniTask();

        if (appUpdateInfoOp.IsSuccessful)
        {
            var appUpdateInfo = appUpdateInfoOp.GetResult();
            var hasUpdate = appUpdateInfo.UpdateAvailability == UpdateAvailability.UpdateAvailable;
            var supportFlexible = appUpdateInfo.IsUpdateTypeAllowed(AppUpdateOptions.FlexibleAppUpdateOptions());

            Debug.Log($"[InGameUpdate] HasUpdate: {hasUpdate}, SupportFlexible: {supportFlexible}");
            
            return hasUpdate && supportFlexible ? appUpdateInfo : null;
        }
        else
        {
            Debug.LogError($"[InGameUpdate] Failed to get app update info: {appUpdateInfoOp.Error}");
            return null;
        }
    }
}

#endif
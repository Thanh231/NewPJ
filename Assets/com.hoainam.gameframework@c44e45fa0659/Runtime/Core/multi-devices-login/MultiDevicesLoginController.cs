
#if DETECT_MULTI_DEVICES_LOGIN

using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public class MultiDevicesLoginController : SingletonMonoBehaviour<MultiDevicesLoginController>
{
    public UnityAction OnMultiDevicesLoginDetected;
    
    private string sessionId;

    public async UniTask Init(string userId)
    {
        sessionId = Guid.NewGuid().ToString();
        StaticUtils.LogFramework($"[MultiDevicesLogin] sessionId: {sessionId}, userId: {userId}");

        var key = $"users_session/{userId}";
        await FirebaseController.instance.RealtimeDatabaseSetValue(key, sessionId);
        
        FirebaseController.instance.RealtimeDatabaseSubscribeValueChanged(key, value =>
        {
            if (value != sessionId)
            {
                StaticUtils.LogFramework(
                    $"[MultiDevicesLogin] new devices login detected! current sessionId: {sessionId}, new sessionId: {value}");
                
                OnMultiDevicesLoginDetected?.Invoke();
            }
        });
    }
}

#endif
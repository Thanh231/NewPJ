
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UtcTime : SingletonMonoBehaviour<UtcTime>
{
    private DateTime? lastServerTime;
    private float lastTimeGetServerTime;
    
    private bool alreadyDebugLog;
    
    public DateTime UtcNow
    {
        get
        {
            DateTime result;
            string debugLogMsg;

            if (GameFrameworkConfig.instance.testUseDeviceTime)
            {
                result = DateTime.UtcNow;
                debugLogMsg = "using testing device time";
            }
            else
            {
                if (lastServerTime != null)
                {
                    var dt = Time.realtimeSinceStartup - lastTimeGetServerTime;
                    result = lastServerTime.Value.AddSeconds(dt);
                    debugLogMsg = "using server time";
                }
                else
                {
                    result = UnbiasedTime.Instance.Now().ToUniversalTime();
                    debugLogMsg = "using device time";
                }
            }

            if (!alreadyDebugLog)
            {
                Debug.Log($"[UtcTime] {debugLogMsg}");
                alreadyDebugLog = true;
            }

            return result;
        }
    }

    public async UniTask Init()
    {
        try
        {
            lastServerTime = await ServerController.instance.GetUTCNow();
            lastTimeGetServerTime = Time.realtimeSinceStartup;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}

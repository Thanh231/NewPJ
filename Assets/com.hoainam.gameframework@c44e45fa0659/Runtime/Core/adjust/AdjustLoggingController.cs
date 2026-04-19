#if USE_ADJUST_LOGGING

using AdjustSdk;
using UnityEngine;

public partial class AdjustLoggingController : SingletonMonoBehaviour<AdjustLoggingController>
{
    protected override void Awake()
    {
        base.Awake();

        var appToken = GameFrameworkConfig.instance.adjustAppToken;
        var cfg = new AdjustConfig(appToken, AdjustEnvironment.Production);

        cfg.SessionSuccessDelegate += OnInitSuccess;
        cfg.SessionFailureDelegate += OnInitFailure;
        cfg.EventSuccessDelegate += OnSendEventSuccess;
        cfg.EventFailureDelegate += OnSendEventFailure;

        Adjust.InitSdk(cfg);
    }

    private void OnSendEventFailure(AdjustEventFailure obj)
    {
        Debug.LogError($"[Adjust] Event Failure: {obj.EventToken}, Message: {obj.Message}, AdId: {obj.Adid}");
    }

    private void OnSendEventSuccess(AdjustEventSuccess obj)
    {
        Debug.Log($"[Adjust] Event Success: {obj.EventToken}, Message: {obj.Message}, AdId: {obj.Adid}");
    }

    private void OnInitFailure(AdjustSessionFailure obj)
    {
        Debug.LogError($"[Adjust] Init Failure: Message: {obj.Message}, AdId: {obj.Adid}");
    }

    private void OnInitSuccess(AdjustSessionSuccess obj)
    {
        Debug.Log($"[Adjust] Init Success: Message: {obj.Message}, AdId: {obj.Adid}");
    }
}

#endif
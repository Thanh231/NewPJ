
#if CLOUD_CODE_USE_AWS

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public partial class AWSController : IServer_cloudCode
{
    public async UniTask<bool> ValidateGooglePlayReceipt(string pid, string purchaseToken, bool isSubscriptionProduct,
        string orderNumber, decimal price)
    {
        var url = GameFrameworkConfig.instance.awsLambdaUrlValidateGooglePlayReceipt;
        var dicParams = new Dictionary<string, string>()
        {
            { "productId", pid },
            { "purchaseToken", purchaseToken },
            { "isSubscription", isSubscriptionProduct.ToString() },
            { "orderNumber", orderNumber },
            { "price", StaticUtils.DecimalToString(price) },
            { "currency", IAPController.instance.CurrencyCode },
            { "userId", ServerUsersManager.instance.userUID.Value },
        };
        var res = await StaticUtils.GetHttpRequest(url, true, dicParams);
        return StaticUtils.StringToBool(res.resultAsText);
    }

    public async UniTask<bool> ValidateAppStoreReceipt(string payload, string transactionId, string productId, decimal price)
    {
        var url = GameFrameworkConfig.instance.awsLambdaUrlValidateAppStoreReceipt;
        var dicParams = new Dictionary<string, string>()
        {
            { "payload", payload },
            { "transactionId", transactionId },
            { "productId", productId },
            { "price", StaticUtils.DecimalToString(price) },
            { "currency", IAPController.instance.CurrencyCode },
            { "userId", ServerUsersManager.instance.userUID.Value },
        };
        var res = await StaticUtils.GetHttpRequest(url, true, dicParams);
        return StaticUtils.StringToBool(res.resultAsText);
    }

    public async UniTask<DateTime> GetUTCNow()
    {
        var url = GameFrameworkConfig.instance.awsLambdaUrlGetUtcTime;
        var res = await StaticUtils.GetHttpRequest(url, true);
        var t = StaticUtils.StringToDateTime(res.resultAsText);
        return new DateTime(t.Ticks, DateTimeKind.Utc);
    }
}

#endif
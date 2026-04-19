
using System;
using Cysharp.Threading.Tasks;

public interface IServer_cloudCode
{
    UniTask<bool> ValidateGooglePlayReceipt(string pid, string purchaseToken, bool isSubscriptionProduct,
        string orderNumber, decimal price);
    UniTask<bool> ValidateAppStoreReceipt(string payload, string transactionId, string productId, decimal price);
    UniTask<DateTime> GetUTCNow();
}


using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Samsung;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

//on beta test stage, you may encounter this issue:
//after upgrade app -> buy item in shop -> get "Product does not exist in this store" error
//you need a fresh installation in this stage
//refer: https://forum.developer.samsung.com/t/store-error-message-on-updating-closed-beta-test-app/3451 

public class IAPImplementation_samsung : IIAPImplementation
{
    #region core

    private List<ProductVo> lProducts;
    
    public async UniTask Init()
    {
        await UniTask.CompletedTask;
        
        //spawn samsung iap object
        var prefab = Resources.Load<GameObject>("samsung-iap-controller");
        UnityEngine.Object.Instantiate(prefab);

        //get list product info
        SamsungIAP.Instance.GetProductsDetails("", l =>
        {
            if (l != null)
            {
                lProducts = l.results;

                var productId = IAPController.instance.GetSpecificConsumablePID();
                IAPController.instance.CurrencyCode = GetCurrencyCode(productId);
            }
        });

        //get subscription info
        QuerySubscriptionInfo(l =>
        {
            foreach (var i in l)
            {
                OnPurchaseSuccess(i.mItemId, i.mPurchaseId);
            }
        });
    }

    #endregion
    
    #region get meta

    private ProductVo GetProduct(string productId)
    {
        if (lProducts == null)
        {
            throw new Exception("[IAP] lProducts is null");
        }

        var p = lProducts.Find(x => x.mItemId == productId);
        if (p == null)
        {
            throw new Exception($"[IAP] product {productId} is unavailable");
        }
        return p;
    }
    
    private string GetCurrencyCode(string productId)
    {
        var product = GetProduct(productId);
        return product.mCurrencyCode;
    }

    public string GetPriceAsString(string productId)
    {
        var product = GetProduct(productId);
        var price = StaticUtils.StringToDecimal(product.mItemPrice);
        return IAPController.instance.FormatPriceString(price, product.mCurrencyCode);
    }

    public decimal GetPriceAsNumber(string productId)
    {
        var product = GetProduct(productId);
        return StaticUtils.StringToDecimal(product.mItemPrice);
    }

    #endregion

    #region purchase

    public void Purchase(string productId)
    {
        SamsungIAP.Instance.StartPayment(productId, passThroughParam: "", result =>
        {
            if (result.errorInfo.errorCode == 0)
            {
                OnPurchaseSuccess(productId, result.results.mPurchaseId);
            }
            else
            {
                OnPurchaseFail(productId, result);
            }
        });
    }

    private void OnPurchaseFail(string productId, PurchasedInfo result)
    {
        Debug.LogError($"[IAP] purchase {productId} fail, reason={JsonConvert.SerializeObject(result.errorInfo)}");
        IAPController.instance.OnPurchaseFailed(IAPPurchaseFailReason.ErrorFromSamsungStore);
    }

    private void OnPurchaseSuccess(string productId, string purchaseId)
    {
        IAPController.instance.OnRewardPlayer();
        SamsungIAP.Instance.ConsumePurchasedItems(purchaseId, null);
        if (IAPController.instance.IsSubscriptionPID(productId))
        {
            QuerySubscriptionInfo();
        }
    }

    #endregion

    #region subscription
    
    private void QuerySubscriptionInfo(UnityAction<List<OwnedProductVo>> callback = null)
    {
        SamsungIAP.Instance.GetOwnedList(Samsung.ItemType.all, l =>
        {
            var dicSubscriptionInfo = new Dictionary<string, DateTime>();
            var subscriptionProducts = IAPController.instance.GetListSubscriptionPID();
            foreach (var i in subscriptionProducts)
            {
                dicSubscriptionInfo.Add(i, GetSubscriptionExpire(i, l.results));
            }
            IAPController.instance.OnQuerySubscriptionInfo(dicSubscriptionInfo);

            callback?.Invoke(l.results);
        });
    }

    private DateTime GetSubscriptionExpire(string id, List<OwnedProductVo> lProducts)
    {
        var product = lProducts.Find(x => x.mItemId.Equals(id));
        if (product == null)
        {
            return new DateTime();
        }
        
        return StaticUtils.StringToDateTime(product.mSubscriptionEndDate);
    }

    #endregion
    
    #region unused

    public void RestorePurchase()
    {
    }

    #endregion
}

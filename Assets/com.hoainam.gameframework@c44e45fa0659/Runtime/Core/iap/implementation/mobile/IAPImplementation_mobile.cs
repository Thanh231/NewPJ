
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

public abstract class IAPImplementation_mobile : IIAPImplementation
{
    #region core

    private StoreController storeController;

    public async UniTask Init()
    {
        storeController = UnityIAPServices.StoreController();

        storeController.OnProductsFetchFailed += OnProductsFetchFailed;
        storeController.OnProductsFetched += OnProductsFetched;
        storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
        storeController.OnPurchaseDeferred += OnPurchaseDeferred;
        storeController.OnPurchaseFailed += OnPurchaseFailed;
        storeController.OnPurchasePending += OnPurchasePending;
        storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
        storeController.OnPurchasesFetched += OnPurchasesFetched;
        storeController.OnStoreDisconnected += OnStoreDisconnected;

        await ConnectToStore();
        await FetchProducts();
        await FetchPurchases();

        OnInitialized();
    }

    protected virtual void OnInitialized()
    {
        IAPController.instance.CurrencyCode = GetCurrencyCode();
        StaticUtils.LogFramework($"[IAP] detected currency code: {IAPController.instance.CurrencyCode}");
    }

    private string GetCurrencyCode()
    {
        var lPids = IAPController.instance.GetListConsumablePID();
        foreach (var pid in lPids)
        {
            try
            {
                var product = GetProduct(pid);
                return product.metadata.isoCurrencyCode;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        throw new Exception("there is no available consumable product to get currency code");
    }

    protected Product GetProduct(Order order)
    {
        var lItems = order.CartOrdered.Items();
        return lItems.First().Product;
    }
    
    protected Product GetProduct(string productId)
    {
        var product = storeController.GetProductById(productId);
        if (product == null || !product.availableToPurchase)
        {
            throw new Exception($"[IAP] product {productId} is unavailable");
        }

        return product;
    }

    #endregion

    #region connect to store

    private async UniTask ConnectToStore()
    {
        await storeController.Connect();
    }
    
    private void OnStoreDisconnected(StoreConnectionFailureDescription description)
    {
        Debug.LogError($"[IAP] store disconnected: {description.Message}");
    }

    #endregion

    #region fetch products

    private UniTaskCompletionSource utcsFetchProducts;
    
    protected virtual List<ProductDefinition> GetListProductsToFetch()
    {
        var lProduct = new List<ProductDefinition>();
        var lConsumable = IAPController.instance.GetListConsumablePID();
        var lSubscription = IAPController.instance.GetListSubscriptionPID();

        foreach (var pid in lConsumable)
        {
            lProduct.Add(new ProductDefinition(pid, ProductType.Consumable));
        }

        foreach (var pid in lSubscription)
        {
            lProduct.Add(new ProductDefinition(pid, ProductType.Subscription));
        }
        
        return lProduct;
    }
    
    private UniTask FetchProducts()
    {
        utcsFetchProducts = new UniTaskCompletionSource();
        
        var lProduct = GetListProductsToFetch();
        storeController.FetchProducts(lProduct);

        return utcsFetchProducts.Task;
    }
    
    private void OnProductsFetched(List<Product> fetchedProducts)
    {
        utcsFetchProducts.TrySetResult();
    }

    private void OnProductsFetchFailed(ProductFetchFailed failure)
    {
        if (failure.FailedFetchProducts != null && failure.FailedFetchProducts.Count > 0)
        {
            Debug.LogError($"[IAP] there are {failure.FailedFetchProducts.Count} products failed to fetch:");
            foreach (var failedProduct in failure.FailedFetchProducts)
            {
                Debug.LogError($"   - product id={failedProduct.id}");
            }
        }
        else
        {
            Debug.LogError($"[IAP] fetch products failed: {failure.FailureReason}");
        }
    }

    #endregion

    #region fetch purchases

    private UniTaskCompletionSource utcsFetchPurchases;
    
    private UniTask FetchPurchases()
    {
        utcsFetchPurchases = new UniTaskCompletionSource();
        
        storeController.FetchPurchases();
        
        return  utcsFetchPurchases.Task;
    }
    
    private void OnPurchasesFetched(Orders purchases)
    {
        utcsFetchPurchases.TrySetResult();
    }

    private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription failure)
    {
        Debug.LogError($"[IAP] fetch purchases failed: reason={failure.FailureReason} msg={failure.Message}");
    }

    #endregion

    #region confirm pending purchase

    private void ConfirmPendingPurchase(PendingOrder order)
    {
        StaticUtils.LogFramework("[IAP] start confirm pending purchase");
        storeController.ConfirmPurchase(order);
    }

    private void OnPurchaseConfirmed(Order order)
    {
        if (order is ConfirmedOrder confirmedOrder)
        {
            OnPurchaseConfirmedSuccess(confirmedOrder);
        }
        else if (order is FailedOrder failedOrder)
        {
            var pid = GetProduct(failedOrder).definition.id;
            Debug.LogError(
                $"[IAP] confirm pending purchase failed product={pid} reason={failedOrder.FailureReason} msg={failedOrder.Details}");
        }
        else
        {
            Debug.LogError($"[IAP] confirm pending purchase unknown order type: {order.GetType().Name}");
        }
    }

    private void OnPurchaseConfirmedSuccess(ConfirmedOrder order)
    {
        StaticUtils.LogFramework("[IAP] confirm pending purchase success");
        
        var pid = GetProduct(order).definition.id;
        
        IAPController.instance.OnRewardPlayer();
        if (IAPController.instance.IsSubscriptionPID(pid))
        {
            QuerySubscriptionInfo();
        }
    }

    #endregion
    
    #region purchase

    private void OnPurchaseFailed(FailedOrder failure)
    {
        var pid = GetProduct(failure).definition.id;
        Debug.LogError($"[IAP] purchase failed product={pid} reason={failure.FailureReason} msg={failure.Details}");

        var reason = IAPController.instance.ConvertPurchaseFailReason(failure.FailureReason);
        IAPController.instance.OnPurchaseFailed(reason);
    }
    
    private void OnPurchaseDeferred(DeferredOrder order)
    {
        var pid = GetProduct(order).definition.id;
        Debug.LogError($"[IAP] purchase deferred product={pid}");
        
        //need to show dialog to inform user that purchase is deferred
    }

    private void OnPurchasePending(PendingOrder order)
    {
        if (WillProcessThisPurchase(order))
        {
            OnPurchasePendingAsync(order).Forget();
        }
    }

    private async UniTask OnPurchasePendingAsync(PendingOrder order)
    {
        var delayForTestRepay = GameFrameworkConfig.instance.iapDelayForTestRepay;
        if (delayForTestRepay > 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delayForTestRepay));
        }
        
        StaticUtils.LogFramework($"[IAP] purchase success, start validate receipt: {order.Info.Receipt}");

        var validateResult = await ValidateReceipt(order);
        if (validateResult)
        {
            StaticUtils.LogFramework("[IAP] validate receipt success, confirm pending purchase");
            ConfirmPendingPurchase(order);
        }
        else
        {
            Debug.LogError($"[IAP] validate receipt failed, check receipt below");
            Debug.LogError(order.Info.Receipt);
            IAPController.instance.OnPurchaseFailed(IAPPurchaseFailReason.ValidateReceiptFail);
        }
    }

    /// <summary>
    /// on ios, sometimes, OnPurchasePending get called twice for some mysterious reason.
    /// override this method to skip processing duplicate purchase.
    /// </summary>
    /// <param name="order">order to check</param>
    /// <returns>false if order is duplicate</returns>
    protected virtual bool WillProcessThisPurchase(PendingOrder order)
    {
        return true;
    }
    
    protected abstract UniTask<bool> ValidateReceipt(Order order);

    protected void UserReportPurchaseException(Exception e, Order order, string summary)
    {
#if USE_USER_REPORT
        UserReportManager.instance.SendReport(new UserReportManager.ReportInfo()
        {
            description = $"transaction id={product.transactionID}",
            dimensions = new Dictionary<string, string>()
            {
                { "version", Application.version }
            },
            summary = summary,
            attachment = new List<UserReportManager.AttachmentInfo>()
            {
                new UserReportManager.AttachmentInfo()
                {
                    title="receipt.json",
                    content=product.hasReceipt?product.receipt:$"{product.definition.id} has no receipt"
                },
                new UserReportManager.AttachmentInfo()
                {
                    title="exception.txt",
                    content=e.ToString(),
                }
            },
        });
#endif
    }

    #endregion
    
    #region subscription utils

    private void QuerySubscriptionInfo()
    {
        var dicSubscriptionInfo = new Dictionary<string, DateTime>();
        var listSubscriptionPID = IAPController.instance.GetListSubscriptionPID();
        foreach (var i in listSubscriptionPID)
        {
            dicSubscriptionInfo.Add(i, GetSubscriptionExpire(i));
        }
        IAPController.instance.OnQuerySubscriptionInfo(dicSubscriptionInfo);
    }

    private DateTime GetSubscriptionExpire(string id)
    {
        var product = GetProduct(id);
        if (product.receipt == null)
        {
            return new DateTime();
        }
        var mng = new SubscriptionManager(product, intro_json: null);
        var info = mng.getSubscriptionInfo();

        return info.getExpireDate();
    }

    #endregion

    #region IIAPImplementation

    public string GetPriceAsString(string productId)
    {
        var product = GetProduct(productId);
        return IAPController.instance.FormatPriceString(product.metadata.localizedPrice,
            product.metadata.isoCurrencyCode);
    }

    public decimal GetPriceAsNumber(string productId)
    {
        var product = GetProduct(productId);
        return product.metadata.localizedPrice;
    }

    public void Purchase(string productId)
    {
        StaticUtils.LogFramework($"[IAP] start purchase product: {productId}");
        storeController.PurchaseProduct(productId);
    }

    public abstract void RestorePurchase();

    #endregion
}
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPController : SingletonMonoBehaviour<IAPController>
{
    #region core

    private IIAPImplementation impl;
    private IIAPListener listener;
    private Dictionary<string, ProductType> dicPID2Type;
    private Dictionary<string, string> dicUID2PID;
    private bool isIAPInProcess = false;
    
    public ReactiveProperty<bool> IsPlayPassRx { get; set; } = new(false);

    private const string PLAYER_PREFS_KEY_LAST_PURCHASED_UID = nameof(PLAYER_PREFS_KEY_LAST_PURCHASED_UID);
    
    /// <summary>
    /// to determine country of 1 player:
    /// - use system language: this is not accurate because many vietnamese use english system language for instance
    /// - use currency code from IAP store: more accurate because users hardly change their store country
    /// </summary>
    public string CurrencyCode { get; set; } = "";

    /// <summary>
    /// a PID may represent for multiple products in game.
    /// UID represents for unique product in game.
    /// </summary>
    /// <param name="dicPID2Type">to know a PID has type of consumable or subscription</param>
    /// <param name="dicUID2PID">to convert UID to PID</param>
    /// <param name="listener">a listener in client</param>
    public async UniTask Init(Dictionary<string, ProductType> dicPID2Type, Dictionary<string, string> dicUID2PID,
        IIAPListener listener)
    {
        this.listener = listener;
        this.dicPID2Type = dicPID2Type;
        this.dicUID2PID = dicUID2PID;
        if (impl == null)
        {
#if SAMSUNG_STORE
            impl = new IAPImplementation_samsung();
#else
            
#if UNITY_EDITOR || UNITY_STANDALONE
            impl = new IAPImplementation_editor();
#elif UNITY_ANDROID
			impl = new IAPImplementation_mobile_android();
#elif UNITY_IOS
			impl = new IAPImplementation_mobile_ios();
#endif
            
#endif
            await impl.Init();
        }
    }

    #endregion

    #region utils

    public string FormatPriceString(decimal price, string currencyCode)
    {
        var priceStr = StaticUtils.DecimalToFriendlyString(price);
        return $"{currencyCode} {priceStr}";
    }
    
    public List<string> GetListConsumablePID()
    {
        var res = new List<string>();
        foreach (var i in dicPID2Type)
        {
            if (i.Value == ProductType.Consumable)
            {
                res.Add(i.Key);
            }
        }

        return res;
    }
    
    public List<string> GetListSubscriptionPID()
    {
        var res = new List<string>();
        foreach (var i in dicPID2Type)
        {
            if (i.Value == ProductType.Subscription)
            {
                res.Add(i.Key);
            }
        }

        return res;
    }

    public string GetSpecificConsumablePID()
    {
        var l = GetListConsumablePID();
        return l[0];
    }

    public bool IsSubscriptionPID(string productId)
    {
        var l = GetListSubscriptionPID();
        return l.Contains(productId);
    }

    public IAPPurchaseFailReason ConvertPurchaseFailReason(PurchaseFailureReason pluginReason)
    {
        var str = pluginReason.ToString();
        try
        {
            return StaticUtils.StringToEnum<IAPPurchaseFailReason>(str);
        }
        catch (Exception e)
        {
            Debug.LogException(new Exception($"[IAP] unknown purchase fail reason from plugin: {str}"));
            return IAPPurchaseFailReason.UnknownFromPlugin;
        }
    }
    
    #endregion
    
    #region IIAPImplementation bridge

    /// <summary>
    /// in case a PID represents multiple products in game,
    /// use UID instead of PID to identify which product to get price for
    /// </summary>
    /// <param name="uid">unique ID to identify product in game</param>
    public string GetPriceAsString(string uid)
    {
        try
        {
            return impl.GetPriceAsString(dicUID2PID[uid]);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return "Buy";
        }
    }

    /// <summary>
    /// in case a PID represents multiple products in game,
    /// use UID instead of PID to identify which product to get price for
    /// </summary>
    /// <param name="uid">unique ID to identify product in game</param>
    public decimal GetPriceAsNumber(string uid)
    {
        try
        {
            return impl.GetPriceAsNumber(dicUID2PID[uid]);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return 0;
        }
    }

    /// <summary>
    /// in case a PID represents multiple products in game,
    /// use UID instead of PID to identify which product to purchase
    /// </summary>
    /// <param name="uid">unique ID to identify product in game</param>
    public void Purchase(string uid)
    {
        if (isIAPInProcess)
        {
            return;
        }

        try
        {
            PlayerPrefs.SetString(PLAYER_PREFS_KEY_LAST_PURCHASED_UID, uid);
            isIAPInProcess = true;
            impl.Purchase(dicUID2PID[uid]);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            OnPurchaseFailed(IAPPurchaseFailReason.NotInitializedController);
        }
    }

    public void RestorePurchase()
    {
        impl.RestorePurchase();
    }

    #endregion

    #region IIAPListener bridge

    public void OnPurchaseFailed(IAPPurchaseFailReason reason)
    {
        isIAPInProcess = false;
        var uid = PlayerPrefs.GetString(PLAYER_PREFS_KEY_LAST_PURCHASED_UID, "");
        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogException(new Exception("[IAP] cannot found last purchased uid"));
        }
        else
        {
            listener.OnPurchaseFailed(uid, reason);
        }
    }

    public void OnRewardPlayer()
    {
        isIAPInProcess = false;
        var uid = PlayerPrefs.GetString(PLAYER_PREFS_KEY_LAST_PURCHASED_UID, "");
        if (string.IsNullOrEmpty(uid))
        {
            Debug.LogException(new Exception("[IAP] cannot found last purchased uid"));
        }
        else
        {
            listener.OnRewardPlayer(uid);

#if USE_ADJUST_LOGGING
            var revenue = (double)GetPriceAsNumber(uid);
            AdjustLoggingController.instance.LogIAP(revenue, CurrencyCode);
#endif
        }
    }

    public void OnQuerySubscriptionInfo(Dictionary<string, DateTime> dic)
    {
        listener.OnQuerySubscriptionInfo(dic);
    }

    #endregion
}
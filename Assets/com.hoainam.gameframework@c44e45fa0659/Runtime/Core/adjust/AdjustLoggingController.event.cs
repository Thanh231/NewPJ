
#if USE_ADJUST_LOGGING

using AdjustSdk;

//adjust don't allow to attach additional parameters to any event.
public partial class AdjustLoggingController
{
    #region IAP
    
    public void LogIAP(double revenue, string currency)
    {
        var evtToken = GameFrameworkConfig.instance.adjustIAPEventToken;
        var evt = new AdjustEvent(evtToken);

        //adjust will convert any currency to USD/KRW internally,
        //so we just pass the original currency here.
        //pass KRW is hard because we don't have IAP price in KRW.
        evt.SetRevenue(revenue, currency);
        
        StaticUtils.LogFramework($"[Adjust] Logging IAP: revenue={revenue}, currency={currency}");
        
        Adjust.TrackEvent(evt);
    }

    #endregion

    #region ad

    //logged when a feature in game ask users to watch ad
    public void LogAd_opportunity()
    {
        StaticUtils.LogFramework("[Adjust] log ad opportunity");

        MaxSdk.TrackEvent("rewarded_ad_opportunity");
    }

    //logged when users click to watch ad
    public void LogAd_impression()
    {
        StaticUtils.LogFramework("[Adjust] log ad impression");

        MaxSdk.TrackEvent("ad_view");
    }

    //while watching ad, if users click to install game, log it
    public void LogAd_clicked()
    {
        StaticUtils.LogFramework("[Adjust] log ad click");

        MaxSdk.TrackEvent("ad_clicked");
    }

    //logged when users has finished watching ad and get rewards
    public void LogAd_rewarded(double revenue)
    {
        var adNetwork = AdController.instance.AdNetworkNameForAdjustLogging;
        var evt = new AdjustAdRevenue(adNetwork);
        
        evt.SetRevenue(revenue, "USD");
        
        StaticUtils.LogFramework($"[Adjust] log ad rewarded: revenue={revenue}, adNetwork={adNetwork}");
        
        Adjust.TrackAdRevenue(evt);
    }

    #endregion

    #region gaming
    #endregion
}

#endif
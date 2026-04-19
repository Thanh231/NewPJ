
using System;
using System.Collections.Generic;

public interface IIAPListener
{
    void OnPurchaseFailed(string uid, IAPPurchaseFailReason reason);
    
    /// <summary>
    /// in case a PID represents multiple products in game,
    /// use UID instead of PID to identify which product to reward
    /// </summary>
    /// <param name="uid">unique ID to identify product in game</param>
    void OnRewardPlayer(string uid);
    
    void OnQuerySubscriptionInfo(Dictionary<string, DateTime> dic);
}

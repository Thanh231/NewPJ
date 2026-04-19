
#if USE_BALANCY

using System.Collections.Generic;
using Balancy;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public partial class BalancyController
{
    public class MyOfferInfo
    {
        public string name;
        public int secondsLeft;
    }
    
    public UnityAction<MyOfferInfo> OnNewOfferActivated;

    private void Init_offers()
    {
        Callbacks.OnNewOfferActivated += offerInfo =>
        {
            var myInfo = new MyOfferInfo()
            {
                name = offerInfo.GameOffer.Name.Value,
                secondsLeft = offerInfo.GetSecondsLeftBeforeDeactivation()
            };
            OnNewOfferActivated?.Invoke(myInfo);
        };
    }
    
    public async UniTask<List<MyOfferInfo>> GetActiveOffers()
    {
        await UniTask.WaitUntil(() => isServerAvailable != null);
        if (!isServerAvailable.Value)
        {
            Debug.LogError("[Balancy] cannot get active offers because server is not available.");
            return null;
        }

        var resultList = new List<MyOfferInfo>();
        var rawList = Profiles.System.SmartInfo.GameOffers;
        foreach (var rawOffer in rawList)
        {
            var offerInfo = new MyOfferInfo()
            {
                name = rawOffer.GameOffer.Name.Value,
                secondsLeft = rawOffer.GetSecondsLeftBeforeDeactivation()
            };
            resultList.Add(offerInfo);
        }

        return resultList;
    }
}

#endif
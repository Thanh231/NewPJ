
#if USE_BALANCY

using System.Collections.Generic;
using Balancy;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public partial class BalancyController
{
    public class MyEventInfo
    {
        public string name;
        public int secondsLeft;
    }

    public UnityAction<MyEventInfo> OnNewEventActivated;

    private void Init_event()
    {
        Callbacks.OnNewEventActivated += eventInfo =>
        {
            if (string.IsNullOrEmpty(eventInfo.OfferInstanceId))
            {
                var myInfo = new MyEventInfo()
                {
                    name = eventInfo.GameEvent.Name.Value,
                    secondsLeft = eventInfo.GetSecondsLeftBeforeDeactivation()
                };
                OnNewEventActivated?.Invoke(myInfo);
            }
        };
    }

    public async UniTask<List<MyEventInfo>> GetActiveEvents()
    {
        await UniTask.WaitUntil(() => isServerAvailable != null);
        if (!isServerAvailable.Value)
        {
            Debug.LogError("[Balancy] cannot get active events because server is not available.");
            return null;
        }

        var resultList = new List<MyEventInfo>();
        var rawList = Profiles.System.SmartInfo.GameEvents;
        foreach (var rawEvent in rawList)
        {
            if (string.IsNullOrEmpty(rawEvent.OfferInstanceId))
            {
                var eventInfo = new MyEventInfo()
                {
                    name = rawEvent.GameEvent.Name.Value,
                    secondsLeft = rawEvent.GetSecondsLeftBeforeDeactivation()
                };
                resultList.Add(eventInfo);
            }
        }

        return resultList;
    }
}

#endif


#if USE_BALANCY

using Balancy;
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class BalancyController
{
    public async UniTask<T> GetCustomProfile<T>() where T : Balancy.Data.ParentBaseData, new()
    {
        await UniTask.WaitUntil(() => isServerAvailable != null);
        if (!isServerAvailable.Value)
        {
            Debug.LogError("[Balancy] cannot get custom profile because server is not available.");
            return null;
        }

        return Profiles.Get<T>();
    }
}

#endif
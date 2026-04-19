
#if USE_FIREBASE_CRASHLYTICS

using Cysharp.Threading.Tasks;
using Firebase.Crashlytics;

public partial class FirebaseController
{
    public async UniTask SetCrashlyticsUserId(string userId)
    {
        await UniTask.WaitUntil(() => isFirebaseAvailable);

        Crashlytics.SetUserId(userId);
    }
}

#endif
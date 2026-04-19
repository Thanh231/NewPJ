
#if USE_FIREBASE_REALTIME_DATABASE

using Cysharp.Threading.Tasks;
using Firebase.Database;
using UnityEngine;
using UnityEngine.Events;

public partial class FirebaseController
{
    public async UniTask RealtimeDatabaseSetValue(string key, string value)
    {
        var db = FirebaseDatabase.DefaultInstance;
        var reference = db.RootReference.Child(key);
        await reference.SetValueAsync(value);
    }
    
    public void RealtimeDatabaseSubscribeValueChanged(string key, UnityAction<string> onValueChanged)
    {
        var db = FirebaseDatabase.DefaultInstance;
        var reference = db.RootReference.Child(key);
        reference.ValueChanged += (_, args) =>
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(
                    $"[Firebase][Realtime Database] cannot listen change at {key}: {args.DatabaseError.Message}");
                return;
            }
            if (args.Snapshot != null && args.Snapshot.Exists)
            {
                var snapshotKey = args.Snapshot.Key;
                StaticUtils.LogFramework($"[FirebaseRealtimeDatabase] value changed at {snapshotKey}");
                
                var value = args.Snapshot.GetValue(true).ToString();
                onValueChanged?.Invoke(value);
            }
        };
    }
}

#endif
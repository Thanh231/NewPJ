
#if DATABASE_USE_FIREBASE

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;

public partial class FirebaseController : IServer_database
{
    //this operation does not require the document or even collection to already exist
    public async UniTask Database_overwriteDoc(string collectionName, string documentKey,
        Dictionary<string, object> documentValue)
    {
        var db = FirebaseFirestore.DefaultInstance;
        var doc = db.Collection(collectionName).Document(documentKey);
        await doc.SetAsync(documentValue);
    }

    //this operation require the document to already exist
    public async UniTask Database_updateFieldDoc(string collectionName, string documentKey,
        Dictionary<string, object> documentValue)
    {
        var db = FirebaseFirestore.DefaultInstance;
        var doc = db.Collection(collectionName).Document(documentKey);
        await doc.UpdateAsync(documentValue);
    }
    
    public async UniTask<bool> Database_checkDocExist(string collectionName, string documentKey)
    {
        var db = FirebaseFirestore.DefaultInstance;
        var doc = db.Collection(collectionName).Document(documentKey);
        var snapshot = await doc.GetSnapshotAsync();
        return snapshot.Exists;
    }
}

#endif
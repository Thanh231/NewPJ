#if PLAYER_DATA_USE_FIREBASE

using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Firebase.Storage;
using UnityEngine.Events;

public partial class FirebaseController : IServer_playerData
{
    #region implement IServer_playerData

    public async UniTask<string> PlayerData_getText(string userId, string fileName)
    {
        var path = $"{userId}/{fileName}";
        string result = null;
        await StorageDownloadFile(path, stream =>
        {
            if (stream != null)
            {
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
        });
        return result;
    }

    public async UniTask PlayerData_get(string userId, UnityAction<Stream> callback)
    {
        var path = $"{userId}/{PlayerModelManager.ModelFileNameOnServer}.bin";
        await StorageDownloadFile(path, callback);
    }

    public async UniTask PlayerData_set(string userId, Stream modelContent)
    {
        var path = $"{userId}/{PlayerModelManager.ModelFileNameOnServer}.bin";
        await StorageUploadFile(path, modelContent);
    }

    #endregion
    
    #region api

    private async UniTask StorageUploadFile(string filename, Stream fileContent)
    {
        var storage = FirebaseStorage.DefaultInstance;
        var fileRef = storage.RootReference.Child(filename);
        fileContent.Position = 0;
        await fileRef.PutStreamAsync(fileContent);
    }

    private async UniTask StorageDownloadFile(string filename, UnityAction<Stream> callback)
    {
        try
        {
            var storage = FirebaseStorage.DefaultInstance;
            var fileRef = storage.RootReference.Child(filename);
            var stream = await fileRef.GetStreamAsync();
            stream.Position = 0;
            callback?.Invoke(stream);
            stream.Close();
        }
        catch (Exception e)
        {
            if (IsObjectNotFoundException(e))
            {
                StaticUtils.LogErrorFramework($"[FirebaseStorage] path is not found: {filename}");
                
                callback?.Invoke(null);
                return;
            }

            StaticUtils.RethrowException(e);
        }
    }

    private async UniTask<bool> StorageFileExists(string path)
    {
        var storage = FirebaseStorage.DefaultInstance;
        var fileRef = storage.RootReference.Child(path);
        try
        {
            await fileRef.GetMetadataAsync();
            return true;
        }
        catch (Exception e)
        {
            if (IsObjectNotFoundException(e))
            {
                return false;
            }

            StaticUtils.RethrowException(e);
            return false;
        }
    }

    private async UniTask<bool> StorageFolderExists(string path)
    {
        await UniTask.CompletedTask;
        
        //firebase has an API call "List", we can use this to check a folder exists,
        //this API is available on all platforms, but UNITY 
        throw new Exception("not supported");
    }

    private bool IsObjectNotFoundException(Exception e)
    {
        if (e is StorageException storageException)
        {
            if (storageException.ErrorCode == StorageException.ErrorObjectNotFound)
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}

#endif
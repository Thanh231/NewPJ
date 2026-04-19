
using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ServerController : SingletonMonoBehaviour<ServerController>
{
    private IServer_cloudCode implCloudCode;
    private IServer_database implDatabase;
    private IServer_gameContent implGameContent;
    private IServer_playerData implPlayerData;
    private IServer_authenticate implAuthenticate;
    
    private AWSController _awsInstanceInEditor;
    private AWSController awsInstanceInEditor
    {
        get
        {
            if (_awsInstanceInEditor == null)
            {
                _awsInstanceInEditor = new AWSController();
            }
            return _awsInstanceInEditor;
        }
    }

    public ServerEnvironment serverEnvironment { get; set; }

    public void Init(ServerEnvironment serverEnvironment, bool usedInEditorTool = false)
    {
        this.serverEnvironment = serverEnvironment;
        
#if CLOUD_CODE_USE_AWS
        implCloudCode = AWSController.instance;
#endif

#if DATABASE_USE_FIREBASE
        implDatabase = FirebaseController.instance;
#endif

#if GAME_CONTENT_USE_AWS
        if (usedInEditorTool)
        {
            implGameContent = awsInstanceInEditor;
        }
        else
        {
            implGameContent = AWSController.instance;
        }
#endif

#if PLAYER_DATA_USE_FIREBASE
        implPlayerData = FirebaseController.instance;
#endif
        
#if AUTHENTICATE_USE_FIREBASE
        implAuthenticate = FirebaseController.instance;
#endif
    }

    #region auhentication

    public async UniTask<ServerSignInResult> SignIn(SocialSignInProviderType providerType, bool linkAccount)
    {
        return await implAuthenticate.SignIn(providerType, linkAccount);
    }

    public async UniTask SignOut(SocialSignInProviderType providerType)
    {
        await implAuthenticate.SignOut(providerType);
    }

    #endregion
    
    #region game content

    public async UniTask<string> GameContent_get(string key)
    {
        return await implGameContent.GameContent_get(key);
    }
    
    public async UniTask GameContent_download(string key, string path)
    {
        await implGameContent.GameContent_download(key, path);
    }

    public async UniTask GameContent_set(string key, string value)
    {
        await implGameContent.GameContent_set(key, value);
    }

    public async UniTask GameContent_set(string key, byte[] value)
    {
        await implGameContent.GameContent_set(key, value);
    }

    public async UniTask GameContent_applySet()
    {
        await implGameContent.GameContent_applySet();
    }

    #endregion
    
    #region player data

    public async UniTask<string> PlayerData_getText(string userId, string filename)
    {
        return await implPlayerData.PlayerData_getText(userId, filename);
    }

    public async UniTask PlayerData_get(string userId, UnityAction<Stream> callback)
    {
        await implPlayerData.PlayerData_get(userId, callback);
    }

    public async UniTask PlayerData_set(string userId, Stream modelContent)
    {
        await implPlayerData.PlayerData_set(userId, modelContent);
    }

    #endregion

    #region database

    public async UniTask Database_overwriteDoc(string collectionName, string documentKey, Dictionary<string, object> documentValue)
    {
        await implDatabase.Database_overwriteDoc(collectionName, documentKey, documentValue);
    }

    public async UniTask Database_updateFieldDoc(string collectionName, string documentKey, Dictionary<string, object> documentValue)
    {
        await implDatabase.Database_updateFieldDoc(collectionName, documentKey, documentValue);
    }

    public async UniTask<bool> Database_checkDocExist(string collectionName, string documentKey)
    {
        return await implDatabase.Database_checkDocExist(collectionName, documentKey);
    }

    #endregion
    
    #region cloud code

    public async UniTask<bool> ValidateGooglePlayReceipt(string pid, string purchaseToken, bool isSubscriptionProduct,
        string orderNumber, decimal price)
    {
        Debug.Log(
            $"[CloudCode] ValidateGooglePlayReceipt called. pid={pid}, purchaseToken={purchaseToken}, isSubscriptionProduct={isSubscriptionProduct}");
        return await implCloudCode.ValidateGooglePlayReceipt(pid, purchaseToken, isSubscriptionProduct, orderNumber,
            price);
    }

    public async UniTask<bool> ValidateAppStoreReceipt(string payload, string transactionId, string productId,
        decimal price)
    {
        Debug.Log($"[CloudCode] ValidateAppStoreReceipt called. payload={payload}");
        return await implCloudCode.ValidateAppStoreReceipt(payload, transactionId, productId, price);
    }

    public async UniTask<DateTime> GetUTCNow()
    {
        return await implCloudCode.GetUTCNow();
    }

    #endregion
}
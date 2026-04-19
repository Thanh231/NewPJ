
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class ServerUsersManager
{
    private const string dbCollectionName = "users_properties";

    private async UniTask UpdatePropertiesAfterLogin()
    {
        var docKey = userUID.Value;
        var playerExists = await ServerController.instance.Database_checkDocExist(dbCollectionName, docKey);
        if (playerExists)
        {
            var dic = new Dictionary<string, object>()
            {
                { "last_login", UtcTime.instance.UtcNow }
            };
            await ServerController.instance.Database_updateFieldDoc(dbCollectionName, docKey, dic);
        }
        else
        {
            var dic = new Dictionary<string, object>()
            {
                { "first_login", UtcTime.instance.UtcNow },
                { "country", Application.systemLanguage.ToString() },
                { "last_login", UtcTime.instance.UtcNow },
                { "platform", GetPlatform() }
            };
            await ServerController.instance.Database_overwriteDoc(dbCollectionName, docKey, dic);
        }
    }

    private string GetPlatform()
    {
#if UNITY_EDITOR
        return "Editor";
#endif

#if UNITY_STANDALONE
        return "Standalone";
#endif

#if UNITY_IOS
        return "iOS";
#endif

#if UNITY_ANDROID

#if SAMSUNG_STORE
        return "Samsung";
#else
        return "Android";
#endif

#endif
        return "Unknown";
    }
}
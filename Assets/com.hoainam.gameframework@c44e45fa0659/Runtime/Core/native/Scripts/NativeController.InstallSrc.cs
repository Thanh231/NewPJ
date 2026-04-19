
using System;
using System.Collections.Generic;
using System.Net;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public partial class NativeController
{
    // when using meta ad campaign, don't need to configure URL or referrer parameter
    // meta ad will generate the URL and send to google play
    // when install app from Google play, must use personal email (@gmail.com), not company email

    #region core

    /// <summary>
    /// get install source
    /// </summary>
    /// <returns>ad campaign name</returns>
    public async UniTask<string> GetInstallSource()
    {
        try
        {
            var rawInstallSource = await impl.GetInstallSource();
            if (!string.IsNullOrEmpty(rawInstallSource))
            {
                Debug.Log($"[Install Source] {rawInstallSource}");
                
                var dict = ParseRawInstallSource(rawInstallSource);
                var src = dict["utm_source"];
                if (src.Equals("apps.facebook.com"))
                {
                    return ParseFacebookInstallSource(dict);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                Debug.Log("[Install Source] get install source return nothing.");
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return null;
        }
    }
    
    public void NativeCallback_getInstallSourceSuccess(string data)
    {
        impl.NativeCallback_getInstallSourceSuccess(data);
    }

    public void NativeCallback_getInstallSourceFail(string data)
    {
        impl.NativeCallback_getInstallSourceFail(data);
    }
    
    private static Dictionary<string, string> ParseRawInstallSource(string rawInstallSource)
    {
        var dict = new Dictionary<string, string>();
        var l = rawInstallSource.Split('&');
        foreach (var str in l)
        {
            var ll = str.Split('=');
            dict.Add(ll[0], ll[1]);
        }

        return dict;
    }
    
    #endregion

    #region facebook ad install source

    public class FacebookEncryptedReferrerData
    {
        public class Payload
        {
            public string data;
            public string nonce;
        }

        public Payload source;
    }
    
    public class FacebookDecryptedReferrerData
    {
        public string campaign_name;
    }
    
    private string ParseFacebookInstallSource(Dictionary<string, string> dict)
    {
        var decodedData = WebUtility.UrlDecode(dict["utm_content"]);
        var encryptedReferrerData = JsonConvert.DeserializeObject<FacebookEncryptedReferrerData>(decodedData);
        
        var decryptedReferrerData =
            impl.DecryptFacebookReferrerData(encryptedReferrerData.source.data, encryptedReferrerData.source.nonce);
        if (!string.IsNullOrEmpty(decryptedReferrerData))
        {
            Debug.Log($"[Facebook Install Source] {decryptedReferrerData}]");
            var obj = JsonConvert.DeserializeObject<FacebookDecryptedReferrerData>(decryptedReferrerData);
            return obj.campaign_name;
        }
        else
        {
            return null;
        }
    }

    #endregion
}


using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public static partial class StaticUtils
{
    public class SlackResponse
    {
        public bool ok;
        public string error;
    }

    //if you got InvalidOperationException: Insecure connection not allowed,
    //Go to player settings -> other settings -> configuration -> allow downloads over HTTP -> always allowed
    
    #region get request

    public static async UniTask<HttpGetResult> GetImageHttpRequest(string url)
    {
        var req = UnityWebRequestTexture.GetTexture(url);
        await req.SendWebRequest();

        var result = new HttpGetResult() { url = url };
        var texture = DownloadHandlerTexture.GetContent(req);
        result.resultAsBinary = texture.EncodeToPNG();

        req.Dispose();
        return result;
    }

    public static async UniTask<HttpGetResult> GetHttpRequest(string url, bool returnText,
        Dictionary<string, string> parameters = null)
    {
        try
        {
            url = BuildUrlWithParams(url, parameters);
            var req = UnityWebRequest.Get(url);
            var op = await req.SendWebRequest();

            var result = new HttpGetResult() { url = url };
            if (returnText)
            {
                result.resultAsText = op.downloadHandler.text;
            }
            else
            {
                result.resultAsBinary = op.downloadHandler.data;
            }

            req.Dispose();
            return result;
        }
        catch (Exception e)
        {
            Debug.LogError($"get request {url} has error below:");
            RethrowException(e);

            return null;
        }
    }

    #endregion
    
    #region post request

    public static async UniTask<string> PostHttpRequest(string url, Dictionary<string, string> form)
    {
        var req = UnityWebRequest.Post(url, form);
        try
        {
            var op = await req.SendWebRequest();
            return op.downloadHandler.text;
        }
        catch (UnityWebRequestException e)
        {
            throw new HttpPostException(e);
        }
        finally
        {
            req.Dispose();
        }
    }

    public static async UniTask<string> PostHttpRequest(string url, string json)
    {
        var req = UnityWebRequest.Put(url, json);
        req.SetRequestHeader("Content-Type", "application/json");
        try
        {
            var op = await req.SendWebRequest();
            return op.downloadHandler.text;
        }
        catch (UnityWebRequestException e)
        {
            throw new HttpPostException(e);
        }
        finally
        {
            req.Dispose();
        }
    }

    public static async UniTask<string> PostUploadHttpRequest(string url, byte[] data,
        Dictionary<string, string> headers, bool overwrite)
    {
        var req = new UnityWebRequest(url, overwrite ? "PATCH" : "POST");
        req.uploadHandler = new UploadHandlerRaw(data);
        req.downloadHandler = new DownloadHandlerBuffer();
        foreach (var i in headers)
        {
            req.SetRequestHeader(i.Key, i.Value);
        }

        try
        {
            var op = await req.SendWebRequest();
            return op.downloadHandler.text;
        }
        catch (UnityWebRequestException e)
        {
            throw new HttpPostException(e);
        }
        finally
        {
            req.Dispose();
        }
    }

    #endregion

    #region service requests

    //how to get botToken: create app at api.slack.com/apps, then install app to workspace to get the token.
    //in order for bot can send a message into a channel, need to add bot to the channel first.
    public static void SlackSendMessageRequest(string botToken, string channelName, string message)
    {
        //build body
        return;
        
        channelName = channelName.StartsWith('#') ? channelName : $"#{channelName}";
        var bodyJson = JsonConvert.SerializeObject(new { channel = channelName, text = message });
        var bodyData = Encoding.UTF8.GetBytes(bodyJson);

        //build request
        var request = (HttpWebRequest)WebRequest.Create("https://slack.com/api/chat.postMessage");
        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers["Authorization"] = $"Bearer {botToken}";
        request.ContentLength = bodyData.Length;

        using (var stream = request.GetRequestStream())
        {
            stream.Write(bodyData, 0, bodyData.Length);
        }

        //send request
        using (var response = (HttpWebResponse)request.GetResponse())
        {
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var responseText = reader.ReadToEnd();
                var slackRes = JsonConvert.DeserializeObject<SlackResponse>(responseText);
                if (!slackRes.ok)
                {
                    throw new Exception($"send message to slack channel {channelName} fail with msg: {slackRes.error}");
                }
            }
        }
    }


    #endregion

    #region utils

    private static string BuildUrlWithParams(string url, Dictionary<string, string> form)
    {
        if (form == null || form.Count == 0)
        {
            return url;
        }

        var sb = new StringBuilder(url);
        var firstParam = true;
        foreach (var i in form)
        {
            var separator = firstParam ? "?" : "&";
            sb.Append(separator).Append(i.Key).Append('=').Append(EncodeParameterValue(i.Value));
            firstParam = false;
        }
        return sb.ToString();
    }

    private static string EncodeParameterValue(string val)
    {
        val = val.Replace("%", "%25");
        val = val.Replace("=", "%3D");
        val = val.Replace("+", "%2B");
        val = val.Replace("&", "%26");
        val = val.Replace("?", "%3F");
        
        return val;
    }

    #endregion
}

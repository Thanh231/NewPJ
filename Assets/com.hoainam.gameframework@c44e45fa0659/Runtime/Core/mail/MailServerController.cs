
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

//admin can send mail to users through 3 ways:
//1. universal mail: sent to all users.
//2. personal mail: sent to specific user
//3. segment mail: sent to users in specific segment

public class MailServerController : SingletonMonoBehaviour<MailServerController>
{
    private List<MailInfo> listMail = new();

    #region fetch mail
    
    public async UniTask FetchMail()
    {
        var universalMails = await FetchUniversalMail();
        var personalMails = await FetchPersonalMail();
        
        listMail.AddRange(universalMails);
        listMail.AddRange(personalMails);
    }
    
    private async UniTask<List<MailInfo>> FetchUniversalMail()
    {
        var environment = ServerController.instance.serverEnvironment;
        try
        {
            var json = await ServerController.instance.GameContent_get($"{environment}/universal_mail.json");
            return JsonConvert.DeserializeObject<List<MailInfo>>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"--------------------server {environment} don't have universal mail config");
            Debug.LogException(e);
            Debug.LogError("-----------------------------------------------------");
            return new List<MailInfo>();
        }
    }

    private async UniTask<List<MailInfo>> FetchPersonalMail()
    {
        if (!ServerUsersManager.instance.IsLoggedIn)
        {
            StaticUtils.LogFramework("[Mail] user not logged in, skip fetch personal mail");
            return new List<MailInfo>();
        }

        var userId = ServerUsersManager.instance.userUID.Value;
        StaticUtils.LogFramework($"[Mail] fetch personal mail for user {userId}");
        var json = await ServerController.instance.PlayerData_getText(userId, "personal_mail.json");
        if (string.IsNullOrEmpty(json))
        {
            StaticUtils.LogFramework("[Mail] no personal mail found");
            return new List<MailInfo>();
        }
        else
        {
            StaticUtils.LogFramework($"[Mail] personal mail json: {json}");
            return JsonConvert.DeserializeObject<List<MailInfo>>(json);
        }
    }
    
    #endregion

    #region get mail

    public List<MailInfo> GetListMail(SystemLanguage language)
    {
        var l = new List<MailInfo>();
        var now = UtcTime.instance.UtcNow;
        foreach (var mailInfo in listMail)
        {
            if (mailInfo.timeExpired > now)
            {
                mailInfo.SetLanguage(language);
                l.Add(mailInfo);
            }
        }

        return l;
    }

    #endregion
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class MailLocalizedContent
{
    public SystemLanguage language;
    public string title;
    public string content;
}

public class MailRewardInfo
{
    public string rewardType;
    public int amount;
}

public class MailInfo
{
    public string mailId;
    public List<MailLocalizedContent> localizedContents;
    public DateTime timeSent;
    public DateTime timeExpired;
    public List<MailRewardInfo> rewards;

    public string MailTitle { get; set; }
    public string MailContent { get; set; }
    public bool HasRewards => rewards != null && rewards.Count > 0;
    
    public void SetLanguage(SystemLanguage language)
    {
        var localizedContent = localizedContents.Find(x => x.language == language);
        if (localizedContent != null)
        {
            MailTitle = localizedContent.title;
            MailContent = localizedContent.content;
        }
        else
        {
            // Fallback to English if the specified language is not found
            localizedContent = localizedContents.Find(x => x.language == SystemLanguage.English);
            if (localizedContent != null)
            {
                MailTitle = localizedContent.title;
                MailContent = localizedContent.content;
            }
        }
    }
}
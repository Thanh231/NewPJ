
using System.Collections.Generic;
using UnityEngine;

public class LanguageInfoItem
{
    public LanguageGroupName groupName;
    public string androidIsoCode;
    public string iosIsoCode;
    public string localLanguageName;
    
    public LanguageInfoItem(List<string> l)
    {
        groupName = StaticUtils.StringToEnum<LanguageGroupName>(l[1]);
        androidIsoCode = l[2];
        iosIsoCode = l[3];
        localLanguageName = l[4];
    }
}

public class LanguageInfoConfig
{
    public Dictionary<SystemLanguage, LanguageInfoItem> languageInfoItems = new();

    public LanguageInfoConfig()
    {
        var txt = Resources.Load<TextAsset>("language-info.csv").text;
        using (var stream = new StringStream(txt))
        {
            var csv = new CsvReader(stream.GetReader());
            var l = csv.ReadRecord();
            while ((l = csv.ReadRecord()) != null)
            {
                var key = StaticUtils.StringToEnum<SystemLanguage>(l[0]);
                languageInfoItems.Add(key, new LanguageInfoItem(l));
            }
        }
    }

    public LanguageInfoItem GetLanguageInfoItem(SystemLanguage l)
    {
        return languageInfoItems[l];
    }

    public LanguageInfoItem GetLanguageInfoItem(string s)
    {
        return GetLanguageInfoItem(StaticUtils.StringToEnum<SystemLanguage>(s));
    }
}

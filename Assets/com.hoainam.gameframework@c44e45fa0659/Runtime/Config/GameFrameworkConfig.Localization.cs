
using System;
using System.Collections.Generic;
using UnityEngine;

public enum LanguageGroupName
{
    undefined = -1,
    
    Latin,
    Korean,
    Japanese,
    Chinese,
    Hindi,
    Thai,
    
    count,
}

[Serializable]
public class LanguageGroupInfo
{
    public LanguageGroupName languageGroupName;
    public Font font;
    public int padding;
    public int atlasSize;
}

public partial class GameFrameworkConfig
{
    [Header("localization")]
    public List<LanguageGroupInfo> languageGroupCfg;
    public List<string> locFileNames;

    public LanguageGroupInfo GetLanguageGroupInfo(LanguageGroupName groupName)
    {
        return languageGroupCfg.Find(x => x.languageGroupName == groupName);
    }
}

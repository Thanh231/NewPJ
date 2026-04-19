
using System.Collections.Generic;
using UnityEngine;

public static class IosLocalizeProcessor
{
    public static void Process(Dictionary<SystemLanguage, string> dicAppName,
        Dictionary<SystemLanguage, string> dicTrackingDesc)
    {
        var template = StaticUtils.GetResourceFileText("IosLocalizeTemplate");
        var keys = dicAppName.Keys;
        var languageInfo = new LanguageInfoConfig();
        
        foreach (var i in keys)
        {
            var text = template.Replace("{appName}", dicAppName[i]);
            text = text.Replace("{userTrackingUsageDescription}", dicTrackingDesc[i]);
            var folderName = $"{languageInfo.GetLanguageInfoItem(i).iosIsoCode}.lproj";
            var path = $"_game/localized-system-data/IosLocalization/{folderName}/InfoPlist.strings";
            StaticUtils.WriteTextFile(path, text);
        }
    }
}

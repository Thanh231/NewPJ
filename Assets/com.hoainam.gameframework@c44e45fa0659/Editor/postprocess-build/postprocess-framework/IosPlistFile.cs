
using System.IO;
using UnityEditor.iOS.Xcode;

public class IosPlistFile
{
    #region core

    private string plistPath;
    private PlistDocument plist;

    public IosPlistFile(string buildPath)
    {
        plistPath = $"{buildPath}/Info.plist";
        plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));
    }
    
    public void Save()
    {
        File.WriteAllText(plistPath, plist.WriteToString());
    }

    #endregion

    #region modify plist

    public void SetString(string key, string val)
    {
        plist.root.SetString(key, val);
    }
    
    public void AddUrlScheme(string urlScheme)
    {
        var rootDict = plist.root;

        // Get or create CFBundleURLTypes array
        PlistElementArray urlTypesArray;
        if (rootDict.values.ContainsKey("CFBundleURLTypes"))
        {
            urlTypesArray = rootDict.values["CFBundleURLTypes"].AsArray();
        }
        else
        {
            urlTypesArray = rootDict.CreateArray("CFBundleURLTypes");
        }

        // Check if URL scheme already exists
        bool urlSchemeExists = false;
        for (int i = 0; i < urlTypesArray.values.Count; i++)
        {
            var urlTypeDict = urlTypesArray.values[i].AsDict();
            if (urlTypeDict.values.ContainsKey("CFBundleURLSchemes"))
            {
                var schemesArray = urlTypeDict.values["CFBundleURLSchemes"].AsArray();
                for (int j = 0; j < schemesArray.values.Count; j++)
                {
                    if (schemesArray.values[j].AsString() == urlScheme)
                    {
                        urlSchemeExists = true;
                        break;
                    }
                }
            }
            if (urlSchemeExists) break;
        }

        // If not exists, add new URL scheme
        if (!urlSchemeExists)
        {
            var newUrlTypeDict = urlTypesArray.AddDict();
            var schemesArray = newUrlTypeDict.CreateArray("CFBundleURLSchemes");
            schemesArray.AddString(urlScheme);
        }
    }

    #endregion
}
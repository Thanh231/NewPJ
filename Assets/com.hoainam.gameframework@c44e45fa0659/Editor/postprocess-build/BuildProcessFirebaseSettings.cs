
//when firebase config file have more than 1 app,
//firebase runtime doesn't pick up app base on package name.

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildProcessFirebaseSettings : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    #region firebase settings class

    public class FirebaseSettings
    {
        public string appId;
        public string androidClientId;
    }

    public class GoogleServicesJson
    {
        public class Client
        {
            public class ClientInfo
            {
                public class AndroidClientInfo
                {
                    public string package_name;
                }

                public string mobilesdk_app_id;
                public AndroidClientInfo android_client_info;
            }

            public class OAuthClient
            {
                public string client_id;
                public int client_type;
            }

            public ClientInfo client_info;
            public List<OAuthClient> oauth_client;
        }

        public List<Client> client;
    }

    #endregion

    #region core

    private static FirebaseSettings cacheFirebaseSettings = null;
    private const string GoogleServicesXMLPath = "Plugins/Android/FirebaseApp.androidlib/res/values/google-services.xml";
    
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.Android)
        {
            return;
        }
        
        if (!StaticUtils.CheckFileExist(GoogleServicesXMLPath))
        {
            return;
        }
        
        cacheFirebaseSettings = GetFirebaseSettingsFromXML();
        var settings = GetFirebaseSettingsFromJson();
        SetFirebaseSettings(settings);
    }
    
    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.Android)
        {
            return;
        }

        if (cacheFirebaseSettings == null)
        {
            return;
        }
        
        SetFirebaseSettings(cacheFirebaseSettings);
    }

    #endregion

    #region utils

    private static FirebaseSettings GetFirebaseSettingsFromXML()
    {
        var path = Path.Combine(Application.dataPath, GoogleServicesXMLPath);
        var xml = new XmlFile(path);

        var androidClientIdTag = xml.GetChildElementWithAttribute(xml.Root, "string", "name", "default_android_client_id");
        var appIdTag = xml.GetChildElementWithAttribute(xml.Root, "string", "name", "google_app_id");

        return new FirebaseSettings()
        {
            androidClientId = androidClientIdTag.InnerText,
            appId = appIdTag.InnerText,
        };
    }

    private static FirebaseSettings GetFirebaseSettingsFromJson()
    {
        var json = StaticUtils.ReadTextFile("google-services.json");
        var jsonObj = JsonConvert.DeserializeObject<GoogleServicesJson>(json);

        foreach (var i in jsonObj.client)
        {
            if (i.client_info.android_client_info.package_name.Equals(Application.identifier))
            {
                foreach (var j in i.oauth_client)
                {
                    if (j.client_type == 1)
                    {
                        return new FirebaseSettings()
                        {
                            androidClientId = j.client_id,
                            appId = i.client_info.mobilesdk_app_id,
                        };
                    }
                }
            }
        }
        return null;
    }

    private static void SetFirebaseSettings(FirebaseSettings firebaseSettings)
    {
        var path = Path.Combine(Application.dataPath, GoogleServicesXMLPath);
        var xml = new XmlFile(path);

        var androidClientIdTag = xml.GetChildElementWithAttribute(xml.Root, "string", "name", "default_android_client_id");
        var appIdTag = xml.GetChildElementWithAttribute(xml.Root, "string", "name", "google_app_id");

        androidClientIdTag.InnerText = firebaseSettings.androidClientId;
        appIdTag.InnerText = firebaseSettings.appId;

        xml.Save();
    }
    
    #endregion
}

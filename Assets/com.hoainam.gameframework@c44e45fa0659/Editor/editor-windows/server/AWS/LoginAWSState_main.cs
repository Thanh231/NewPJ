#if USE_SERVER_AWS

using Amazon.Runtime.CredentialManagement;
using UnityEditor;
using UnityEngine;

public class LoginAWSState_main : EditorWindowState
{
    private string accessKey;
    private string secretKey;
    
    public override void OnDraw()
    {
        accessKey = EditorGUILayout.TextField("Access Key:", accessKey);
        secretKey = EditorGUILayout.TextField("Secret Key:", secretKey);

        if (GUILayout.Button("Login"))
        {
            OnBtnClicked_login();
        }
    }

    private void OnBtnClicked_login()
    {
        var options = new CredentialProfileOptions()
        {
            AccessKey = accessKey,
            SecretKey = secretKey,
        };
        var profile = new CredentialProfile("default", options);
        var file = new SharedCredentialsFile();
        file.RegisterProfile(profile);
        
        StaticUtilsEditor.DisplayDialog("login success");
    }
}

#endif
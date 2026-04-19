
using System;
using UnityEngine;

public class BuildPlayerState_releaseType : EditorWindowState
{
    public override void OnDraw()
    {
        if (GUILayout.Button("this build is for EOD build"))
        {
            OnClicked_buildEOD();
        }

        if (GUILayout.Button("this build is only for myself to test"))
        {
            OnClicked_buildMyself();
        }
    }

    private void OnClicked_buildEOD()
    {
        try
        {
            // var token = GameFrameworkConfig.instance.slackBotTokenForBuildNote;
            // var channel = GameFrameworkConfig.instance.slackChannelNameForBuildNote;
            // var msg = StaticUtils.ReadTextFile($"{StaticUtils.GetFrameworkPath()}/Editor/release-version-suite/build-player-tool/build_note_template.txt", true);
            // StaticUtils.SlackSendMessageRequest(token, channel, msg);

            BuildPlayerWindow.SetSentBuildNote();

            FSM.SwitchState(new BuildPlayerState_version());
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            StaticUtilsEditor.DisplayDialog("build fail, see console for detail");
        }
    }

    private void OnClicked_buildMyself()
    {
        BuildPlayerWindow.GoToBuildState(this);
    }
}
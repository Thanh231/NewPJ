
#if USE_BALANCY

using System;
using Balancy;
using UnityEngine;

public partial class BalancyController : SingletonMonoBehaviour<BalancyController>
{
    private bool? isServerAvailable;

    public void Init()
    {
        if (isServerAvailable != null)
        {
            return;
        }

        //mandatory callbacks
        Callbacks.OnDataUpdated += status =>
        {
            if (status.IsCloudSynced)
            {
                var info = API.GetStatus();
                isServerAvailable = !string.IsNullOrEmpty(info.BranchName);
                if (isServerAvailable.Value)
                {
                    var playerId = Profiles.System.GeneralInfo.ProfileId;
                    Debug.Log($"[Balancy] connected to branch: {info.BranchName}, player id: {playerId}");
                }
                else
                {
                    Debug.LogError("[Balancy] failed to connect to server. check console for details.");
                }
            }
        };
        
        var environment = GetBalancyEnvironment();
        Main.Init(new AppConfig()
        {
            ApiGameId = GameFrameworkConfig.instance.balancyApiGameId,
            PublicKey = GameFrameworkConfig.instance.balancyPublicKey,
            Environment = environment,
            BranchName = GetBranchName(environment)
        });

        Init_event();
        Init_offers();
    }

    #region utils

    private Constants.Environment GetBalancyEnvironment()
    {
        var serverEnvironment = ServerController.instance.serverEnvironment;
        return serverEnvironment switch
        {
            ServerEnvironment.Dev => Constants.Environment.Development,
            ServerEnvironment.InternalQA => Constants.Environment.Stage,
            ServerEnvironment.TemporaryQA => Constants.Environment.Stage,
            ServerEnvironment.QA => Constants.Environment.Stage,
            ServerEnvironment.Live => Constants.Environment.Production,
            _ => throw new Exception("Unknown server environment.")
        };
    }

    private string GetBranchName(Constants.Environment environment)
    {
        return environment switch
        {
            Constants.Environment.Development => GameFrameworkConfig.instance.balancyDevelopmentBranchName,
            Constants.Environment.Stage => GameFrameworkConfig.instance.balancyStageBranchName,
            Constants.Environment.Production => GameFrameworkConfig.instance.balancyProductionBranchName,
            _ => throw new Exception("Unknown balancy environment.")
        };
    }

    #endregion
}

#endif
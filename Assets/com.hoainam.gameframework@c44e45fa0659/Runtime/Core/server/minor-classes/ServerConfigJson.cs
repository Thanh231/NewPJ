
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class ServerConfigJson
{
    #region other classes

    public class TesterInfo
    {
        public string deviceId;
        public string testerName;
    }

    #endregion
    
    #region core
    
    public string gameConfigVersion;
    public string gameClientVersion;
    public string addressableVersion;
    public List<TesterInfo> listTesters = new();
    public bool isMandatoryUpdate;

    private ServerController serverController;

    public ServerConfigJson()
    {
    }
    
    public ServerConfigJson(ServerController serverController)
    {
        this.serverController = serverController;
    }

    #endregion
    
    #region remote

    public async UniTask Download()
    {
        var json = await serverController.GameContent_get(
            $"{serverController.serverEnvironment}/server_config.json");
        if (!string.IsNullOrEmpty(json))
        {
            JsonConvert.PopulateObject(json, this);
        }
    }

    public async UniTask Upload()
    {
        var json = StaticUtils.JsonSerializeToFriendlyText(this);
        await serverController.GameContent_set($"{serverController.serverEnvironment}/server_config.json", json);
    }

    #endregion

    #region local

    private string localFilePath => $"{PlayerModelManager.GetModelFolderPath()}/server_config.json";

    public void Save()
    {
        var json = StaticUtils.JsonSerializeToFriendlyText(this);
        StaticUtils.WriteTextFile(localFilePath, json);
    }

    public void Load()
    {
        if (StaticUtils.CheckFileExist(localFilePath))
        {
            var json = StaticUtils.ReadTextFile(localFilePath);
            JsonConvert.PopulateObject(json, this);
        }
    }

    #endregion

    #region public utils

    public bool IsTester(string deviceId)
    {
        return listTesters != null && listTesters.Any(x => x.deviceId == deviceId);
    }

    public bool HasNewVersion()
    {
        if (string.IsNullOrEmpty(gameClientVersion))
        {
            return false;
        }

        return StaticUtils.CompareVersion(Application.version, gameClientVersion) < 0;
    }
    
    public void ChangeGameConfigVersion()
    {
        gameConfigVersion = CreateNewVersionName();
    }

    public void ChangeAddressableVersion()
    {
        addressableVersion = CreateNewVersionName();
    }

    private string CreateNewVersionName()
    {
        return StaticUtils.DateTimeToString(DateTime.UtcNow);
    }

    #endregion
}

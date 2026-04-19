
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//reader/writer for excel-based configs
public class ConfigReadWriteManager
{
    #region data member

    public List<IBaseConfig> listConfigs;
    public Dictionary<Type, IBaseConfig> dicConfigs = new();

    public ConfigReadWriteManager(List<IBaseConfig> listConfigs)
    {
        this.listConfigs = listConfigs;

        foreach (var cfg in listConfigs)
        {
            var cfgType = cfg.GetType();
            if (dicConfigs.ContainsKey(cfgType))
            {
                throw new Exception($"there are more than 1 {cfgType.Name} in listConfigs");
            }
            else
            {
                dicConfigs.Add(cfgType, cfg);
            }
        }
    }

    #endregion

    #region read

    public void ReadConfig_editor()
    {
        foreach (var cfg in listConfigs)
        {
            var cfgPath = $"../GameConfig/{cfg.GetType().Name}.csv";
            StaticUtils.OpenFileForRead(cfgPath, stream =>
            {
                LoadCfgFromStream(stream, cfg);
            });
        }
    }

    public async UniTask ReadConfig_standalone()
    {
        foreach (var cfg in listConfigs)
        {
            var cfgName = cfg.GetType().Name;
            var cfgTxt = await GetCfgOnStandalone(cfgName);

            using (var stream = new StringStream(cfgTxt))
            {
                LoadCfgFromStream(stream.GetReader(), cfg);
            }
        }
    }

    public async UniTask ReadConfig_mobile()
    {
        var cfgBin = await GetCfgOnMobile();

        using (var stream = new MemoryStream(cfgBin))
        {
            var reader = new BinaryReader(stream);
            var filestream = new FileStream_binaryReader(reader);
            foreach (var cfg in listConfigs)
            {
                var numItems = 0;
                try
                {
                    filestream.ReadOrWriteInt(ref numItems);
                    cfg.Read(filestream, numItems);
                }
                catch (Exception e)
                {
                    var cfgName = cfg.GetType().Name;
                    Debug.LogError($"[Config] read config {cfgName} failed");
                    StaticUtils.RethrowException(e);
                }
            }
        }
    }

    private async UniTask<string> GetCfgOnStandalone(string cfgName)
    {
        var cfgPathFile = $"../../GameConfig/{cfgName}.csv";
        var cfgPathStream = $"GameConfig/{cfgName}.csv";
        var readCfgFromFile = false;
        
#if USE_SERVER_GAME_CONFIG
        readCfgFromFile = StaticUtils.CheckFileExist(cfgPathFile);
#endif

        if (readCfgFromFile)
        {
            return StaticUtils.ReadTextFile(cfgPathFile);
        }
        else
        {
            return await StaticUtils.GetStreamingFileText(cfgPathStream);
        }
    }

    private async UniTask<byte[]> GetCfgOnMobile()
    {
        const string cfgPath = "GameConfig/all_config.bin";
        var readCfgFromFile = false;
        
#if USE_SERVER_GAME_CONFIG
        readCfgFromFile = StaticUtils.CheckFileExist(cfgPath);
#endif

        if (readCfgFromFile)
        {
            return StaticUtils.ReadBinaryFile(cfgPath);
        }
        else
        {
            return await StaticUtils.GetStreamingFileBinary(cfgPath);
        }
    }
    
    private void LoadCfgFromStream(StreamReader reader, IBaseConfig cfg)
    {
        try
        {
            var filestream = new FileStream_csvReader(reader);
            cfg.Read(filestream, filestream.NumItems);
        }
        catch (Exception e)
        {
            var cfgName = cfg.GetType().Name;
            Debug.LogError($"[Config] read config {cfgName} failed");
            StaticUtils.RethrowException(e);
        }
    }

    #endregion

    #region write

    public void WriteConfig_standalone()
    {
        var projPath = StaticUtils.GetProjectPath();
        var srcPath = $"{projPath}/GameConfig";
        var destPath = $"{projPath}/Assets/StreamingAssets/GameConfig";
        StaticUtils.CopyFolder(srcPath, destPath, isAbsolutePath: true);
    }

    public void WriteConfig_mobile()
    {
        var projPath = StaticUtils.GetProjectPath();
        var path = $"{projPath}/Assets/StreamingAssets/GameConfig/all_config.bin";
        StaticUtils.OpenFileForWrite(path, WriteConfigBinary, isAbsolutePath: true);
    }

    public void WriteConfigBinary(BinaryWriter writer)
    {
        var streamWriter = new FileStream_binaryWriter(writer);
        foreach (var cfg in listConfigs)
        {
            cfg.Write(streamWriter);
        }
    }

    #endregion
}
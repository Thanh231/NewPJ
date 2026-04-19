using System.IO;
using Cysharp.Threading.Tasks;

//some level-based games need custom config in client code, so this is abstract class
//for reading/writing it
public abstract class ExtraConfigReadWriteManager
{
    public abstract string configFilename {get;}

    #region read

    public void ReadConfig_editor()
    {
        var path = $"../GameConfig/{configFilename}.txt";
        var text = StaticUtils.ReadTextFile(path);
        OnReadConfig_text(text);
    }

    public async UniTask ReadConfig_standalone()
    {
        var text = await GetConfigContent_standalone();
        OnReadConfig_text(text);
    }

    public async UniTask ReadConfig_mobile()
    {
        var data = await GetConfigContent_mobile();
        using (var stream = new MemoryStream(data))
        {
            using (var reader = new BinaryReader(stream))
            {
                OnReadConfig_binary(reader);
            }
        }
    }

    protected abstract void OnReadConfig_text(string text);
    protected abstract void OnReadConfig_binary(BinaryReader reader);

    private async UniTask<string> GetConfigContent_standalone()
    {
        var cfgPathOnline = $"../../GameConfig/{configFilename}.txt";
        var cfgPathOffline = $"GameConfig/{configFilename}.txt";
        var useOfflineCfg = true;

        #if USE_SERVER_GAME_CONFIG
        useOfflineCfg = !StaticUtils.CheckFileExist(cfgPathOnline);
        #endif

        if (useOfflineCfg)
        {
            return await StaticUtils.GetStreamingFileText(cfgPathOffline);
        }
        else
        {
            return StaticUtils.ReadTextFile(cfgPathOnline);
        }
    }

    private async UniTask<byte[]> GetConfigContent_mobile()
    {
        string cfgPath = $"GameConfig/{configFilename}.bin";
        var useOfflineCfg = true;

        #if USE_SERVER_GAME_CONFIG
        useOfflineCfg = !StaticUtils.CheckFileExist(cfgPath);
        #endif

        if (useOfflineCfg)
        {
            return await StaticUtils.GetStreamingFileBinary(cfgPath);
        }
        else
        {
            return StaticUtils.ReadBinaryFile(cfgPath);
        }
    }

    #endregion

    #region write

    public void WriteConfig_text()
    {
        //no need to do anything because normal config IO copy the whole folder of config
    }

    public void WriteConfig_binary()
    {
        var projPath = StaticUtils.GetProjectPath();
        var path = $"{projPath}/Assets/StreamingAssets/GameConfig/{configFilename}.bin";
        StaticUtils.OpenFileForWrite(path, OnWriteConfig_binary, isAbsolutePath: true);
    }

    protected abstract void OnWriteConfig_binary(BinaryWriter writer);

    #endregion
}
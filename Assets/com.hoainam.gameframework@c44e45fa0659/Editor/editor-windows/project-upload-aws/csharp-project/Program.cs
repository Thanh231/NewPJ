
#if IN_CSHARP_PROJ

using System.Text;
using Amazon;
using Amazon.CloudFront;
using Amazon.S3;

namespace CSharpProjUploadAWS;

internal class Program
{
    #region main

    private static async Task Main(string[] args)
    {
        var region = RegionEndpoint.GetBySystemName(Const.awsRegion);
        var s3Client = new AmazonS3Client(Const.awsAccessKeyId, Const.awsSecretAccessKey, region);
        var cloudFrontClient = new AmazonCloudFrontClient(Const.awsAccessKeyId, Const.awsSecretAccessKey, region);
        
        if (args == null || args.Length == 0)
        {
            args = GetDummyArgs();
        }

        var type = args[0];
        var serverEnvironment = args[1];

        try
        {
            switch (type)
            {
                case "config":
                    await UploadCfg(s3Client, serverEnvironment, args);
                    break;
                case "addressable":
                    await UploadAddressable(s3Client, serverEnvironment, args);
                    break;
                case "game_version":
                    await ChangeGameVersion(s3Client, serverEnvironment, args);
                    break;
                case "add_tester":
                    await AddTester(s3Client, args);
                    break;
                case "remove_tester":
                    await RemoveTester(s3Client, args);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown operation type: {type}");
            }
        
            await ApplySet(cloudFrontClient);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Environment.Exit(1);
        }
        
        Console.WriteLine("DONE...........");
    }

    private static string[] GetDummyArgs()
    {
        //add tester
        /*
        return
        [
            "add_tester",
            "tester_name_001",
            "device_id_001"
        ];
        /**/
        
        //remove tester
        /**/
        return
        [
            "remove_tester",
            "device_id_001"
        ];
        /**/

        //game version
        /*
        return
        [
            "game_version",
            "Dev",
            "1.0.0",
            false, //isMandatoryUpdate
        ];
        /**/

        //config
        /*
        return
        [
            "config",
            "Dev",
            @"D:\unity-projects\project_11\GameConfig",
            @"D:\unity-projects\project_11\Assets\StreamingAssets\GameConfig\all_config.bin",
        ];
        /**/

        //addressable
        /*
        return
        [
            "addressable",
            "Dev",
            @"D:\unity-projects\project_11_event_assets\ServerData",
        ];
        /**/
    }
    
    #endregion

    #region game version

    private static async Task ChangeGameVersion(AmazonS3Client s3Client, string serverEnvironment, string[] args)
    {
        var version = args[2];
        var isMandatoryUpdate = bool.Parse(args[3]);
        await UpdateServerConfig(s3Client, serverEnvironment, cfg =>
        {
            cfg.gameClientVersion = version;
            cfg.isMandatoryUpdate = isMandatoryUpdate;
        });
    }

    #endregion
    
    #region config

    private static async Task UploadCfg(AmazonS3Client s3Client, string serverEnvironment, string[] args)
    {
        var serverCfg = await UpdateServerConfig(s3Client, serverEnvironment, cfg =>
        {
            cfg.ChangeGameConfigVersion();
        });
        
        var cfgTextFolder = args[2];
        var cfgBinaryFolder = args[3];
        var parentPath = $"{serverEnvironment}/game_configs/{serverCfg.gameConfigVersion}";

        var lCfg = await UploadCfg_text(s3Client, cfgTextFolder, serverEnvironment, parentPath);
        await UploadCfg_binary(s3Client, cfgBinaryFolder, serverEnvironment, parentPath);
        await UploadCfg_metadata(s3Client, lCfg, parentPath);
    }

    private static async Task<List<string>> UploadCfg_text(AmazonS3Client s3Client, string cfgTextFolder, string serverEnvironment,
        string parentPath)
    {
        var lTasks = new List<Task>();
        var lFiles = Directory.GetFiles(cfgTextFolder);
        var lCfg = new List<string>();

        foreach (var file in lFiles)
        {
            var fileName = Path.GetFileName(file);
            var fileWithoutExt = Path.GetFileNameWithoutExtension(file);
            var serverPath = $"{parentPath}/text/{fileName}";
            lTasks.Add(UploadFile(s3Client, file, serverPath, FileType.CSV));
            lCfg.Add(fileWithoutExt);
        }

        await Task.WhenAll(lTasks);
        
        return lCfg;
    }

    private static async Task UploadCfg_binary(AmazonS3Client s3Client, string cfgBinaryFolder, string serverEnvironment,
        string parentPath)
    {
        var lTasks = new List<Task>();
        var lFiles = Directory.GetFiles(cfgBinaryFolder);

        foreach (var file in lFiles)
        {
            var fileName = Path.GetFileName(file);
            var serverPath = $"{parentPath}/binary/{fileName}";
            lTasks.Add(UploadFile(s3Client, file, serverPath, FileType.Binary));
        }
        
        await Task.WhenAll(lTasks);
    }

    private static async Task UploadCfg_metadata(AmazonS3Client s3Client, List<string> lCfg, string parentPath)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < lCfg.Count; i++)
        {
            sb.Append(lCfg[i]);
            if (i < lCfg.Count - 1)
            {
                sb.Append('\n');
            }
        }

        var serverPath = $"{parentPath}/list_cfg.txt";
        await UploadText(s3Client, sb.ToString(), serverPath, FileType.Text);
    }

    #endregion

    #region addressable

    private static async Task UploadAddressable(AmazonS3Client s3Client, string serverEnvironment, string[] args)
    {
        var parentFolder = args[2];
        var lTasks = new List<Task>();
        var lPlatforms = new List<string>()
        {
            "StandaloneWindows64",
            "Android",
            "iOS"
        };
        
        var serverCfg = await UpdateServerConfig(s3Client, serverEnvironment, cfg =>
        {
            cfg.ChangeAddressableVersion();
        });

        foreach (var platform in lPlatforms)
        {
            var localPath = $"{parentFolder}/{platform}/remotegroup_assets_all.bundle";
            var serverPath =
                $"{serverEnvironment}/addressable/{serverCfg.addressableVersion}/{platform}/remotegroup_assets_all.bundle";
            lTasks.Add(UploadFile(s3Client, localPath, serverPath, FileType.Binary));
        }

        await Task.WhenAll(lTasks);
    }

    #endregion

    #region testers

    private static async Task AddTester(AmazonS3Client s3Client, string[] args)
    {
        var testerName = args[1];
        var deviceId = args[2];
        await UpdateServerConfig(s3Client, "Dev", cfg =>
        {
            cfg.AddTester(testerName, deviceId);
        });
    }
    
    private static async Task RemoveTester(AmazonS3Client s3Client, string[] args)
    {
        var deviceId = args[1];
        await UpdateServerConfig(s3Client, "Dev", cfg =>
        {
            cfg.RemoveTester(deviceId);
        });
    }

    #endregion

    #region common utils

    private static async Task<ServerConfigJson> UpdateServerConfig(AmazonS3Client s3Client, string serverEnvironment,
        Action<ServerConfigJson> callback)
    {
        var serverCfg = new ServerConfigJson(serverEnvironment, s3Client);
        await serverCfg.Download();

        callback?.Invoke(serverCfg);

        await serverCfg.Upload();

        return serverCfg;
    }

    private static async Task UploadFile(AmazonS3Client s3Client, string filepath, string serverPath, FileType fileType)
    {
        var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
        await AwsApi.UploadFileToS3(s3Client, fileStream, fileType, serverPath);
    }

    private static async Task UploadText(AmazonS3Client s3Client, string text, string serverPath, FileType fileType)
    {
        var stream = GenerateStreamFromString(text);
        await AwsApi.UploadFileToS3(s3Client, stream, fileType, serverPath);
    }
    
    private static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    private static async Task ApplySet(AmazonCloudFrontClient cloudFrontClient)
    {
        await AwsApi.InvalidateCloudFront(cloudFrontClient);
    }
    
    #endregion
}

#endif
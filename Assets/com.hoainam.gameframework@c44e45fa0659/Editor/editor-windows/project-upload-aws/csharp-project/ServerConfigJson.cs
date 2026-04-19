
#if IN_CSHARP_PROJ

using System.Text;
using Amazon.S3;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CSharpProjUploadAWS;

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

    private string serverEnvironment;
    private AmazonS3Client s3Client;

    public ServerConfigJson(string serverEnvironment, AmazonS3Client s3Client)
    {
        this.serverEnvironment = serverEnvironment;
        this.s3Client = s3Client;
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
        return DateTime.UtcNow.ToString("dd-MM-yyyy__HH-mm-ss");
    }

    public void AddTester(string testerName, string deviceId)
    {
        var testerInfo = new TesterInfo()
        {
            testerName = testerName,
            deviceId = deviceId
        };
        listTesters.Add(testerInfo);
    }
    
    public void RemoveTester(string deviceId)
    {
        listTesters.RemoveAll(t => t.deviceId == deviceId);
    }

    #endregion
    
    #region download/upload

    public async Task Download()
    {
        var json = await AwsApi.DownloadFileFromS3($"{serverEnvironment}/server_config.json");
        if (!string.IsNullOrEmpty(json))
        {
            JsonConvert.PopulateObject(json, this);
        }
    }

    public async Task Upload()
    {
        var json = JsonSerializeToFriendlyText(this);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        await AwsApi.UploadFileToS3(s3Client, stream, FileType.Json, $"{serverEnvironment}/server_config.json");
    }

    private static string JsonSerializeToFriendlyText(object obj)
    {
        var settings = new JsonSerializerSettings();
        settings.Formatting = Formatting.Indented;
        settings.Converters.Add(new StringEnumConverter());
        return JsonConvert.SerializeObject(obj, settings);
    }

    #endregion
}

#endif
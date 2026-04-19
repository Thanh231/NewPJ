
#if GAME_CONTENT_USE_AWS

using System.IO;
using System.Text;
using Amazon;
using Amazon.CloudFront;
using Amazon.S3;
using Cysharp.Threading.Tasks;

public partial class AWSController : IServer_gameContent
{
    #region core

    private AmazonS3Client adminS3Client
    {
        get
        {
            var region = RegionEndpoint.GetBySystemName(GameFrameworkConfig.instance.awsRegion);
            return new AmazonS3Client(GetAdminCredentials(), region);
        }
    }
    
    private AmazonCloudFrontClient adminCloudFrontClient
    {
        get
        {
            var region = RegionEndpoint.GetBySystemName(GameFrameworkConfig.instance.awsRegion);
            return new AmazonCloudFrontClient(GetAdminCredentials(), region);
        }
    }
    
    private static string ConvertS3FileType(FileType fileType)
    {
        return fileType switch
        {
            FileType.Text => "text/plain",
            FileType.Json => "application/json",
            FileType.CSV => "text/csv",
            FileType.Binary => "application/octet-stream",
            FileType.Zip => "application/x-zip-compressed",
            _ => null,
        };
    }

    #endregion

    #region implement IServer_gameContent

    public async UniTask<string> GameContent_get(string key)
    {
        var rootUrl = GameFrameworkConfig.instance.awsS3GameContentUrl;
        var url = $"{rootUrl}/{key}";

        var res = await StaticUtils.GetHttpRequest(url, true);
        return res.resultAsText;
    }

    public async UniTask GameContent_download(string key, string path)
    {
        var rootUrl = GameFrameworkConfig.instance.awsS3GameContentUrl;
        var url = $"{rootUrl}/{key}";
        
        var res = await StaticUtils.GetHttpRequest(url, false);
        path = $"{path}/{Path.GetFileName(key)}";
        StaticUtils.WriteBinaryFile(path, res.resultAsBinary, true);
    }

    public async UniTask GameContent_set(string key, string value)
    {
        var cfg = GameFrameworkConfig.instance;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));
        var path = $"{cfg.awsS3GameContentRoot}/{key}";
        await UploadFileToS3(adminS3Client, stream, FileType.Text, path);
    }

    public async UniTask GameContent_set(string key, byte[] value)
    {
        var cfg = GameFrameworkConfig.instance;
        var stream = new MemoryStream(value);
        var path = $"{cfg.awsS3GameContentRoot}/{key}";
        await UploadFileToS3(adminS3Client, stream, FileType.Binary, path);
    }

    public async UniTask GameContent_applySet()
    {
        await InvalidateCloudFront(adminCloudFrontClient);
    }

    #endregion
}

#endif
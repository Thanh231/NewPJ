
using Cysharp.Threading.Tasks;

public interface IServer_gameContent
{
    UniTask<string> GameContent_get(string key);
    UniTask GameContent_download(string key, string path);
    
    UniTask GameContent_set(string key, string value);
    UniTask GameContent_set(string key, byte[] value);

    //using aws S3 + cloud front, cloud front using cache, so when update files in S3,
    //get files from cloud front won't return the latest version.
    //use this to clear cache from cloud front,
    //if don't use s3 + cloud front, this won't do anything.
    UniTask GameContent_applySet();
}

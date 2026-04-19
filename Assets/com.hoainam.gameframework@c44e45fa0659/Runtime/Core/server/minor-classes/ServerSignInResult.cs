public class ServerSignInResult
{
    public string uid;
    public bool needDownloadData;
    public bool needUploadData;

    public ServerSignInResult(string uid, bool needDownloadData, bool needUploadData)
    {
        this.uid = uid;
        this.needDownloadData = needDownloadData;
        this.needUploadData = needUploadData;
    }
}
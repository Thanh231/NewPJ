
#if IN_CSHARP_PROJ

using Amazon.CloudFront;
using Amazon.CloudFront.Model;
using Amazon.S3;
using Amazon.S3.Model;

namespace CSharpProjUploadAWS;

public class AwsApi
{
    #region download file from s3

    public static async Task<string> DownloadFileFromS3(string objPath)
    {
        using var client = new HttpClient();
        var url = $"{Const.awsS3GameContentUrl}/{objPath}";
        try
        {
            return await client.GetStringAsync(url);
        }
        catch (Exception e)
        {
            if (e.Message.Contains("403 (Forbidden)"))
            {
                return null;
            }

            throw;
        }
    }

    #endregion
    
    #region upload file to S3

    public static async Task UploadFileToS3(AmazonS3Client client, Stream stream, FileType fileType, string objPath)
    {
        var request = BuildPutObjectRequest(stream, fileType, objPath);
        await client.PutObjectAsync(request);
    }

    private static string ToS3FileType(FileType fileType)
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

    private static PutObjectRequest BuildPutObjectRequest(Stream stream, FileType fileType, string objPath)
    {
        return new PutObjectRequest()
        {
            BucketName = Const.awsS3BucketName,

            AutoResetStreamPosition = true,
            AutoCloseStream = true,
            InputStream = stream,

            //BucketKeyEnabled: use default encryption algorithm
            //CalculateContentMD5Header: use default checksum algorithm
            //CannedACL: dont grant access control on each bucket/object, use IAM role instead
            //ChecksumAlgorithm: use default checksum algorithm
            //ChecksumCRC32: use default checksum algorithm
            //ChecksumCRC32C: use default checksum algorithm
            //ChecksumSHA1: use default checksum algorithm
            //ChecksumSHA256: use default checksum algorithm
            ContentType = ToS3FileType(fileType),
            //DisableMD5Stream: use default checksum algorithm
            //DisablePayloadSigning: use default algorithm to check requester's authentication
            //ExpectedBucketOwner: check account who own bucket
            //FilePath: use stream instead of path to file
            //ContentBody: use stream instead of direct string
            //Grants: dont grant access control on each bucket/object, use IAM role instead
            Key = $"{Const.awsS3GameContentRoot}/{objPath}",
            //MD5Digest: use default checksum algorithm
            //ObjectLockLegalHoldStatus: object lock mean create object for the first time and cannot modify
            //ObjectLockMode: object lock mean create object for the first time and cannot modify
            //ObjectLockRetainUntilDate: object lock mean create object for the first time and cannot modify
            //RequestPayer: use to determine payer for this request
            //ServerSideEncryptionCustomerMethod: use default encryption algorithm
            //ServerSideEncryptionCustomerProvidedKey: use default encryption algorithm
            //ServerSideEncryptionCustomerProvidedKeyMD5: use default encryption algorithm
            //ServerSideEncryptionKeyManagementServiceEncryptionContext: use default encryption algorithm
            //ServerSideEncryptionKeyManagementServiceKeyId: use default encryption algorithm
            //ServerSideEncryptionMethod: use default encryption algorithm
            //StorageClass: use default Standard, this is for access frequency
            //StreamTransferProgress: callback for progress when upload
            //TagSet: use later for categorize by game, server, etc
            //UseChunkEncoding: algorithm used when upload by stream, use default
            //WebsiteRedirectLocation: used when bucket used to host a website
        };
    }
    
    #endregion

    #region invalidate cloud front

    public static async Task InvalidateCloudFront(AmazonCloudFrontClient client)
    {
        var request = BuildCreateInvalidationRequest();
        await client.CreateInvalidationAsync(request);
    }

    private static CreateInvalidationRequest BuildCreateInvalidationRequest()
    {
        var paths = new List<string>()
        {
            $"/{Const.awsS3GameContentRoot}/*"
        };
        
        return new CreateInvalidationRequest()
        {
            DistributionId = Const.awsCloudFrontDistributionId,
            InvalidationBatch = new InvalidationBatch()
            {
                Paths = new Paths()
                {
                    Quantity = paths.Count,
                    Items = paths,
                },
                CallerReference = DateTime.UtcNow.Ticks.ToString(),
            }
        };
    }

    #endregion
}

#endif
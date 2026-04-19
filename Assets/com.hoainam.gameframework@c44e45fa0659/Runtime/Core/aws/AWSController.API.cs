
using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;

#if GAME_CONTENT_USE_AWS

using Amazon.CloudFront;
using Amazon.CloudFront.Model;
using Amazon.S3;
using Amazon.S3.Model;

#endif


public partial class AWSController
{
#if GAME_CONTENT_USE_AWS
    
    #region upload file to S3
    
    private static async UniTask UploadFileToS3(AmazonS3Client client, Stream stream, FileType fileType, string objPath)
    {
        var request = BuildPutObjectRequest(stream, fileType, objPath);
        await client.PutObjectAsync(request);
    }
    
    private static PutObjectRequest BuildPutObjectRequest(Stream stream, FileType fileType, string objPath)
    {
        var cfg = GameFrameworkConfig.instance;
        
        return new PutObjectRequest()
        {
            BucketName = cfg.awsS3BucketName,

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
            ContentType = ConvertS3FileType(fileType),
            //DisableMD5Stream: use default checksum algorithm
            //DisablePayloadSigning: use default algorithm to check requester's authentication
            //ExpectedBucketOwner: check account who own bucket
            //FilePath: use stream instead of path to file
            //ContentBody: use stream instead of direct string
            //Grants: dont grant access control on each bucket/object, use IAM role instead
            Key = objPath,
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

    private static async UniTask InvalidateCloudFront(AmazonCloudFrontClient client)
    {
        var request = BuildCreateInvalidationRequest();
        await client.CreateInvalidationAsync(request);
    }

    private static CreateInvalidationRequest BuildCreateInvalidationRequest()
    {
        var cfg = GameFrameworkConfig.instance;
        var paths = new List<string>()
        {
            $"/{cfg.awsS3GameContentRoot}/*"
        };
        
        return new CreateInvalidationRequest()
        {
            DistributionId = cfg.awsCloudFrontDistributionId,
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
    
#endif
}
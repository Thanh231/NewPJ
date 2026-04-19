using UnityEngine;

public partial class GameFrameworkConfig
{
    [Header("AWS")]
    public string awsS3BucketName;
    public string awsS3GameContentRoot;
    public string awsS3UserDataRoot;
    public string awsRegion;
    public string awsS3GameContentUrl;
    public string awsCloudFrontDistributionId;
    public string awsLambdaUrlGetUtcTime;
    public string awsLambdaUrlValidateGooglePlayReceipt;
    public string awsLambdaUrlValidateAppStoreReceipt;
}

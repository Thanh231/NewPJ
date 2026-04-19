
using System;

#if GAME_CONTENT_USE_AWS
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
#endif

public partial class AWSController : SingletonMonoBehaviour<AWSController>
{
#if GAME_CONTENT_USE_AWS
    private static AWSCredentials GetAdminCredentials()
    {
        var chain = new CredentialProfileStoreChain();
        if (chain.TryGetAWSCredentials("default", out var credentials))
        {
            return credentials;
        }

        throw new Exception("need login AWS first");
    }
#endif
}

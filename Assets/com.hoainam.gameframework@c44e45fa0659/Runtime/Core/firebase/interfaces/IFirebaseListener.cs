
using System.Collections.Generic;

public interface IFirebaseListener
{
#if USE_FIREBASE_ANALYTICS
    Dictionary<string,string> GetUserProperties();
#endif
    
#if USE_FIREBASE_REMOTE_CONFIG
    Dictionary<string, object> GetDefaultRemoteConfigValue();
#endif
}

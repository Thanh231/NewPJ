
#if USE_FIREBASE_ANALYTICS

using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using UnityEngine;

public partial class FirebaseController : SingletonMonoBehaviour<FirebaseController>
{
	public bool isFirebaseAvailable { get; set; }
	
	private IFirebaseListener listener;
    
	public void Init(IFirebaseListener listener)
	{
		this.listener = listener;

		if (isFirebaseAvailable)
		{
			return;
		}
		
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
		{
			if (task.Result == DependencyStatus.Available)
			{
				Debug.Log($"[firebase] appId={FirebaseApp.DefaultInstance.Options.AppId}");

				isFirebaseAvailable = true;
				
#if USE_FIREBASE_REMOTE_CONFIG
				var defaultRemoteCfg = listener.GetDefaultRemoteConfigValue();
				AnalyticsAddDefaultRemoteCfg(defaultRemoteCfg);
				InitRemoteConfig(defaultRemoteCfg).Forget();
#endif
			}
			else
			{
				Debug.LogError($"[firebase] resolve dependency fail, status={task.Result}");
			}
		});
	}
}

#endif
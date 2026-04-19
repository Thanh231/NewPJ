
using Cysharp.Threading.Tasks;

public class UpdateGameController : SingletonMonoBehaviour<UpdateGameController>
{
    private IUpdateGame impl;
    
    protected override void Awake()
    {
        base.Awake();
        
#if !UNITY_EDITOR && UNITY_ANDROID && USE_INGAME_UPDATE
        impl = new UpdateGameImpl_android();
#else
        impl = new UpdateGameImpl_nonAndroid();
#endif
    }

    public async UniTask OptionalUpdate()
    {
        await impl.OptionalUpdate();
    }

    public void ForcedUpdate()
    {
        NativeController.instance.OpenStorePage();
    }
}
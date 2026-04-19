using Cysharp.Threading.Tasks;

public class UpdateGameImpl_nonAndroid : IUpdateGame
{
    public async UniTask OptionalUpdate()
    {
        await UniTask.CompletedTask;
        NativeController.instance.OpenStorePage();
    }
}
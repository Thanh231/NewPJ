using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class SceneController_load : MonoBehaviour
{
    public string nextSceneName;

    void Start()
    {
        Load().Forget();
    }

    private async UniTask Load()
    {
        var continueLoad = await InitFirstControllers();
        if (!continueLoad)
        {
            return;
        }

        await InitOrderedControllers();
        await InitUnorderedControllers();

        LoaderOverlayManager.instance.LoadScene(nextSceneName);
    }

    private async UniTask<bool> InitFirstControllers()
    {
        //load game configs
        var lCfgs = ConfigManager.GetListConfigsImplementedInClientCode();
        if (lCfgs == null)
        {
            throw new Exception("there are no IListConfigDeclaration implementations");
        }
        #if USE_SERVER_GAME_CONFIG
        #else
        await ConfigManager.instance.LoadAllConfigs(lCfgs);
        #endif

        //load player model
        PlayerModelManager.instance.LoadAllModels(PlayerModelManager.GetListModelImplementedInClientCode());

        return true;
    }

    private async UniTask InitOrderedControllers()
    {
        
    }

    private async UniTask InitUnorderedControllers()
    {
        
    }
}
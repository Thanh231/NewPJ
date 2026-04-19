
using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Debug = UnityEngine.Debug;

public class PlayerModelManager : SingletonMonoBehaviour<PlayerModelManager>
{
    #region data members

#if (UNITY_EDITOR && !EDITOR_USE_BINARY_MODEL) || (!UNITY_EDITOR && UNITY_STANDALONE)
    private static IPlayerModelFile modelFile = new PlayerModelFile_text();
#else
	private static IPlayerModelFile modelFile = new PlayerModelFile_binary();
#endif
    
    public const string ModelFileNameOnServer = "AllModels";

    private List<BasePlayerModel> lModels;
    private Dictionary<string, BasePlayerModel> dictModel = new();
    private bool isUploadingModel = false;

    #endregion

    #region list models

    public List<string> GetListModelNames()
    {
        var l = new List<string>();
        foreach (var i in lModels)
        {
            l.Add(i.GetType().Name);
        }
        return l;
    }

    public BasePlayerModel GetPlayerModel(string modelName)
    {
        if (dictModel.ContainsKey(modelName))
        {
            return dictModel[modelName];
        }
        else
        {
            throw new Exception($"there's no {modelName} in lModels");
        }
    }

    public BasePlayerModel GetPlayerModel(Type type)
    {
        return GetPlayerModel(type.Name);
    }

    public T GetPlayerModel<T>() where T : BasePlayerModel
    {
        return (T)GetPlayerModel(typeof(T));
    }

    #endregion

    #region load model

    /// <summary>
    /// sometimes, we need to load a specific model before we load all models
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T QuickLoadModel<T>() where T : BasePlayerModel, new()
    {
        var model = new T();
        LoadModel(model);
        return model;
    }

    public void LoadAllModels(List<BasePlayerModel> lModels)
    {
        this.lModels = lModels;

        foreach (var model in lModels)
        {
            var modelName = model.GetType().Name;
            if (dictModel.ContainsKey(modelName))
            {
                throw new Exception($"there're more than 1 {modelName} in lModels");
            }
            else
            {
                dictModel.Add(modelName, model);
            }
        }

        foreach (var i in lModels)
        {
            LoadModel(i);
        }
    }

    private static void LoadModel(BasePlayerModel model)
    {
        var path = GetModelFilePath(model);
        if (StaticUtils.CheckFileExist(path))
        {
            try
            {
                modelFile.ReadModel(path, model);
            }
            catch (Exception e)
            {
                Debug.LogError($"[PlayerModel] failed to load model {model.GetType().Name}, details below:");
                StaticUtils.RethrowException(e);
            }
        }
        else
        {
            model.OnModelInitializing();
            SaveModel(model);
        }
        model.OnModelLoaded();
    }

    #endregion

    #region save model

    public static void SaveModel(BasePlayerModel model)
    {
        var path = GetModelFilePath(model);
        modelFile.WriteModel(path, model);

        var modelName = model.GetType().Name;
        StaticUtils.LogFramework($"save model {modelName}");
    }

    public void SaveAllModel()
    {
        foreach (var i in lModels)
        {
            SaveModel(i);
        }
    }

    #endregion

    #region upload model

    public async UniTask UploadPlayerModel()
    {
        if (!ServerUsersManager.instance.IsLoggedIn || isUploadingModel)
        {
            return;
        }
        
        isUploadingModel = true;
        var binaryFile = new PlayerModelFile_binary();

        using (var stream = new MemoryStream())
        {
            AppendAllModels(stream, binaryFile);
            var userId = ServerUsersManager.instance.userUID.Value;
            await ServerController.instance.PlayerData_set(userId, stream);
        }
        
        isUploadingModel = false;
    }

    private void AppendAllModels(MemoryStream stream, PlayerModelFile_binary binaryFile)
    {
        var writer = new BinaryWriter(stream);

        writer.Write(lModels.Count);
        foreach (var model in lModels)
        {
            writer.Write(model.GetType().Name);
            var data = ModelToByteArray(model, binaryFile);
            writer.Write(data.Length);
            writer.Write(data);
        }
    }

    private byte[] ModelToByteArray(BasePlayerModel model, PlayerModelFile_binary binaryFile)
    {
        using (var stream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(stream))
            {
                binaryFile.WriteModel(writer, model);
                return stream.ToArray();
            }
        }
    }

    #endregion

    #region download model

    public async UniTask DownloadPlayerModel(bool useCache = false)
    {
        var userId = ServerUsersManager.instance.userUID.Value;
        var binaryFile = new PlayerModelFile_binary();

        var dicPlayerModel = useCache ? cacheServerPlayerModel : await DownloadPlayerModelIntoDic(userId);

        foreach (var model in lModels)
        {
            var modelName = model.GetType().Name;
            if (dicPlayerModel.ContainsKey(modelName))
            {
                ApplyPlayerModel(model, dicPlayerModel[modelName], binaryFile);
            }
        }

        SaveAllModel();
        GameReloader.instance.Reload();
    }

    private async UniTask<Dictionary<string, byte[]>> DownloadPlayerModelIntoDic(string userId)
    {
        var dict = new Dictionary<string, byte[]>();
        await ServerController.instance.PlayerData_get(userId, stream =>
        {
            using (var reader = new BinaryReader(stream))
            {
                var count = reader.ReadInt32();
                for (var i = 0; i < count; i++)
                {
                    var modelName = reader.ReadString();
                    var dataLength = reader.ReadInt32();
                    var data = reader.ReadBytes(dataLength);
                    dict.Add(modelName, data);
                }
            }
        });
        return dict;
    }

    private void ApplyPlayerModel(BasePlayerModel model, byte[] data, PlayerModelFile_binary binaryFile)
    {
        using (var stream = new MemoryStream(data))
        {
            binaryFile.ReadModel(stream, model);
        }
    }

    #endregion

    #region download individual model

    private Dictionary<string, byte[]> cacheServerPlayerModel;

    public async UniTask CacheServerPlayerModel()
    {
        var userId = ServerUsersManager.instance.userUID.Value;
        cacheServerPlayerModel = await DownloadPlayerModelIntoDic(userId);
    }

    public T DownloadPlayerModel<T>() where T : BasePlayerModel, new()
    {
        if (cacheServerPlayerModel == null)
        {
            throw new Exception("need to call CacheServerPlayerModel() first");
        }

        var model = new T();
        var binaryFile = new PlayerModelFile_binary();
        ApplyPlayerModel(model, cacheServerPlayerModel[typeof(T).Name], binaryFile);
        return model;
    }

    #endregion

    #region utils

    public void ClearAllModels()
    {
        foreach (var i in lModels)
        {
            var path = GetModelFilePath(i);
            StaticUtils.DeleteFile(path);
        }
    }

    public static string GetModelFolderPath()
    {
#if UNITY_EDITOR
        return "../PlayerModels";
#elif UNITY_STANDALONE
	    return "../../PlayerModels";
#else
	    return "PlayerModels";
#endif
    }

    private static string GetModelFilePath(BasePlayerModel model)
    {
        var folder = GetModelFolderPath();
        var filename = $"{model.GetType().Name}.{modelFile.Extension}";
        return $"{folder}/{filename}";
    }

    public static List<BasePlayerModel> GetListModelImplementedInClientCode()
    {
        var assembly = StaticUtils.GetAssembly(StaticUtils.MainAssemblyName);
		var interfaceTypes = StaticUtils.ListClassImplementOrInherit(assembly, typeof(IListModelDeclaration));

		if (interfaceTypes.Count != 1)
		{
			throw new Exception($"there are {interfaceTypes.Count} IListModelDeclaration implementations");
		}
		
		var listModels = (IListModelDeclaration)Activator.CreateInstance(interfaceTypes[0]);

		return listModels.listModels;
    }

    #endregion
}

using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : SingletonMonoBehaviour<ConfigManager>
{
	#region core

	public class LoadAllCfgResult
	{
		public bool needDownloadCfg;
		public bool needInformUser;
	}

	private ConfigReadWriteManager configReadWriteManager;
	private ExtraConfigReadWriteManager extraConfigReadWriteManager;

	public List<IBaseConfig> ListConfigs => configReadWriteManager.listConfigs;
	
	public T GetConfig<T>() where T : class, IBaseConfig
	{
		var typeT = typeof(T);
		if (configReadWriteManager.dicConfigs.ContainsKey(typeT))
		{
			return (T)configReadWriteManager.dicConfigs[typeT];
		}
		else
		{
			throw new Exception($"there's no {typeT.Name} in listConfigs");
		}
	}

	public T GetExtraConfig<T>() where T : ExtraConfigReadWriteManager
	{
		return (T)extraConfigReadWriteManager;
	}
	
	#endregion
	
	#region load configs

	/// <summary>
	/// load all config when use remote configs from server
	/// </summary>
	/// <param name="serverCfg"></param>
	/// <param name="listConfigDeclaration"></param>
	/// <returns>true if it has new config version from server</returns>
	public async UniTask<LoadAllCfgResult> LoadAllConfigs(ServerConfigJson serverCfg, List<IBaseConfig> lCfgs)
	{
		configReadWriteManager = new ConfigReadWriteManager(lCfgs);
		#if USE_EXTRA_GAME_CONFIG
		extraConfigReadWriteManager = GetExtraCfgImplementedInClientCode();
		#endif

		var result = new LoadAllCfgResult();
		
#if USE_SERVER_GAME_CONFIG && !UNITY_EDITOR
		result = await DownloadServerGameConfig(serverCfg);
#endif

		await DoLoadAllConfigs();
		
		return result;
	}

	public async UniTask LoadAllConfigs(List<IBaseConfig> lCfgs)
	{
		configReadWriteManager = new ConfigReadWriteManager(lCfgs);
		#if USE_EXTRA_GAME_CONFIG
		extraConfigReadWriteManager = GetExtraCfgImplementedInClientCode();
		#endif
		await DoLoadAllConfigs();
	}

	private async UniTask DoLoadAllConfigs()
	{
#if UNITY_EDITOR
		await UniTask.CompletedTask;
		configReadWriteManager.ReadConfig_editor();
		if (extraConfigReadWriteManager != null)
		{
			extraConfigReadWriteManager.ReadConfig_editor();
		}
		
#elif UNITY_STANDALONE_WIN
		await configReadWriteManager.ReadConfig_standalone();
		if (extraConfigReadWriteManager != null)
		{
			await extraConfigReadWriteManager.ReadConfig_standalone();
		}
#else
		await configReadWriteManager.ReadConfig_mobile();
		if (extraConfigReadWriteManager != null)
		{
			await extraConfigReadWriteManager.ReadConfig_mobile();
		}
#endif
	}

	#endregion

	#region download config

	/// <summary>
	/// do download remote config from server
	/// </summary>
	/// <param name="serverCfg"></param>
	/// <returns>true if it has new config version from server</returns>
	private async UniTask<LoadAllCfgResult> DownloadServerGameConfig(ServerConfigJson serverCfg)
	{
		var localSettings = new ServerConfigJson();
		localSettings.Load();

		var result = new LoadAllCfgResult();

		if (string.IsNullOrEmpty(serverCfg.gameConfigVersion) ||
		    localSettings.gameConfigVersion == serverCfg.gameConfigVersion)
		{
			return result;
		}

		Debug.LogError(
			$"local version={localSettings.gameConfigVersion} remote version={serverCfg.gameConfigVersion} => download config");
		var parentPath = $"{ServerController.instance.serverEnvironment}/game_configs/{serverCfg.gameConfigVersion}";
#if UNITY_STANDALONE_WIN
		await DownloadConfig_standalone(parentPath);
#else
		await DownloadConfig_mobile(parentPath);
#endif

		result.needDownloadCfg = true;
		result.needInformUser = !string.IsNullOrEmpty(localSettings.gameConfigVersion);
		return result;
	}

	private async UniTask DownloadConfig_standalone(string parentPath)
	{
		var configFolder = $"{Application.dataPath}/../../GameConfig";
		var lTasks = new List<UniTask>();
		foreach (var i in ListConfigs)
		{
			var cfgName = i.GetType().Name;
			var key = $"{parentPath}/text/{cfgName}.csv";
			lTasks.Add(ServerController.instance.GameContent_download(key, configFolder));
		}
		await UniTask.WhenAll(lTasks);
	}

	private async UniTask DownloadConfig_mobile(string parentPath)
	{
		var configFolder = $"{Application.persistentDataPath}/GameConfig";
		var key = $"{parentPath}/binary/all_config.bin";
		await ServerController.instance.GameContent_download(key, configFolder);
	}

	#endregion

	#region prepare cfg for build

	public static void PrepareConfigForBuild(bool isWindows)
	{
		var cfgPath = $"{Application.dataPath}/StreamingAssets/GameConfig";
		StaticUtils.DeleteFolder(cfgPath, isAbsolutePath: true);

		PrepareConfigForBuild_normalCfg(isWindows);
		PrepareConfigForBuild_extraCfg(isWindows);
	}

	private static void PrepareConfigForBuild_normalCfg(bool isWindows)
	{
		var lCfgs = GetListConfigsImplementedInClientCode();
		if (lCfgs == null)
		{
			return;
		}

		var configReadWriteManager = new ConfigReadWriteManager(lCfgs);
		configReadWriteManager.ReadConfig_editor();

		if (isWindows)
		{
			configReadWriteManager.WriteConfig_standalone();
		}
		else
		{
			configReadWriteManager.WriteConfig_mobile();
		}
	}

	private static void PrepareConfigForBuild_extraCfg(bool isWindows)
	{
		var extraCfgReadWriteManager = GetExtraCfgImplementedInClientCode();
		if (extraCfgReadWriteManager == null)
		{
			return;
		}

		extraCfgReadWriteManager.ReadConfig_editor();

		if (isWindows)
		{
			extraCfgReadWriteManager.WriteConfig_text();
		}
		else
		{
			extraCfgReadWriteManager.WriteConfig_binary();
		}
	}

	#endregion

	#region get implementation in client code
	
	public static List<IBaseConfig> GetListConfigsImplementedInClientCode()
	{
		var assembly = StaticUtils.GetAssembly(StaticUtils.MainAssemblyName);
		var interfaceTypes = StaticUtils.ListClassImplementOrInherit(assembly, typeof(IListConfigDeclaration));

		if (interfaceTypes.Count == 0)
		{
			return null;
		}

		if (interfaceTypes.Count > 1)
		{
			throw new Exception($"there are {interfaceTypes.Count} IListConfigDeclaration implementations");
		}
		
		var listConfigs = (IListConfigDeclaration)Activator.CreateInstance(interfaceTypes[0]);

		return listConfigs.listConfigs;
	}

	public static ExtraConfigReadWriteManager GetExtraCfgImplementedInClientCode()
	{
		var assembly = StaticUtils.GetAssembly(StaticUtils.MainAssemblyName);
		var classTypes = StaticUtils.ListClassImplementOrInherit(assembly, typeof(ExtraConfigReadWriteManager));

		if (classTypes.Count == 0)
		{
			return null;
		}

		if (classTypes.Count > 1)
		{
			throw new Exception($"there are {classTypes.Count} ExtraConfigReadWriteManager implementations");
		}
		
		return (ExtraConfigReadWriteManager)Activator.CreateInstance(classTypes[0]);
	}

	#endregion
}

using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class ObjectPoolPrefabCfg
{
	public string name;
	public bool useAssetRef;
	public GameObject prefab;
	public AssetReferenceGameObject assetRef;
	public int preSpawnedAmount;
	public float lifeTimeInSecs;

	public async UniTask<GameObject> GetPrefab()
	{
		if (useAssetRef)
		{
			return await AssetManager.instance.LoadPrefab(assetRef);
		}
		else
		{
			return prefab;
		}
	}
}
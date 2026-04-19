
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

public class AssetGroup
{
	private readonly List<AsyncOperationHandle> handles = new();

	private async UniTask<T> LoadAsset<T>(object key)
	{
		var handle = Addressables.LoadAssetAsync<T>(key);
		handles.Add(handle);

		await UniTask.WaitUntil(() => handle.IsDone);
		
		return handle.Result;
	}

	public async UniTask<GameObject> LoadPrefab(AssetReferenceGameObject assetRef)
	{
		return await LoadAsset<GameObject>(assetRef);
	}

	public async UniTask<GameObject> LoadPrefab(string parentPath, string assetName)
	{
		var fullPath = $"{parentPath}/{assetName}.prefab";
		return await LoadAsset<GameObject>(fullPath);
	}

	public async UniTask<Sprite> LoadSprite(string parentPath, string assetName)
	{
		var fullPath = $"{parentPath}/{assetName}.png";
		return await LoadAsset<Sprite>(fullPath);
	}

	public async UniTask<TextAsset> LoadText(string parentPath, string assetName)
	{
		var fullPath = $"{parentPath}/{assetName}.txt";
		return await LoadAsset<TextAsset>(fullPath);
	}

	public async UniTask<TileBase> LoadTile(string parentPath, string assetName)
	{
		var fullPath = $"{parentPath}/{assetName}.asset";
		return await LoadAsset<TileBase>(fullPath);
	}

	public async UniTask<TMP_FontAsset> LoadTmpFont(string parentPath, string assetName)
	{
		var fullPath = $"{parentPath}/{assetName}.asset";
		return await LoadAsset<TMP_FontAsset>(fullPath);
	}

	//to release asset, create a new group with BundleMode=PackSeparately,
	//assign address to assets separately, don't assign address to parent folder
	public void ReleaseGroup()
	{
		foreach (var handle in handles)
		{
			Addressables.Release(handle);
		}
		handles.Clear();
	}
}

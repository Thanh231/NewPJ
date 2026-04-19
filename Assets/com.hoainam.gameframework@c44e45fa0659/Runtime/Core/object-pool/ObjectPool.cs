using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Text;

public class ObjectPool : MonoBehaviour
{
	public List<ObjectPoolPrefabCfg> prefabCfgs;

	private Dictionary<string, List<GameObject>> dicPool = new Dictionary<string, List<GameObject>>();

	private async UniTask Start()
	{
		foreach (var cfg in prefabCfgs)
		{
			if (cfg.preSpawnedAmount > 0)
			{
				var prefab = await cfg.GetPrefab();
				dicPool.Add(cfg.name, new List<GameObject>());
				for (var i = 0; i < cfg.preSpawnedAmount; i++)
				{
					var o = Instantiate(prefab, transform);
					o.SetActive(false);
					dicPool[cfg.name].Add(o);
				}
			}
		}
	}

	#region spawn

	public async UniTask<GameObject> Spawn(string name)
	{
		var cfg = prefabCfgs.Find(x => x.name == name);

		if (cfg == null)
		{
			var sb = new StringBuilder("[");
			for (var i = 0; i < prefabCfgs.Count; i++)
			{
				sb.Append(prefabCfgs[i].name);
				if (i < prefabCfgs.Count - 1)
				{
					sb.Append(", ");
				}
			}
			sb.Append("]");
			
			throw new Exception($"[pool] can't find prefab with name={name}, all names={sb}");
		}

		var obj = FindInactiveObject(name);
		if (obj)
		{
			obj.SetActive(true);
		}
		else
		{
			var prefab = await cfg.GetPrefab();
			obj = Instantiate(prefab, transform);
			if (!dicPool.ContainsKey(name))
			{
				dicPool.Add(name, new List<GameObject>());
			}
			dicPool[name].Add(obj);
		}

		if (cfg.lifeTimeInSecs > 0)
		{
			StartCoroutine(WaitToDespawn(obj, cfg.lifeTimeInSecs));
		}

		return obj;
	}

	private IEnumerator WaitToDespawn(GameObject obj, float lifetime)
	{
		yield return new WaitForSeconds(lifetime);
		Despawn(obj);
	}

	private GameObject FindInactiveObject(string name)
	{
		if (!dicPool.ContainsKey(name))
		{
			return null;
		}
		return dicPool[name].Find(x => !x.activeSelf);
	}

	public async UniTask<T> Spawn<T>(string name)
	{
		var o = await Spawn(name);
		return o.GetComponent<T>();
	}

	#endregion

	#region despawn

	public void Despawn(GameObject o)
	{
		o.SetActive(false);
	}

	public void DespawnAll()
	{
		foreach (var i in dicPool)
		{
			foreach (var j in i.Value)
			{
				Despawn(j);
			}
		}
	}

	#endregion
}
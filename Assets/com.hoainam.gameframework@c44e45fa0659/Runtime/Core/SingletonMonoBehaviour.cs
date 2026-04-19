
using System;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
	public static T instance { get; private set; }

	protected virtual void Awake()
	{
		if (instance != null)
		{
			throw new Exception($"there are more than 1 instance of type {typeof(T).Name}");
		}

		instance = (T)this;
	}

	protected virtual void OnDestroy()
	{
		instance = null;
	}
}
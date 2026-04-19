
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseRecycleView : MonoBehaviour
{
	#region data members

	public RectTransform prefabItem;

	private Queue<RectTransform> inactivePool = new Queue<RectTransform>();
	protected List<object> lData;

	private ScrollRect scrollRect = null;
	protected ScrollRect ScrollRect
	{
		get
		{
			if (!scrollRect)
			{
				scrollRect = GetComponent<ScrollRect>();
			}
			return scrollRect;
		}
	}

	protected float ItemLength => ScrollRect.vertical ? prefabItem.sizeDelta.y : prefabItem.sizeDelta.x;
	protected float ViewportLength => ScrollRect.vertical ? ScrollRect.viewport.rect.height : ScrollRect.viewport.rect.width;

	#endregion

	#region setup

	public void SetData(List<object> lData)
	{
		this.lData = lData;

		ScrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
		ScrollRect.onValueChanged.AddListener(OnScrollValueChanged);

		SetupContentGameObject();
		InstantiateListItems();
	}

	private void SetupContentGameObject()
	{
		var rect = ScrollRect.content.GetComponent<RectTransform>();
		var contentLength = CalculateContentLength();
		if (ScrollRect.vertical)
		{
			rect.offsetMin = new Vector2(0, -contentLength);
		}
		else
		{
			rect.offsetMax = new Vector2(contentLength, 0);
		}
	}

	protected abstract float CalculateContentLength();
	protected abstract void InstantiateListItems();

	#endregion

	#region update item visibility

	private void OnScrollValueChanged(Vector2 _)
	{
		CheckToRemoveBegin();
		CheckToRemoveEnd();

		CheckToAddBegin();
		CheckToAddEnd();
	}

	protected abstract void CheckToRemoveBegin();
	protected abstract void CheckToRemoveEnd();

	protected abstract void CheckToAddBegin();
	protected abstract void CheckToAddEnd();

	#endregion

	#region pool

	protected RectTransform SpawnItemFromPool()
	{
		RectTransform rect;
		if (inactivePool.Count > 0)
		{
			rect = inactivePool.Dequeue();
			rect.gameObject.SetActive(true);
		}
		else
		{
			rect = Instantiate(prefabItem, ScrollRect.content.transform);
		}

		return rect;
	}

	protected void DespawnItemToPool(RectTransform rect)
	{
		rect.gameObject.SetActive(false);
		inactivePool.Enqueue(rect);
	}

	#endregion

	#region utils

	protected float GetItemPos_local(RectTransform rect)
	{
		return ScrollRect.vertical ? rect.anchoredPosition.y : rect.anchoredPosition.x;
	}

	protected float GetItemPos_global(RectTransform rect)
	{
		var pos = GetItemPos_local(rect);
		pos += ScrollRect.vertical ? ScrollRect.content.anchoredPosition.y : ScrollRect.content.anchoredPosition.x;
		return pos;
	}

	protected void ConfigureListItem(RectTransform item, float posLength, object data, float posSide = 0)
	{
		var isVertical = ScrollRect.vertical;
		if (isVertical)
		{
			item.anchorMin = new Vector2(0.5f, 1);
			item.anchorMax = new Vector2(0.5f, 1);
		}
		else
		{
			item.anchorMin = new Vector2(0, 0.5f);
			item.anchorMax = new Vector2(0, 0.5f);
		}
		item.pivot = isVertical ? new Vector2(0.5f, 1) : new Vector2(0, 0.5f);
		item.anchoredPosition = isVertical ? new Vector2(posSide, -posLength) : new Vector2(posLength, posSide);

		var listItem = item.GetComponent<RecycleScrollViewItem>();

		if (!listItem)
		{
			throw new Exception("item of recycle view must have RecycleScrollViewItem attached to it");
		}

		listItem.SetData(data);
	}

	#endregion
}
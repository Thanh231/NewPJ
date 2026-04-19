using UnityEngine;
using UnityEngine.UI;

public class RecycleListView : BaseRecycleView
{
	#region data members

	public float itemSpace;

	private Deque<RectTransform> activePool = new Deque<RectTransform>();
	private Vector2Int activeItemsIdx;

	#endregion

	#region setup list view

	protected override float CalculateContentLength()
	{
		var nItems = lData.Count;
		return ItemLength * nItems + itemSpace * (nItems - 1);
	}

	protected override void InstantiateListItems()
	{
		var pos = 0f;
		activeItemsIdx = new Vector2Int(0, -1);

		for (var i = 0; i < lData.Count; i++)
		{
			var rect = SpawnItemFromPool();
			ConfigureListItem(rect, pos, lData[i]);
			activePool.Enqueue_end(rect);

			pos += ItemLength + itemSpace;
			activeItemsIdx.y++;

			if (pos >= ViewportLength)
			{
				break;
			}
		}
	}

	#endregion

	#region update item visibility

	protected override void CheckToAddBegin()
	{
		var firstItem = activePool.Peek_begin();
		var nextItemDist = ScrollRect.vertical ? itemSpace : -itemSpace;
		var pos = GetItemPos_global(firstItem) + nextItemDist;
		var needSpawn = ScrollRect.vertical ? pos < 0 : pos > 0;
		if (needSpawn && activeItemsIdx.x > 0)
		{
			var rect = SpawnItemFromPool();
			nextItemDist += ScrollRect.vertical ? ItemLength : -ItemLength;
			var newItemPos = GetItemPos_local(firstItem) + nextItemDist;
			activeItemsIdx.x--;
			ConfigureListItem(rect, Mathf.Abs(newItemPos), lData[activeItemsIdx.x]);
			activePool.Enqueue_begin(rect);
		}
	}

	protected override void CheckToRemoveBegin()
	{
		var firstItem = activePool.Peek_begin();
		var pos = GetItemPos_global(firstItem);
		pos += ScrollRect.vertical ? -ItemLength : ItemLength;
		var needDespawn = ScrollRect.vertical ? pos > 0 : pos < 0;
		if (needDespawn)
		{
			DespawnItemToPool(firstItem);
			activePool.Dequeue_begin();
			activeItemsIdx.x++;
		}
	}

	protected override void CheckToAddEnd()
	{
		var lastItem = activePool.Peek_end();
		var nextItemDist = ScrollRect.vertical ? -(ItemLength + itemSpace) : ItemLength + itemSpace;
		var pos = GetItemPos_global(lastItem) + nextItemDist;
		var needSpawn = ScrollRect.vertical ? pos > -ViewportLength : pos < ViewportLength;
		if (needSpawn && activeItemsIdx.y < lData.Count - 1)
		{
			var rect = SpawnItemFromPool();
			var newItemPos = GetItemPos_local(lastItem) + nextItemDist;
			activeItemsIdx.y++;
			ConfigureListItem(rect, Mathf.Abs(newItemPos), lData[activeItemsIdx.y]);
			activePool.Enqueue_end(rect);
		}
	}

	protected override void CheckToRemoveEnd()
	{
		var lastItem = activePool.Peek_end();
		var pos = GetItemPos_global(lastItem);
		var needDespawn = ScrollRect.vertical ? pos < -ViewportLength : pos > ViewportLength;
		if (needDespawn)
		{
			DespawnItemToPool(lastItem);
			activePool.Dequeue_end();
			activeItemsIdx.y--;
		}
	}

	#endregion
}
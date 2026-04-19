using UnityEngine;

public class RecycleGridView : BaseRecycleView
{
	#region data members

	public Vector2 itemSpace;
	public int nItemsPerLine;

	private Deque<RectTransform> activePool = new Deque<RectTransform>();
	private Vector2Int activeItemsIdx;

	private int LinesCount => lData.Count % nItemsPerLine == 0 ? lData.Count / nItemsPerLine : lData.Count / nItemsPerLine + 1;
	private float ItemSpace => ScrollRect.vertical ? itemSpace.y : itemSpace.x;

	private float ItemSide => ScrollRect.vertical ? prefabItem.sizeDelta.x : prefabItem.sizeDelta.y;
	private float ItemSpaceSide => ScrollRect.vertical ? itemSpace.x : itemSpace.y;
	private float ContentSide => ItemSide * nItemsPerLine + ItemSpaceSide * (nItemsPerLine - 1);
	private float InitialPosSide => ScrollRect.vertical ? -ContentSide / 2 + ItemSide / 2 : ContentSide / 2 - ItemSide / 2;

	#endregion

	#region setup list view

	protected override float CalculateContentLength()
	{
		return ItemLength * LinesCount + ItemSpace * (LinesCount - 1);
	}

	protected override void InstantiateListItems()
	{
		var pos = 0f;
		activeItemsIdx = new Vector2Int(0, -1);
		for (var i = 0; i < lData.Count; i += nItemsPerLine)
		{
			activeItemsIdx.y += SpawnALine(pos, i, isBegin: false);

			pos += ItemLength + ItemSpace;
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
		var nextItemDist = ScrollRect.vertical ? ItemSpace : -ItemSpace;
		var pos = GetItemPos_global(firstItem) + nextItemDist;
		var needSpawn = ScrollRect.vertical ? pos < 0 : pos > 0;
		if (needSpawn && activeItemsIdx.x > 0)
		{
			nextItemDist += ScrollRect.vertical ? ItemLength : -ItemLength;
			var newItemPos = GetItemPos_local(firstItem) + nextItemDist;
			activeItemsIdx.x -= SpawnALine(Mathf.Abs(newItemPos), activeItemsIdx.x - nItemsPerLine, isBegin: true);
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
			activeItemsIdx.x += DespawnALine(activeItemsIdx.x, isBegin: true);
		}
	}

	protected override void CheckToAddEnd()
	{
		var lastItem = activePool.Peek_end();
		var nextItemDist = ScrollRect.vertical ? -(ItemLength + ItemSpace) : ItemLength + ItemSpace;
		var pos = GetItemPos_global(lastItem) + nextItemDist;
		var needSpawn = ScrollRect.vertical ? pos > -ViewportLength : pos < ViewportLength;
		if (needSpawn && activeItemsIdx.y < lData.Count - 1)
		{
			var newItemPos = GetItemPos_local(lastItem) + nextItemDist;
			activeItemsIdx.y += SpawnALine(Mathf.Abs(newItemPos), activeItemsIdx.y + 1, isBegin: false);
		}
	}

	protected override void CheckToRemoveEnd()
	{
		var lastItem = activePool.Peek_end();
		var pos = GetItemPos_global(lastItem);
		var needDespawn = ScrollRect.vertical ? pos < -ViewportLength : pos > ViewportLength;
		if (needDespawn)
		{
			activeItemsIdx.y -= DespawnALine(activeItemsIdx.y - activeItemsIdx.y % nItemsPerLine, isBegin: false);
		}
	}

	#endregion

	#region utils

	private int SpawnALine(float pos, int idx, bool isBegin)
	{
		var posSide = InitialPosSide;
		var dtPosSide = InitialPosSide > 0 ? -(ItemSide + ItemSpaceSide) : ItemSide + ItemSpaceSide;
		var count = 0;
		for (var i = 0; i < nItemsPerLine; i++)
		{
			if (idx + i >= lData.Count)
			{
				break;
			}

			var rect = SpawnItemFromPool();
			ConfigureListItem(rect, pos, lData[idx + i], posSide);
			if (isBegin)
			{
				activePool.Enqueue_begin(rect);
			}
			else
			{
				activePool.Enqueue_end(rect);
			}
			
			count++;
			posSide += dtPosSide;
		}
		return count;
	}

	private int DespawnALine(int idx, bool isBegin)
	{
		var count = 0;
		for (var i = 0; i < nItemsPerLine; i++)
		{
			if (idx + i >= lData.Count)
			{
				break;
			}
			RectTransform item;
			if (isBegin)
			{
				item = activePool.Dequeue_begin();
			}
			else
			{
				item = activePool.Dequeue_end();
			}
			DespawnItemToPool(item);

			count++;
		}
		return count;
	}

	#endregion
}
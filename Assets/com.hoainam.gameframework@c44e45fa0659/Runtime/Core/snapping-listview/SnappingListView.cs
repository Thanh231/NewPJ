using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.UI;

public class SnappingListView : MonoBehaviour
{
    public float snappingTime = 0.5f;
    public Ease snappingEase = Ease.OutBounce;

    public ReactiveProperty<int> ItemIdx = new ReactiveProperty<int>();

    public ScrollRect scrollRect { get; set; }

    public void ResetListView(int idx = 0)
    {
        var target = scrollRect.content.GetChild(idx).GetComponent<RectTransform>();
        SnapTo(target, true);
        
        ItemIdx.Value = idx;
    }

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void Update()
    {
        if (StaticUtils.IsEndTouchScreen())
        {
            var nearestItem = FindTheNearestItem(out var idx);
            SnapTo(nearestItem, false);

            ItemIdx.Value = idx;
        }
    }

    private RectTransform FindTheNearestItem(out int idx)
    {
        Transform minItem = null;
        var minDistance = float.MaxValue;
        idx = -1;

        for (var i = 0; i < scrollRect.content.childCount; i++)
        {
            var child = scrollRect.content.GetChild(i);
            var distance = (child.position - scrollRect.viewport.position).sqrMagnitude;
            if (distance < minDistance)
            {
                minItem = child;
                idx = i;
                minDistance = distance;
            }
        }
        
        return minItem.GetComponent<RectTransform>();
    }

    private void SnapTo(RectTransform target, bool instantly)
    {
        var viewportSz = scrollRect.viewport.rect.size;

        if (scrollRect.horizontal)
        {
            const int minX = 0;
            var maxX = scrollRect.content.rect.width - viewportSz.x;
            var x = target.anchoredPosition.x - viewportSz.x / 2;

            var pos = (x - minX) / (maxX - minX);
            pos = Mathf.Clamp(pos, 0, 1);

            if (instantly)
            {
                scrollRect.horizontalNormalizedPosition = pos;
            }
            else
            {
                scrollRect.DOHorizontalNormalizedPos(pos, snappingTime).SetEase(snappingEase);
            }
        }
        else
        {
            const int minY = 0;
            var maxY = scrollRect.content.rect.height - viewportSz.y;
            
            var y = -target.anchoredPosition.y - viewportSz.y / 2;
            
            var pos = (y - minY) / (maxY - minY);
            pos = Mathf.Clamp(pos, 0, 1);

            pos = 1 - pos; //in vertical orientation, start position=1 and end position=0

            if (instantly)
            {
                scrollRect.verticalNormalizedPosition = pos;
            }
            else
            {
                scrollRect.DOVerticalNormalizedPos(pos, snappingTime).SetEase(snappingEase);
            }
        }
    }
}

using UnityEngine.EventSystems;
using UnityEngine;

public class DraggableUI : MonoBehaviour, IDragHandler
{
    public RectTransform target;
    public bool limitX;
    public bool limitY;

    public void OnDrag(PointerEventData eventData)
    {
        var scale = target.lossyScale;
        var delta = eventData.delta / scale;
        var pos = target.anchoredPosition;
        if (!limitX)
        {
            pos.x += delta.x;
        }
        if (!limitY)
        {
            pos.y += delta.y;
        }

        target.anchoredPosition = pos;
    }
}
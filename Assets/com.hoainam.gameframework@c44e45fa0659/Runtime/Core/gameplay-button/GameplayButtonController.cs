using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameplayButtonController : SingletonMonoBehaviour<GameplayButtonController>
{
    public List<string> uiTagsCanGoThrough;
    
    public Rect? limitedTouchRegion { get; set; }

    private Vector2 touchStartPosition;
    private Camera mainCamera;
    private EventSystem eventSystem;

    private void Start()
    {
        mainCamera = Camera.main;
        eventSystem = EventSystem.current;
    }

    private void Update()
    {
        if (StaticUtils.IsBeginTouchScreen())
        {
            touchStartPosition = StaticUtils.GetTouchPosition();
        }

        if (StaticUtils.IsEndTouchScreen())
        {
            var pos = (Vector2)StaticUtils.GetTouchPosition();
            if (StaticUtils.IsClickOnScreen(pos, touchStartPosition))
            {
                if (AllowToClick(pos, out var worldPos))
                {
                    OnClickOnScreen(worldPos);
                }
            }
        }
    }

    private bool AllowToClick(Vector2 screenPos, out Vector2 worldPos)
    {
        worldPos = mainCamera.ScreenToWorldPoint(screenPos);

        if (StaticUtils.IsClickOnUI(eventSystem, screenPos, uiTagsCanGoThrough))
        {
            return false;
        }

        return limitedTouchRegion == null || limitedTouchRegion.Value.Contains(worldPos);
    }

    private void OnClickOnScreen(Vector2 worldPos)
    {
        var mask = LayerMask.GetMask("GameplayButton");
        var hit = Physics2D.Raycast(worldPos, Vector2.zero, Mathf.Infinity, mask);
        if (hit.collider)
        {
            hit.collider.GetComponent<GameplayButton>().OnClicked();
        }
    }
}
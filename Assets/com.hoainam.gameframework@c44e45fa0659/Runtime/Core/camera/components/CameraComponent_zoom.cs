using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraComponent_zoom : BaseCameraComponent
{
    #region core

    private float initialZoom;
    private CameraZoomConfig config;
    private Vector2? limitZoom;
    private EventSystem eventSystem;

    public float CameraZoom
    {
        get => CameraController.instance.mainCamera.orthographicSize;
        set => CameraController.instance.mainCamera.orthographicSize = value;
    }

    public CameraComponent_zoom(CameraZoomConfig config)
    {
        this.config = config;
        initialZoom = CameraZoom;
        eventSystem = EventSystem.current;
    }

    public override void Update()
    {
        base.Update();
        
        ZoomOnEditor();
        ZoomOnMobile();
    }

    #endregion

    #region private utils

    private void ZoomOnEditor()
    {
        var amount = StaticUtils.GetMouseScrollDelta() * config.scrollSpeed;
        CameraZoom = ClampCameraZoom(CameraZoom - amount);
    }

    private void ZoomOnMobile()
    {
        if (StaticUtils.GetTouchCount() != 2)
        {
            return;
        }

        var clickedOnUI = StaticUtils.IsClickOnUI(eventSystem, StaticUtils.GetTouchPosition());
        if (clickedOnUI)
        {
            return;
        }

        var amount = StaticUtils.GetPinchToZoomIntensity() * config.pinchToZoomSpeed;
        CameraZoom = ClampCameraZoom(CameraZoom + amount);
    }

    private float ClampCameraZoom(float cameraZoom)
    {
        return limitZoom == null ? cameraZoom : Mathf.Clamp(cameraZoom, limitZoom.Value.x, limitZoom.Value.y);
    }

    #endregion

    #region public functions

    public void ZoomTo(float zoom, float duration = -1)
    {
        if (duration > 0)
        {
            DOVirtual.Float(CameraZoom, zoom, duration, x => CameraZoom = x);
        }
        else
        {
            CameraZoom = zoom;
        }
    }
    
    public void ResetZoom(float duration = -1)
    {
        ZoomTo(initialZoom, duration);
    }

    public void SetLimitZoom(Vector2 limitZoom)
    {
        this.limitZoom = limitZoom;
    }

    #endregion
}
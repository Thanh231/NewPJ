using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraComponent_pan : BaseCameraComponent
{
    #region core

    private CameraPanConfig config;
    private Vector3 initialCamPos;
    private Vector3? dragOrigin;
    private Vector3 inertiaVelocity;
    private int lastTouchCount;
    private Rect? viewport;
    private bool clickedOnUI;
    private EventSystem eventSystem;
    
    public Vector3 CameraPosition
    {
        get => CameraController.instance.mainCamera.transform.position;
        set => CameraController.instance.mainCamera.transform.position = value;
    }

    public CameraComponent_pan(CameraPanConfig config)
    {
        this.config = config;
        initialCamPos = CameraPosition;
        eventSystem = EventSystem.current;
    }

    public override void Update()
    {
        base.Update();

        var touchCount = StaticUtils.GetTouchCount(config.panWithMouse);
        var isTouching = StaticUtils.IsTouchingScreen(config.panWithMouse);

        HandleCameraPan(touchCount, isTouching);
        HandleCameraInertiaMove(isTouching);

        lastTouchCount = touchCount;
    }

    #endregion

    #region private utils

    private void HandleCameraPan(int touchCount, bool isTouching)
    {
        //click screen
        if (StaticUtils.IsBeginTouchScreen(config.panWithMouse) ||
            (lastTouchCount > 1 && touchCount == 1))
        {
            clickedOnUI = StaticUtils.IsClickOnUI(eventSystem, StaticUtils.GetTouchPosition());
            dragOrigin = GetTouchPosInWorld();
            inertiaVelocity = Vector3.zero;
        }
        
        //drag screen
        if (isTouching && dragOrigin != null)
        {
            if (touchCount == 1 && !clickedOnUI)
            {
                var dt = dragOrigin.Value - GetTouchPosInWorld();
                CameraPosition = ClampCameraPos(CameraPosition + dt);
                inertiaVelocity = dt / Time.deltaTime;
            }
            else
            {
                inertiaVelocity = Vector3.zero;
            }
        }
    }

    private void HandleCameraInertiaMove(bool isTouching)
    {
        if (!config.usingInertia)
        {
            return;
        }

        if (isTouching)
        {
            return;
        }

        var moveAmount = inertiaVelocity * Time.deltaTime;
        CameraPosition = ClampCameraPos(CameraPosition + moveAmount);
        inertiaVelocity = Vector3.Lerp(inertiaVelocity, Vector3.zero, config.inertiaDamping * Time.deltaTime);
    }

    private Vector3 GetTouchPosInWorld()
    {
        return CameraController.instance.mainCamera.ScreenToWorldPoint(StaticUtils.GetTouchPosition());
    }

    private Vector3 ClampCameraPos(Vector3 pos)
    {
        if (viewport == null)
        {
            return pos;
        }

        var halfCameraHeight = CameraController.instance.mainCamera.orthographicSize;
        var halfCameraWidth = halfCameraHeight * ((float)Screen.width / Screen.height);

        var minX = viewport.Value.x - viewport.Value.width / 2 + halfCameraWidth;
        var maxX = viewport.Value.x + viewport.Value.width / 2 - halfCameraWidth;
        var minY = viewport.Value.y - viewport.Value.height / 2 + halfCameraHeight;
        var maxY = viewport.Value.y + viewport.Value.height / 2 - halfCameraHeight;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        return pos;
    }

    #endregion

    #region public functions

    public void PanTo(Vector3 pos, float duration = -1)
    {
        pos.z = CameraPosition.z;

        if (duration > 0)
        {
            DOVirtual.Vector3(CameraPosition, pos, duration, x => { CameraPosition = x; });
        }
        else
        {
            CameraPosition = pos;
        }
    }

    public void ResetPan(float duration = -1)
    {
        PanTo(initialCamPos, duration);
    }

    public void SetLimit(Rect viewport)
    {
        this.viewport = viewport;
    }

    #endregion
}
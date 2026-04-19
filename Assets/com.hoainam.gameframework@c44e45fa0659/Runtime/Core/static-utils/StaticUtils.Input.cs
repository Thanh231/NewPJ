using UnityEngine;

public static partial class StaticUtils
{
    #region detect click/drag behaviour
    
    private const float STANDARD_THRESHOLD = 3f;
    private const float STANDARD_DPI = 120f;
    
    private static float? _clickThreshold;
    private static float clickThreshold
    {
        get
        {
            if (_clickThreshold == null)
            {
                _clickThreshold = STANDARD_THRESHOLD;
                if (Screen.dpi > 0)
                {
                    _clickThreshold = Screen.dpi / STANDARD_DPI * STANDARD_THRESHOLD;
                }
                Debug.Log($"DPI={Screen.dpi} threshold={_clickThreshold.Value}");
            }
            return _clickThreshold.Value;
        }
    }

    public static bool IsClickOnScreen(Vector2 lastScreenPos, Vector2 currentScreenPos)
    {
        return (currentScreenPos - lastScreenPos).sqrMagnitude <= clickThreshold * clickThreshold;
    }

    #endregion
    
    #region mouse

    public static float GetMouseScrollDelta()
    {
        return Input.mouseScrollDelta.y;
    }

    #endregion
    
    #region single touch
    
    public static bool IsBeginTouchScreen(MouseButtonType type)
    {
        return Input.GetMouseButtonDown((int)type);
    }

    public static bool IsTouchingScreen(MouseButtonType type)
    {
        return Input.GetMouseButton((int)type);
    }

    public static bool IsEndTouchScreen(MouseButtonType type)
    {
        return Input.GetMouseButtonUp((int)type);
    }

    public static bool IsBeginTouchScreen()
    {
        return IsBeginTouchScreen(MouseButtonType.LeftMouse);
    }

    public static bool IsTouchingScreen()
    {
        return IsTouchingScreen(MouseButtonType.LeftMouse);
    }

    public static bool IsEndTouchScreen()
    {
        return IsEndTouchScreen(MouseButtonType.LeftMouse);
    }

    public static Vector3 GetTouchPosition()
    {
        return Input.mousePosition;
    }

    #endregion

    #region multi-touch

    public static int GetTouchCount(MouseButtonType type)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return IsTouchingScreen(type) ? 1 : 0;
#else
        return Input.touchCount;
#endif
    }
    
    public static int GetTouchCount()
    {
        return GetTouchCount(MouseButtonType.LeftMouse);
    }

    public static float GetCurrentTouchesDistance()
    {
        var touch1 = Input.GetTouch(0);
        var touch2 = Input.GetTouch(1);
        return Vector2.Distance(touch1.position, touch2.position);
    }

    public static float GetPreviousTouchesDistance()
    {
        var touch1 = Input.GetTouch(0);
        var touch2 = Input.GetTouch(1);
        var prevPos1 = touch1.position - touch1.deltaPosition;
        var prevPos2 = touch2.position - touch2.deltaPosition;
        return Vector2.Distance(prevPos1, prevPos2);
    }

    public static float GetPinchToZoomIntensity()
    {
        return GetPreviousTouchesDistance() - GetCurrentTouchesDistance();
    }

    #endregion

    #region key

    public static bool IsPressBackKey()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }

    public static bool IsPressCombinationCtrl(KeyCode keyCode)
    {
        return (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
            Input.GetKeyDown(keyCode);
    }

    public static bool IsPressCombinationAlt(KeyCode keyCode)
    {
        return (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) &&
            Input.GetKeyDown(keyCode);
    }

    public static bool IsPressCombinationShift(KeyCode keyCode)
    {
        return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) &&
            Input.GetKeyDown(keyCode);
    }

    #endregion
}
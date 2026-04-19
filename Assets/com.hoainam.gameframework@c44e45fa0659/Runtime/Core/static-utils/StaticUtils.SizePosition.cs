using UnityEngine;
using UnityEngine.UI;

public static partial class StaticUtils
{
    #region screen size

    public static float ScreenToWorldDistance(float screenDist, Camera camera)
    {
        var screenHeight = (float)Screen.height;
        var worldHeight = camera.orthographicSize * 2;
        var ratio = worldHeight / screenHeight;
        return screenDist * ratio;
    }

    public static Vector2 GetScreenSizeInWorld(Camera camera)
    {
        var worldWidth = ScreenToWorldDistance(Screen.width, camera);
        var worldHeight = ScreenToWorldDistance(Screen.height, camera);
        return new Vector2(worldWidth, worldHeight);
    }

    public static Vector2 GetSafeScreenSizeInWorld(Camera camera)
    {
        var worldWidth = ScreenToWorldDistance(Screen.safeArea.size.x, camera);
        var worldHeight = ScreenToWorldDistance(Screen.safeArea.size.y, camera);
        return new Vector2(worldWidth, worldHeight);
    }

    public static float GetSafeTopOffset()
    {
        return Screen.height - Screen.safeArea.y - Screen.safeArea.height;
    }

    public static float GetSafeBottomOffset()
    {
        return Screen.safeArea.y;
    }

    public static float GetSafeTopOffsetInWorld(Camera camera)
    {
        return ScreenToWorldDistance(GetSafeTopOffset(), camera);
    }
    
    public static float GetSafeBottomOffsetInWorld(Camera camera)
    {
        return ScreenToWorldDistance(GetSafeBottomOffset(), camera);
    }

    #endregion

    #region fullscreen background

    public static void ScaleBackgroundFullscreen_UI(Image target)
    {
        var rect = target.GetComponent<RectTransform>();
        var texW = target.sprite.texture.width;
        var texH = target.sprite.texture.height;

        var ratioScreen = (float)Screen.width / Screen.height;
        var ratioTex = (float)texW / texH;

        Vector2 sz;

        if (ratioTex > ratioScreen)
        {
            sz = new Vector2(ratioTex * Screen.height, Screen.height);
        }
        else
        {
            sz = new Vector2(Screen.width, Screen.width / ratioTex);
        }

        var scale = target.transform.lossyScale;
        rect.sizeDelta = new Vector2(sz.x / scale.x, sz.y / scale.y);
    }

    #endregion

    #region RectTransform stretch

    public static void StretchRectTransform_all(RectTransform rt, float top=0, float down=0, float left=0, float right=0)
    {
        rt.anchorMin = new Vector2(0,0);
        rt.anchorMax = new Vector2(1,1);
        rt.offsetMin = new Vector2(left, down);
        rt.offsetMax = new Vector2(-right, -top);
    }

    public static void StretchRectTransform_top(RectTransform rt, float height)
    {
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(0, -height);
        rt.offsetMax = new Vector2(0, 0);
    }

    public static void StretchRectTransform_down(RectTransform rt, float height)
    {
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.offsetMin = new Vector2(0, 0);
        rt.offsetMax = new Vector2(0, height);
    }

    public static void StretchRectTransform_left(RectTransform rt, float width)
    {
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 1);
        rt.offsetMin = new Vector2(0, 0);
        rt.offsetMax = new Vector2(width, 0);
    }

    public static void StretchRectTransform_right(RectTransform rt, float width)
    {
        rt.anchorMin = new Vector2(1, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(-width, 0);
        rt.offsetMax = new Vector2(0, 0);
    }

    #endregion
}
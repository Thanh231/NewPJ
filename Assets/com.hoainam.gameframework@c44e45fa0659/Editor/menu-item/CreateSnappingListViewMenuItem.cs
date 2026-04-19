
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class CreateSnappingListViewMenuItem
{
    [MenuItem("GameObject/UI (Canvas)/\u2726\u2726MY VIEWS\u2726\u2726/Snapping List View - Horizontal")]
    private static void CreateHorizontal(MenuCommand menuCommand)
    {
        Create(menuCommand, false);
    }

    [MenuItem("GameObject/UI (Canvas)/\u2726\u2726MY VIEWS\u2726\u2726/Snapping List View - Vertical")]
    private static void CreateVertical(MenuCommand menuCommand)
    {
        Create(menuCommand, true);
    }

    private static void Create(MenuCommand menuCommand, bool isVertical)
    {
        var parent = menuCommand.context as GameObject;

        var goScrollRect = CreateGO_scrollRect(parent, isVertical);
        var goViewport = CreateGO_viewport(goScrollRect.gameObject);
        var goContent = CreateGO_content(goViewport.gameObject, isVertical);

        goScrollRect.gameObject.AddComponent<SnappingListView>();

        goScrollRect.viewport = goViewport;
        goScrollRect.content = goContent;

        Selection.activeObject = goScrollRect.gameObject;
    }

    private static ScrollRect CreateGO_scrollRect(GameObject parent, bool isVertical)
    {
        //create game object
        var scroll = StaticUtilsEditor.CreateGameObject<ScrollRect>(parent, "snapping-scrollview");

        //add component
        var img = scroll.gameObject.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0f);

        //configure scroll
        scroll.horizontal = !isVertical;
        scroll.vertical = isVertical;

        //return
        return scroll;
    }

    private static RectTransform CreateGO_viewport(GameObject parent)
    {
        var rect = StaticUtilsEditor.CreateGameObject<RectTransform>(parent, "viewport");
        rect.gameObject.AddComponent<RectMask2D>();
        StaticUtils.StretchRectTransform_all(rect);
        return rect;
    }

    private static RectTransform CreateGO_content(GameObject parent, bool isVertical)
    {
        var rect = StaticUtilsEditor.CreateGameObject<RectTransform>(parent, "content");
        if (isVertical)
        {
            StaticUtils.StretchRectTransform_top(rect, 100);
            rect.pivot = new Vector2(0.5f, 1);
        }
        else
        {
            StaticUtils.StretchRectTransform_left(rect, 100);
            rect.pivot = new Vector2(0, 0.5f);
        }
        return rect;
    }
}
using UnityEditor;
using UnityEngine;

public static class CreateTabMenuItem
{
    [MenuItem("GameObject/UI (Canvas)/\u2726\u2726MY VIEWS\u2726\u2726/Tab - Go down")]
    private static void CreateGoDown(MenuCommand menuCommand)
    {
        OpenPopupChooseNumberOfTabs(TabDirection.GoDown, menuCommand.context as GameObject);
    }

    [MenuItem("GameObject/UI (Canvas)/\u2726\u2726MY VIEWS\u2726\u2726/Tab - Go up")]
    private static void CreateGoUp(MenuCommand menuCommand)
    {
        OpenPopupChooseNumberOfTabs(TabDirection.GoUp, menuCommand.context as GameObject);
    }

    [MenuItem("GameObject/UI (Canvas)/\u2726\u2726MY VIEWS\u2726\u2726/Tab - Go right")]
    private static void CreateGoRight(MenuCommand menuCommand)
    {
        OpenPopupChooseNumberOfTabs(TabDirection.GoRight, menuCommand.context as GameObject);
    }

    [MenuItem("GameObject/UI (Canvas)/\u2726\u2726MY VIEWS\u2726\u2726/Tab - Go left")]
    private static void CreateGoLeft(MenuCommand menuCommand)
    {
        OpenPopupChooseNumberOfTabs(TabDirection.GoLeft, menuCommand.context as GameObject);
    }

    private static void OpenPopupChooseNumberOfTabs(TabDirection tabDirection, GameObject parentTab)
    {
        var wnd = EditorWindow.GetWindow<ChooseNumberOfTabsWindow>();
        wnd.titleContent = new GUIContent("choose number of tabs");
        wnd.tabDirection = tabDirection;
        wnd.parentTab = parentTab;
        wnd.Show();
    }
}
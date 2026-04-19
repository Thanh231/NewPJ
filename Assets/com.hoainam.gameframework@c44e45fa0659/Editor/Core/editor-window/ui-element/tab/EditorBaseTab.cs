
using System.Collections.Generic;
using UnityEngine;

public class EditorBaseTab
{
    public abstract class TabItem
    {
        public abstract string tabText { get; }
        public abstract void OnDraw();
        public virtual void OnBeginTab() { }
        public virtual void OnWindowClosed() { }
    }

    private List<TabItem> listTabItems;
    private string[] listTabTexts;
    private int currentTabId;
    private int previousTabId = -1;

    public EditorBaseTab(List<TabItem> listTabItems)
    {
        this.listTabItems = listTabItems;
        listTabTexts = new string[listTabItems.Count];
        for (var i = 0; i < listTabTexts.Length; i++)
        {
            listTabTexts[i] = listTabItems[i].tabText;
        }
    }

    public void Draw()
    {
        currentTabId = GUILayout.Toolbar(currentTabId, listTabTexts);

        if (previousTabId != currentTabId)
        {
            listTabItems[currentTabId].OnBeginTab();
            previousTabId = currentTabId;
        }

        listTabItems[currentTabId].OnDraw();
    }
}

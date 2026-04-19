
using System.Collections.Generic;

public class EditorTabInWindow : EditorBaseTab
{
    public abstract class TabItemWindow : TabItem
    {
        public EditorWindowStateMachine FSM;
    }

    public EditorTabInWindow(List<TabItem> listTabItems, EditorWindowStateMachine FSM)
        : base(listTabItems)
    {
        foreach (var i in listTabItems)
        {
            ((TabItemWindow)i).FSM = FSM;
        }
    }
}

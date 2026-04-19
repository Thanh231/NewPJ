
using System.Collections.Generic;

public class EditorWindowState_tab : EditorWindowState
{
	private EditorBaseTab tabView;
	private List<EditorBaseTab.TabItem> tabItems;

	public EditorWindowState_tab(List<EditorBaseTab.TabItem> tabItems)
	{
		this.tabItems = tabItems;
	}

	public override void OnBeginState()
	{
		base.OnBeginState();

		//init tabView here in order for FSM valid
		tabView = new EditorTabInWindow(tabItems, FSM);
	}

	public override void OnDraw()
	{
		tabView.Draw();
	}

	public override void OnWindowClosed()
	{
		base.OnWindowClosed();

		foreach (var i in tabItems)
		{
			i.OnWindowClosed();
		}
	}
}
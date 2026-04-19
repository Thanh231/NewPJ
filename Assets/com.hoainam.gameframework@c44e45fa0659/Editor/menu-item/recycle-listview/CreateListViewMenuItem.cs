
using UnityEditor;
using UnityEngine;

public class CreateListViewMenuItem
{
	[MenuItem("GameObject/UI (Canvas)/\u2726\u2726MY VIEWS\u2726\u2726/Recycle List View - Horizontal")]
	static void CreateListView_horizontal(MenuCommand menuCommand)
	{
		CreateListView(menuCommand, isVertical: false);
	}

	[MenuItem("GameObject/UI (Canvas)/\u2726\u2726MY VIEWS\u2726\u2726/Recycle List View - Vertical")]
	static void CreateListView_vertical(MenuCommand menuCommand)
	{
		CreateListView(menuCommand, isVertical: true);
	}

	static void CreateListView(MenuCommand menuCommand, bool isVertical)
	{
		var parent = menuCommand.context as GameObject;
		var rect = CreateRecycleViewUtils.CreateScrollView(parent, isVertical);
		rect.gameObject.AddComponent<RecycleListView>();
	}
}
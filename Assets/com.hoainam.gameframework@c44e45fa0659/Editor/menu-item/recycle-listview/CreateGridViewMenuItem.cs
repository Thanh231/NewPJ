
using UnityEditor;
using UnityEngine;

public class CreateGridViewMenuItem
{
	[MenuItem("GameObject/UI (Canvas)/\u2726\u2726MY VIEWS\u2726\u2726/Recycle Grid View - Horizontal")]
	static void CreateListView_horizontal(MenuCommand menuCommand)
	{
		CreateGridView(menuCommand, isVertical: false);
	}

	[MenuItem("GameObject/UI (Canvas)/\u2726\u2726MY VIEWS\u2726\u2726/Recycle Grid View - Vertical")]
	static void CreateListView_vertical(MenuCommand menuCommand)
	{
		CreateGridView(menuCommand, isVertical: true);
	}

	static void CreateGridView(MenuCommand menuCommand, bool isVertical)
	{
		var parent = menuCommand.context as GameObject;
		var rect = CreateRecycleViewUtils.CreateScrollView(parent, isVertical);
		rect.gameObject.AddComponent<RecycleGridView>();
	}
}
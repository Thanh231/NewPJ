public class EditorWindowState_doing : EditorWindowState
{
	const int fontSize = 60;

	public override void OnDraw()
	{
		var text = GetText();
		var windowSize = FSM.position;
		EditorGUILayoutExtension.CenterLabelField(text, windowSize.width, windowSize.height, fontSize);
	}

	protected virtual string GetText()
	{
		return "doing";
	}
}
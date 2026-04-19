
using UnityEditor;
using UnityEngine;

public partial class ImageToolTab_crop : EditorTabInWindow.TabItemWindow
{
    private Texture2D texture;
    private Vector2? selectRectCorner_1;
    private Vector2? selectRectCorner_2;
    private bool isDraggingMouse;
	
    const float tabButtonsHeight = 25;
    const float chooseImageFieldHeight = 100;
    const float applyButtonHeight = 30;
    const float textureMargin = 30;
    
    public override string tabText => "crop";
    public override void OnDraw()
    {
        //handle input
        HandleMouseEvents(Event.current);

        //draw texture field
        var accumulatedY = tabButtonsHeight;
        var fieldRect = new Rect(0, accumulatedY, FSM.position.width, chooseImageFieldHeight);
        accumulatedY += chooseImageFieldHeight;
        texture = (Texture2D)EditorGUI.ObjectField(fieldRect, "", texture, typeof(Texture2D), false);

        //draw apply button
        var btnRect = new Rect(0, accumulatedY, FSM.position.width, applyButtonHeight);
        accumulatedY += applyButtonHeight;
        if (GUI.Button(btnRect, "apply"))
        {
            OnApplyButtonClicked();
        }

        //draw preview texture
        if (texture)
        {
            accumulatedY += textureMargin;
            var textureRect = new Rect(textureMargin, accumulatedY,
                FSM.position.width - 2 * textureMargin, FSM.position.height - accumulatedY - textureMargin);
            EditorGUI.DrawTextureTransparent(textureRect, texture, ScaleMode.ScaleToFit);
        }

        //draw selected area
        if (selectRectCorner_1 != null && selectRectCorner_2 != null)
        {
            EditorGUILayoutExtension.DrawRectangle(selectRectCorner_1.Value, selectRectCorner_2.Value, Color.red, 1);
        }
    }
    
    private void OnApplyButtonClicked()
    {
        GetCropInfo(out bool isHorizontal, out float min, out float max);
        CropTexture(isHorizontal, min, max);
        selectRectCorner_1 = null;
        selectRectCorner_2 = null;
    }
}

using UnityEditor;
using UnityEngine;

public partial class ImageToolTab_crop
{
    #region handling input

	private void HandleMouseEvents(Event e)
	{
		if (!texture)
		{
			return;
		}

		var uiRect = new Rect(0, 0, FSM.position.width, tabButtonsHeight + chooseImageFieldHeight + applyButtonHeight);
		if (uiRect.Contains(e.mousePosition))
		{
			return;
		}

		if (e.type == EventType.MouseDown)
		{
			isDraggingMouse = true;
			selectRectCorner_1 = WrapSelectRectCorner(e.mousePosition);
		}

		if (e.type == EventType.MouseUp && isDraggingMouse)
		{
			isDraggingMouse = false;
		}

		if (isDraggingMouse)
		{
			selectRectCorner_2 = WrapSelectRectCorner(e.mousePosition);
			FSM.Repaint();
		}
	}

	private Vector2 WrapSelectRectCorner(Vector2 pos)
	{
		GetImageCorner(out float minX, out float maxX, out float minY, out float maxY);
		var x = Mathf.Clamp(pos.x, minX, maxX);
		var y = Mathf.Clamp(pos.y, minY, maxY);
		return new Vector2(x, y);
	}

	private void GetImageCorner(out float minX, out float maxX, out float minY, out float maxY)
	{
		minX = textureMargin;
		maxX = FSM.position.width - textureMargin;
		minY = tabButtonsHeight + chooseImageFieldHeight + applyButtonHeight + textureMargin;
		maxY = FSM.position.height - textureMargin;
		var width = maxX - minX;
		var height = maxY - minY;

		var textureAspectRatio = (float)texture.width / texture.height;
		var aspectRatio = width / height;

		if (textureAspectRatio < aspectRatio)
		{
			var newWidth = height * textureAspectRatio;
			minX += (width - newWidth) / 2;
			maxX = minX + newWidth;
		}
		else
		{
			var newHeight = width / textureAspectRatio;
			minY += (height - newHeight) / 2;
			maxY = minY + newHeight;
		}
	}

	#endregion

	#region crop image

	private void CropTexture(bool isHorizontal, float min, float max)
	{
		var path = AssetDatabase.GetAssetPath(texture);
		var needRevertIsReadable = ImageToolWindow.SetImageIsReadable(texture, true);
		var newTexture = ImageToolWindow.CropImage(texture, isHorizontal, min, max);
		ImageToolWindow.SaveTexture(newTexture, path, true);
		if (needRevertIsReadable)
		{
			ImageToolWindow.SetImageIsReadable(texture, false);
		}
	}
	
	private void GetCropInfo(out bool isHorizontal, out float min, out float max)
	{
		GetImageCorner(out float minX, out float maxX, out float minY, out float maxY);

		var width = maxX - minX;
		var height = maxY - minY;

		var minXSelectCorner = Mathf.Min(selectRectCorner_1.Value.x, selectRectCorner_2.Value.x);
		var maxXSelectCorner = Mathf.Max(selectRectCorner_1.Value.x, selectRectCorner_2.Value.x);
		var minYSelectCorner = Mathf.Min(selectRectCorner_1.Value.y, selectRectCorner_2.Value.y);
		var maxYSelectCorner = Mathf.Max(selectRectCorner_1.Value.y, selectRectCorner_2.Value.y);

		isHorizontal = minXSelectCorner == minX && maxXSelectCorner == maxX;
		if (isHorizontal)
		{
			min = (minYSelectCorner - minY) / height;
			max = (maxYSelectCorner - minY) / height;
		}
		else
		{
			min = (minXSelectCorner - minX) / width;
			max = (maxXSelectCorner - minX) / width;
		}
	}
	
	#endregion
}

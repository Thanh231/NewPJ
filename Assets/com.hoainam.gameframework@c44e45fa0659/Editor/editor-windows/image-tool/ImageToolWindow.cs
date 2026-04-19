
using System.IO;
using UnityEditor;
using UnityEngine;

public class ImageToolWindow:EditorWindowStateMachine
{
    [MenuItem("\u2726\u2726TOOLS\u2726\u2726/image tool")]
    static void OnMenuClicked()
    {
        OpenWindow<ImageToolWindow>(new ImageToolStateDefault());
    }

    #region utils

    public static Texture2D ResizeImage(Texture2D texture, int newWidth, int newHeight)
    {
        var newTexture = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, mipChain: false);
        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                float u = (float)x / (newWidth - 1);
                float v = (float)y / (newHeight - 1);
                
                Color pixelColor = texture.GetPixelBilinear(u, v);
                newTexture.SetPixel(x, y, pixelColor);
            }
        }
        newTexture.Apply();
        return newTexture;
    }

    public static Texture2D CropImage(Texture2D texture, bool isHorizontal, float min, float max)
    {
	    //Texture2D has origin at lower left corner
	    //while "min", "max" has origin at upper left corner
	    //so need to convert min max
	    if (isHorizontal)
	    {
		    var tMin = 1 - max;
		    var tMax = 1 - min;

		    min = tMin;
		    max = tMax;
	    }

	    //do crop
	    var width = texture.width;
	    var height = texture.height;
	    var minWidth = 0;
	    var maxWidth = 0;
	    var minHeight = 0;
	    var maxHeight = 0;
	    if (isHorizontal)
	    {
		    minHeight = (int)(min * height);
		    maxHeight = (int)(max * height);
		    height -= maxHeight - minHeight + 1;
	    }
	    else
	    {
		    minWidth = (int)(min * width);
		    maxWidth = (int)(max * width);
		    width -= maxWidth - minWidth + 1;
	    }

	    var newTexture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false);
	    var newX = 0;
	    var newY = 0;
	    for (var y = 0; y < texture.height; y++)
	    {
		    bool needIncreaseY = true;
		    for (var x = 0; x < texture.width; x++)
		    {
			    var pixelColor = texture.GetPixel(x, y);
			    if (isHorizontal)
			    {
				    if (y < minHeight || y > maxHeight)
				    {
					    newTexture.SetPixel(newX, newY, pixelColor);
					    newX++;
				    }
				    else
				    {
					    needIncreaseY = false;
				    }
			    }
			    else
			    {
				    if (x < minWidth || x > maxWidth)
				    {
					    newTexture.SetPixel(newX, newY, pixelColor);
					    newX++;
				    }
			    }
		    }

		    newX = 0;
		    if (needIncreaseY)
		    {
			    newY++;
		    }
	    }

	    newTexture.Apply();
	    return newTexture;
    }

    public static void SaveTexture(Texture2D texture, string path, bool refreshDBToo)
    {
        var pngData = texture.EncodeToPNG();
        File.WriteAllBytes(path, pngData);
        AssetDatabase.ImportAsset(path);
        if (refreshDBToo)
        {
            AssetDatabase.Refresh();
        }
    }

    public static bool SetImageIsReadable(Texture2D texture, bool isReadable)
    {
        var path = AssetDatabase.GetAssetPath(texture);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer.isReadable != isReadable)
        {
            importer.isReadable = isReadable;
            importer.SaveAndReimport();
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
}

using UnityEngine;

//ETC2 format requires the input image to be a multiple of 4 in both dimensions.
public class ImageToolTab_resizeTo4x : BaseTab_batchResize
{
    public override string tabText => "resize to 4x";
    protected override Vector2Int GetNewSize(Texture2D texture)
    {
        // original image has some sub-image inside, like
        // x=0, y=0, width=originalW, height=originalH
        // so new image must have bigger size than original image,
        // otherwise the sub-image will span outside the new image and cause error.
        
        var newWidth = RoundUpToMultipleOf4(texture.width);
        var newHeight = RoundUpToMultipleOf4(texture.height);
        return new Vector2Int(newWidth, newHeight);
    }
    
    public static int RoundUpToMultipleOf4(int value)
    {
        return (value + 3) & ~3;
    }
}

using System.Collections.Generic;

public class ImageToolStateDefault:EditorWindowState_tab
{
    public ImageToolStateDefault()
    :base(new List<EditorBaseTab.TabItem>()
    {
        new ImageToolTab_resize(),
        new ImageToolTab_batchResize(),
        new ImageToolTab_resizeTo4x(),
        new ImageToolTab_crop()
    })
    {
    }
}

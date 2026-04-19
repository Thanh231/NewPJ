
using System.Collections.Generic;

public class EditorFilePicker : EditorFolderPicker
{
    private List<string> lExtension;

    public EditorFilePicker(List<string> lExtension, string lable)
        : this(lExtension, lable, null)
    {
    }

    public EditorFilePicker(List<string> lExtension, string lable, string persistentKey)
        : base(lable, persistentKey)
    {
        this.lExtension = lExtension;
    }

    protected override string OpenDialog()
    {
        return StaticUtilsEditor.DisplayChooseFileDialog("choose file", lExtension);
    }
}

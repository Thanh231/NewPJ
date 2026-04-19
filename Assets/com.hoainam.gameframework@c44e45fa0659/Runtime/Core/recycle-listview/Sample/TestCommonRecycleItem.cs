
using TMPro;

public class TestCommonRecycleItem : RecycleScrollViewItem
{
    public TextMeshProUGUI text;
    
    public override void SetData(object data)
    {
        var dataAsInt = (int)data;
        text.text = dataAsInt.ToString();
    }
}

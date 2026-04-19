using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TabController : MonoBehaviour
{
    public List<TabItemConfig> cfgItems;

    private void Start()
    {
        foreach (var item in cfgItems)
        {
            item.tabBtn.tabController = this;
        }
        
        SelectTab(cfgItems[0].tabBtn);
    }
    
    public void SelectTab(TabButtonController tabBtn)
    {
        foreach (var item in cfgItems)
        {
            var isSelected = item.tabBtn == tabBtn;
            item.tabBtn.SetSelected(isSelected);
            item.tabContent.SetActive(isSelected);
        }
    }
}

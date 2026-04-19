using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ChooseNumberOfTabsWindow : EditorWindow
{
    #region core

    private int numberOfTabs = 2;

    public TabDirection tabDirection {get;set;}
    public GameObject parentTab {get;set;}

    private bool isHorizontalTab => tabDirection == TabDirection.GoUp || tabDirection == TabDirection.GoDown;

    void OnGUI()
    {
        numberOfTabs = EditorGUILayoutExtension.FitIntField("number of tabs:", numberOfTabs);
        if (GUILayout.Button("create tab"))
        {
            CreateTab();
            Close();
        }
    }

    private void CreateTab()
    {
        var tab = CreateBasicStructure(out GameObject parentBtn, out GameObject parentContent);
        for (int i = 0; i < numberOfTabs; i++)
        {
            var tabBtn = CreateTabButton(i + 1, parentBtn);
            var tabContent = CreateTabContent(i + 1, parentContent);

            AddTabItem(tab, tabBtn, tabContent);
        }
    }

    #endregion

    #region basic structure

    private TabController CreateBasicStructure(out GameObject parentBtn, out GameObject parentContent)
    {
        var tab = StaticUtilsEditor.CreateGameObject<TabController>(parentTab, "tab");
        var rtParentBtn = StaticUtilsEditor.CreateUIGameObject(tab.gameObject, "tab-button");
        var rtParentContent = StaticUtilsEditor.CreateUIGameObject(tab.gameObject, "tab-content");

        ConfigureBasicStructure(tab.GetComponent<RectTransform>(), rtParentBtn, rtParentContent);

        parentBtn = rtParentBtn.gameObject;
        parentContent = rtParentContent.gameObject;

        return tab;
    }

    private void ConfigureBasicStructure(RectTransform rtTab, RectTransform rtButton, RectTransform rtContent)
    {
        const int tabHeight = 200;

        //tab size
        rtTab.sizeDelta = new Vector2(700, 700);

        //buttons & content parent size
        switch (tabDirection)
        {
            case TabDirection.GoDown:
            StaticUtils.StretchRectTransform_top(rtButton, tabHeight);
            StaticUtils.StretchRectTransform_all(rtContent, top:tabHeight);
            break;
            case TabDirection.GoUp:
            StaticUtils.StretchRectTransform_down(rtButton, tabHeight);
            StaticUtils.StretchRectTransform_all(rtContent, down:tabHeight);
            break;
            case TabDirection.GoRight:
            StaticUtils.StretchRectTransform_left(rtButton, tabHeight);
            StaticUtils.StretchRectTransform_all(rtContent, left:tabHeight);
            break;
            case TabDirection.GoLeft:
            StaticUtils.StretchRectTransform_right(rtButton, tabHeight);
            StaticUtils.StretchRectTransform_all(rtContent, right:tabHeight);
            break;
        }

        //buttons layout group
        HorizontalOrVerticalLayoutGroup layoutGroup = isHorizontalTab ?
            rtButton.gameObject.AddComponent<HorizontalLayoutGroup>() :
            rtButton.gameObject.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = true;
    }

    private void AddTabItem(TabController tab, TabButtonController tabBtn, GameObject tabContent)
    {
        if (tab.cfgItems == null)
        {
            tab.cfgItems = new List<TabItemConfig>();
        }

        tab.cfgItems.Add(new TabItemConfig()
        {
            tabBtn = tabBtn,
            tabContent = tabContent,
        });
    }

    #endregion

    #region tab buttons

    private TabButtonController CreateTabButton(int idx, GameObject parent)
    {
        var tabBtn = StaticUtilsEditor.CreateGameObject<TabButtonController>(parent, $"tab-btn-{idx}");

        tabBtn.btnSelect = ConfigureButton(tabBtn);
        tabBtn.objNormal = CreateButtonBackground("bg-normal", Color.white, tabBtn.gameObject);
        tabBtn.objSelected = CreateButtonBackground("bg-selected", Color.yellow, tabBtn.gameObject);
        CreateButtonLabel(tabBtn.gameObject, idx);

        return tabBtn;
    }

    private Button ConfigureButton(TabButtonController tabBtn)
    {
        var btn = tabBtn.GetComponent<Button>();
        btn.transition = Selectable.Transition.None;
        return btn;
    }

    private GameObject CreateButtonBackground(string name, Color color, GameObject parent)
    {
        var img = StaticUtilsEditor.CreateGameObject<Image>(parent, name);
        img.color = color;
        StaticUtils.StretchRectTransform_all(img.GetComponent<RectTransform>());
        return img.gameObject;
    }

    private void CreateButtonLabel(GameObject parent, int idx)
    {
        var txt = StaticUtilsEditor.CreateGameObject<TextMeshProUGUI>(parent, "txt-label");
        txt.text = $"tab {idx}";
        txt.color = Color.black;
        txt.alignment = TextAlignmentOptions.Center;
        StaticUtils.StretchRectTransform_all(txt.GetComponent<RectTransform>());
    }

    #endregion

    #region tab content

    private GameObject CreateTabContent(int idx, GameObject parent)
    {
        var content = StaticUtilsEditor.CreateUIGameObject(parent, $"tab-content-{idx}");
        StaticUtils.StretchRectTransform_all(content);
        return content.gameObject;
    }

    #endregion
}
using R3;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Button))]
public class TabButtonController : MonoBehaviour
{
    public Button btnSelect;
    public GameObject objNormal;
    public GameObject objSelected;

    public TabController tabController { get; set; }

    private void Start()
    {
        btnSelect.OnClickAsObservable().Subscribe(_ =>
        {
            tabController.SelectTab(this);
        });
    }

    public void SetSelected(bool selected)
    {
        objSelected.SetActive(selected);
        objNormal.SetActive(!selected);
    }
}

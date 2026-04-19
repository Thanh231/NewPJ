
//text will be: click <link="link_1"><color=#FFAA00><u>here</u></color></link> to continue
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TextAsLink : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI text;

    public UnityAction<string> OnLinkClicked;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var idx = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, null);
        if (idx < 0)
        {
            return;
        }

        var linkId = text.textInfo.linkInfo[idx].GetLinkID();
        OnLinkClicked?.Invoke(linkId);
    }
}

using UnityEngine.UI;
using R3;

public class LocalizedText_legacy : LocalizedText
{
    private Text _text;
    public Text text
    {
        get
        {
            if (_text == null)
            {
                _text = GetComponent<Text>();
            }
            return _text;
        }
    }

    public override void SetText(string txt)
    {
        text.text = txt;
    }

    protected override void Start()
    {
        base.Start();

        LocalizationController.instance.languageRx
            .Subscribe(_ => { LocalizationController.instance.SetupLocalizedText(this); })
            .AddTo(this);
    }
}

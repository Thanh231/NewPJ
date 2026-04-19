
using R3;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class LanguageNameButton : MonoBehaviour
{
    public TextMeshProUGUI txtLanguageName;
    public Image imgBackground;
    public Sprite spriteNormal;
    public Sprite spriteSelected;
    public Button btnSelect;
    
    private static LanguageInfoConfig _languageInfoConfig;
    private static LanguageInfoConfig languageInfoConfig
    {
        get
        {
            if (_languageInfoConfig == null)
            {
                _languageInfoConfig = new LanguageInfoConfig();
            }

            return _languageInfoConfig;
        }
    }

    public void SetData(ReactiveProperty<SystemLanguage> gameLanguage, SystemLanguage language,
        UnityAction<SystemLanguage> onLanguageChanged)
    {
        var cfg = languageInfoConfig.GetLanguageInfoItem(language);
        txtLanguageName.text = cfg.localLanguageName;

        gameLanguage.Select(x => x == language ? spriteSelected : spriteNormal)
            .Subscribe(x => imgBackground.sprite = x)
            .AddTo(this);

        btnSelect.OnClickAsObservable().Subscribe(async _ =>
        {
            await LocalizationController.instance.LoadLocalizationData(language);
            onLanguageChanged?.Invoke(language);
        });
    }
}

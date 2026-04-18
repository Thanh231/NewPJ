using Coffee.UIExtensions;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupWin : BasePopup
{
    [SerializeField] TextMeshProUGUI txtRewardAmount;
    [SerializeField] Button btnContinue;
    [SerializeField] TextMeshProUGUI txtRewardBonusAmount;
    [SerializeField] Button btnWatchAd;

    public ParticleSystem a;
    public ParticleSystem b;
    public ParticleSystem c;
    protected override void Start()
    {
        base.Start();
        // a.Play();
        // b.Play();
        // c.Play();
        // this.GetComponent<UIParticle>().Play();
        txtRewardAmount.text = $"{HardCodeInGame.REWARD_GOLD_WIN}";
        txtRewardBonusAmount.text = $"{HardCodeInGame.REWARD_GOLD_WIN * HardCodeInGame.BOUNE_REWARD_GOLD_MULTI}";

        btnContinue.OnClickAsObservable()
            .Subscribe(_ =>
            {
                CurrencyController.AddGold(HardCodeInGame.REWARD_GOLD_WIN);
                GameManager.Instance.StartGame();
                HapticController.PlayHaptic(HapticType.coin_animation);
                ClosePopup();
            }).AddTo(this);

        btnWatchAd.OnClickAsObservable()
            .Subscribe(_ =>
            {
                HapticController.PlayHaptic(HapticType.valid_button);
                Debug.Log("Add Logic: Watch ad to double reward");
            }).AddTo(this);
    }
}

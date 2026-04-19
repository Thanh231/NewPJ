
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using R3;
using Cysharp.Threading.Tasks;

public class PopupTermKorean : BasePopup
{
    public class ClosePopupResult
    {
        public bool checkedNotification;
        public bool checkedNightNotification;

        public ClosePopupResult(bool checkedNotification, bool checkedNightNotification)
        {
            this.checkedNotification = checkedNotification;
            this.checkedNightNotification = checkedNightNotification;
        }
    }

    [Header("toggle")] 
    public Toggle toggleTerm;
    public Toggle togglePersonalInfo;
    public Toggle toggleNotice;
    public Toggle toggleNightNotice;

    [Header("button")] 
    public Button btnLearnMoreTerm;
    public Button btnLearnMorePersonalInfo;
    public Button btnAgreeAllStartGame;
    public Button btnStartGame;
    
    public UnityAction<ClosePopupResult> closeEvent;

    protected override void Start()
    {
        base.Start();

        SetupToggle();
        SetupButton();
    }

    public override void OnClosePopup(bool isRunAnim = true)
    {
        base.OnClosePopup(isRunAnim);

        var result = new ClosePopupResult(toggleNotice.isOn, toggleNightNotice.isOn);
        closeEvent?.Invoke(result);
    }

    private void SetupToggle()
    {
        toggleNotice.OnValueChangedAsObservable()
            .Subscribe(isOn =>
            {
                toggleNightNotice.interactable = isOn;
                if (!isOn)
                {
                    toggleNightNotice.isOn = false;
                }
            });

        toggleNotice.onValueChanged.AddListener(val =>
        {
            PopupResultEnableSystemNotice.OpenPopup(val, false).Forget();
        });

        toggleNightNotice.onValueChanged.AddListener(val =>
        {
            PopupResultEnableSystemNotice.OpenPopup(val, true).Forget();
        });
    }

    private void SetupButton()
    {
        toggleTerm.OnValueChangedAsObservable()
            .CombineLatest(togglePersonalInfo.OnValueChangedAsObservable(),
                (agreeTerm, agreeInfo) => agreeTerm && agreeInfo)
            .SubscribeToInteractable(btnStartGame);

        btnLearnMoreTerm.OnClickAsObservable()
            .Subscribe(_ => { MobirixStaticUtils.OpenMobirixTermPage(); });

        btnLearnMorePersonalInfo.OnClickAsObservable()
            .Subscribe(_ => { MobirixStaticUtils.OpenPrivacyPolicy(true); });

        btnAgreeAllStartGame.OnClickAsObservable().Subscribe(_ =>
        {
            toggleNotice.isOn = true;
            toggleNightNotice.isOn = true;
            ClosePopup();
        });

        btnStartGame.OnClickAsObservable()
            .Subscribe(_ => { ClosePopup(); });
    }
}
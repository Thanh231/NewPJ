
using R3;
using UnityEngine;
using UnityEngine.UI;

public class IAPRefundDesc : MonoBehaviour
{
	public Button btnGo;

    public void Init(SystemLanguage language)
    {
	    btnGo.OnClickAsObservable().Subscribe(_ =>
	    {
		    MobirixStaticUtils.OpenRefundPolicy();
	    });
	    
	    gameObject.SetActive(language == SystemLanguage.Korean);
    }
}
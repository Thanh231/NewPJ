
using Cysharp.Threading.Tasks;

public class IAPImplementation_editor : IIAPImplementation
{
    public IAPImplementation_editor()
    {
        IAPController.instance.CurrencyCode = "VND";
    }

    public async UniTask Init()
    {
        await UniTask.CompletedTask;
    }

    public string GetPriceAsString(string pid)
    {
        return "0.0 USD";
    }

    public decimal GetPriceAsNumber(string pid)
    {
        return 0;
    }

    public void Purchase(string pid)
    {
        IAPController.instance.OnRewardPlayer();
    }

    public void RestorePurchase()
    {
    }
}
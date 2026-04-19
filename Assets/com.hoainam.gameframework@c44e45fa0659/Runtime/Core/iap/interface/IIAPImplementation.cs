
using Cysharp.Threading.Tasks;

public interface IIAPImplementation
{
	UniTask Init();
	string GetPriceAsString(string pid);
	decimal GetPriceAsNumber(string pid);
	void Purchase(string pid);
	void RestorePurchase();
}
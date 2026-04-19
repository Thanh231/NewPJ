
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPImplementation_mobile_ios : IAPImplementation_mobile
{
	private IAppleExtensions appleExtension;
	private string lastTransactionId = "";

	protected override bool WillProcessThisPurchase(PendingOrder order)
	{
		var receipt = JsonConvert.DeserializeObject<IAPReceiptAppStore>(order.Info.Receipt);
		var transactionId = receipt.TransactionID;
		if (transactionId == lastTransactionId)
		{
			Debug.LogError($"[IAP] duplicated transaction id detected: {transactionId}");
			return false;
		}
		else
		{
			lastTransactionId = transactionId;
			return true;
		}
	}

	protected override async UniTask<bool> ValidateReceipt(Order order)
	{
		var receipt = JsonConvert.DeserializeObject<IAPReceiptAppStore>(order.Info.Receipt);
		var pid = GetProduct(order).definition.id;
		var price = GetPriceAsNumber(pid);
		return await ServerController.instance.ValidateAppStoreReceipt(receipt.Payload, receipt.TransactionID, pid,
			price);
	}

	public override void RestorePurchase()
	{
		appleExtension.RestoreTransactions((result, msg) =>
		{
			Debug.Log($"[IAP] restore iap with result={result} msg={msg}");
		});
	}
}
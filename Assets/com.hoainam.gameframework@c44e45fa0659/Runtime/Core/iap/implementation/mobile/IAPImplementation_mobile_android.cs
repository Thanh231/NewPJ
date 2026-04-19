
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPImplementation_mobile_android: IAPImplementation_mobile
{
	protected override List<ProductDefinition> GetListProductsToFetch()
	{
		var lProducts = base.GetListProductsToFetch();
		
		var pid = GameFrameworkConfig.instance.playPassPID;
		if (!string.IsNullOrEmpty(pid))
		{
			lProducts.Add(new ProductDefinition(pid, ProductType.Consumable));
		}
		
		return lProducts;
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();

		var pid = GameFrameworkConfig.instance.playPassPID;
		if (!string.IsNullOrEmpty(pid))
		{
			var product = GetProduct(pid);
			var price = product.metadata.localizedPrice;
			IAPController.instance.IsPlayPassRx.Value = price == 0;

			Debug.Log($"[PlayPass] product {pid} has price={price}");
		}
	}

	protected override async UniTask<bool> ValidateReceipt(Order order)
	{
		StaticUtils.LogFramework("[IAP] start ValidateReceipt for android");
		
		string token;
		string orderNumber;
        try
		{
			var googleReceipt = IAPReceiptGooglePlay.FromJson(order.Info.Receipt);
			token = googleReceipt.Payload.json.purchaseToken;
			orderNumber = googleReceipt.Payload.json.orderId;
		}
		catch (Exception e)
		{
			UserReportPurchaseException(e, order, "Fail to parse receipt");
			throw e;
		}
        
		StaticUtils.LogFramework("[IAP] start send receipt to server");

		try
		{
			var pid = GetProduct(order).definition.id;
			var price = GetPriceAsNumber(pid);
			var isSubscription = IAPController.instance.IsSubscriptionPID(pid);
			var validateOK =
				await ServerController.instance.ValidateGooglePlayReceipt(pid, token, isSubscription, orderNumber,
					price);
			if (!validateOK)
			{
				UserReportPurchaseException(new Exception(), order, "Validate receipt return FAIL");
			}

			StaticUtils.LogFramework($"[IAP] finish ValidateReceipt for android, result={validateOK}");

			return validateOK;
		}
		catch (Exception e)
		{
			UserReportPurchaseException(e, order, "Fail to validate receipt");
			throw e;
		}
	}

	public override void RestorePurchase()
	{
	}
}
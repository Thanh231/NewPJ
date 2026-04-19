using Newtonsoft.Json;
using System.Collections.Generic;

[JsonObject(MemberSerialization.OptIn)]
public class IAPReceiptGooglePlay
{
	[JsonObject(MemberSerialization.OptIn)]
	public class ReceiptPayload
	{
		public class PayloadJson
		{
			public string orderId;
			public string packageName;
			public string productId;
			public long purchaseTime;
			public int purchaseState;
			public string purchaseToken;
			public int quantity;
			public bool acknowledged;
		}

		public class SkuDetail
		{
			public string productId;
			public string type;
			public string title;
			public string name;
			public string iconUrl;
			public string description;
			public string price;
			public long price_amount_micros;
			public string price_currency_code;
			public string skuDetailsToken;
		}

		[JsonProperty("json")]
		private string jsonAsStr;

		public PayloadJson json;

		[JsonProperty]
		public string signature;

		[JsonProperty("skuDetails")]
		private List<string> skuDetailsAsListStr;

		public List<SkuDetail> skuDetails = new List<SkuDetail>();

		public static ReceiptPayload FromJson(string json)
		{
			var payload = JsonConvert.DeserializeObject<ReceiptPayload>(json);
			payload.json = JsonConvert.DeserializeObject<PayloadJson>(payload.jsonAsStr);

			foreach (var i in payload.skuDetailsAsListStr)
			{
				payload.skuDetails.Add(JsonConvert.DeserializeObject<SkuDetail>(i));
			}

			return payload;
		}
	}

	[JsonProperty("Payload")]
	private string PayloadAsStr;

	public ReceiptPayload Payload;

	[JsonProperty]
	public string Store;

	[JsonProperty]
	public string TransactionID;

	public static IAPReceiptGooglePlay FromJson(string json)
	{
		var receipt = JsonConvert.DeserializeObject<IAPReceiptGooglePlay>(json);
		receipt.Payload = ReceiptPayload.FromJson(receipt.PayloadAsStr);
		return receipt;
	}
}

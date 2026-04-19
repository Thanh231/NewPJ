
using Cysharp.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class GetCountryFlagTool
{
	const string url = "https://en.wikipedia.org/wiki/ISO_4217";

	public static async UniTask GetCountryFlag(string parentFolder)
	{
		var html = await DownloadHTML();
		var dic = ParseHTML(html);
		await DownloadPNG(dic, parentFolder);
	}

	public static async UniTask<string> DownloadHTML()
	{
		var result = await StaticUtils.GetHttpRequest(url, returnText: true);
		return result.resultAsText;
	}

	public static Dictionary<string, List<string>> ParseHTML(string html)
	{
		var result = new Dictionary<string, List<string>>();

		var htmlDoc = new HtmlDocument();
		htmlDoc.LoadHtml(html);

		var lTables = htmlDoc.DocumentNode.SelectNodes("/html/body/div[@class='mw-page-container']/div/div[@class='mw-content-container']/main/div[@class='vector-body']/div[@id='mw-content-text']/div[@class='mw-content-ltr mw-parser-output']/table[@class='wikitable sortable mw-collapsible']");
		HtmlNode table = null;
		foreach (var i in lTables)
		{
			var captionTag = i.SelectSingleNode("caption");
			if (captionTag.InnerText.StartsWith("Active ISO 4217 currency codes"))
			{
				table = i;
				break;
			}
		}

		var rows = table.SelectNodes("tbody/tr");
		foreach (var row in rows)
		{
			var lCells = row.SelectNodes("td");
			if (lCells != null)
			{
				var iconTag = lCells.Last();
				var lIcons = iconTag.SelectNodes("span/span/span/img");
				if (lIcons != null)
				{
					var currencyCode = lCells.First().InnerText;

					var lUrls = new List<string>();
					foreach (var icon in lIcons)
					{
						//def is default value if attribute is not present
						var iconUrl = icon.GetAttributeValue("src", def: "");
						lUrls.Add(iconUrl);
					}

					result.Add(currencyCode, lUrls);
				}
			}
		}

		return result;
	}

	public static async UniTask DownloadPNG(Dictionary<string, List<string>> dic, string parentFolder)
	{
		var lTasks = new List<UniTask>();
		foreach (var i in dic)
		{
			lTasks.Add(DownloadPNG(i.Key, i.Value, parentFolder));
		}
		await UniTask.WhenAll(lTasks);
	}

	public static async UniTask DownloadPNG(string currencyCode, List<string> lUrls, string parentFolder)
	{
		var lTasks = new List<UniTask>();

		if (lUrls.Count == 1)
		{
			var url = lUrls[0];
			var path = $"{parentFolder}/{currencyCode}.png";
			lTasks.Add(DownloadPNG(url, path));
		}
		else
		{
			StaticUtils.CreateFolder($"{parentFolder}/{currencyCode}", isAbsolutePath: true);
			foreach (var i in lUrls)
			{
				var url = i;
				var path =$"{parentFolder}/{currencyCode}/{Path.GetFileName(i)}";
				lTasks.Add(DownloadPNG(url, path));
			}
		}

		await UniTask.WhenAll(lTasks);
	}

	public static async UniTask DownloadPNG(string url, string path)
	{
		var regex = Regex.Match(url, "\\/\\d+px-");
		url = url.Replace(regex.Value, "/45px-");

		var result = await StaticUtils.GetImageHttpRequest(url);
		File.WriteAllBytes(path, result.resultAsBinary);
	}
}
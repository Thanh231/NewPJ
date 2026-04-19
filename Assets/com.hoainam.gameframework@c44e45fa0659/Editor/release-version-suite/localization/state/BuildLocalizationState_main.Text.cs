
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class BuildLocalizationState_main
{
	private readonly string textOutputPath = $"{Application.dataPath}/_game/localization-data/text/{{0}}.txt";

	private Dictionary<LanguageGroupName, string> BuildLocalizationText(List<string> csvPath)
	{
		ReadLocalizationData(csvPath, out var keys, out var dicLanguages);
		ValidateLocalizationData(keys, dicLanguages);
		WriteLocalizationData(keys, dicLanguages);

		return ToLanguageGroupData(dicLanguages);
	}

	#region read data

	private void ReadLocalizationData(List<string> lCsvPath, out List<string> keys,
		out Dictionary<SystemLanguage, List<string>> dicLanguages)
	{
		List<string> headers = null;
		List<List<string>> body = null;
		
		foreach (var path in lCsvPath)
		{
			ReadRawLocalizationData(path, out var tHeaders, out var tBody);
			headers = tHeaders;

			if (body == null)
			{
				body = tBody;
			}
			else
			{
				for (var i = 0; i < body.Count; i++)
				{
					body[i].AddRange(tBody[i]);
				}
			}
		}
		
		keys = body[0];
		dicLanguages = new Dictionary<SystemLanguage, List<string>>();
		for (var i = 1; i < headers.Count; i++)
		{
			var language = StaticUtils.StringToEnum<SystemLanguage>(headers[i]);
			dicLanguages.Add(language, body[i]);
		}
	}

	private static void ReadRawLocalizationData(string csvPath, out List<string> headers, out List<List<string>> body)
	{
		headers = new List<string>();
		body = new List<List<string>>();

		using (var stream = new StreamReader(csvPath))
		{
			var csvReader = new CsvReader(stream);
			var fields = csvReader.ReadRecord();

			foreach (var field in fields)
			{
				if (!string.IsNullOrEmpty(field))
				{
					headers.Add(field);
					body.Add(new List<string>());
				}
				else
				{
					break;
				}
			}

			while ((fields = csvReader.ReadRecord()) != null)
			{
				for (var i = 0; i < fields.Count; i++)
				{
					if (i < body.Count)
					{
						body[i].Add(fields[i]);
					}
				}
			}
		}
	}

	#endregion

	#region validate data

	private static void ValidateLocalizationData(List<string> keys, Dictionary<SystemLanguage, List<string>> dicLanguages)
	{
		ValidateUniqueKeys(keys);
		ValidateNonEmptyLocalization(keys, dicLanguages);
		ValidateCorrectParameters(keys, dicLanguages);
	}

	private static void ValidateUniqueKeys(List<string> keys)
	{
		var set = new HashSet<string>();
		foreach (var i in keys)
		{
			if (!set.Add(i))
			{
				throw new Exception($"[localization data] duplicate key: {i}");
			}
		}
	}

	private static void ValidateNonEmptyLocalization(List<string> keys, Dictionary<SystemLanguage, List<string>> dicLanguages)
	{
		foreach (var pair in dicLanguages)
		{
			for (var i = 0; i < pair.Value.Count; i++)
			{
				if (string.IsNullOrEmpty(pair.Value[i]))
				{
					throw new Exception($"[localization data] empty text at key={keys[i]} language={pair.Key}");
				}
			}
		}
	}

	private static void ValidateCorrectParameters(List<string> keys, Dictionary<SystemLanguage, List<string>> dicLanguages)
	{
		for (var i = 0; i < keys.Count; i++)
		{
			var nParamsInEnglish = GetNumberOfParameters(dicLanguages[SystemLanguage.English][i]);
			if (nParamsInEnglish < 0)
			{
				throw new Exception($"[localization data] parameter error at key={keys[i]} language=English");
			}

			foreach (var pair in dicLanguages)
			{
				if (pair.Key != SystemLanguage.English)
				{
					var nParamsInOtherLang = GetNumberOfParameters(pair.Value[i]);
					if (nParamsInOtherLang != nParamsInEnglish)
					{
						throw new Exception($"[localization data] parameter error at key={keys[i]} language={pair.Key}");
					}
				}
			}
		}
	}

	private static int GetNumberOfParameters(string text)
	{
		//build list of params
		var idx = 0;
		var lParams = new List<int>();
		while (idx < text.Length)
		{
			if (text[idx] == '{')
			{
				var param = ParseParameter(text, ref idx);
				if (param >= 0)
				{
					lParams.Add(param);
				}
				else
				{
					return -1;
				}
			}
			else
			{
				idx++;
			}
		}
		
		//process list params
		lParams.Sort();
		if (lParams.Count == 0)
		{
			return 0;
		}

		if (lParams[0] != 0)
		{
			return -1;
		}

		for (var i = 0; i < lParams.Count - 1; i++)
		{
			if (lParams[i] != lParams[i + 1] - 1)
			{
				return -1;
			}
		}

		return lParams.Count;
	}

	private static int ParseParameter(string text, ref int idxBegin)
	{
		var idxEnd = text.IndexOf('}', idxBegin);
		if (idxEnd <= idxBegin + 1)
		{
			return -1;
		}

		var paramTxt = text.Substring(idxBegin + 1, idxEnd - idxBegin - 1);
		var trimmedTxt = paramTxt.Trim();

		if (!trimmedTxt.Equals(paramTxt))
		{
			return -1;
		}
		
		try
		{
			idxBegin = idxEnd + 1;
			return StaticUtils.StringToInt(paramTxt);
		}
		catch (Exception e)
		{
			return -1;
		}
	}

	#endregion

	#region transform data

	private Dictionary<LanguageGroupName, string> ToLanguageGroupData(
		Dictionary<SystemLanguage, List<string>> dicLanguages)
	{
		var dic = new Dictionary<LanguageGroupName, HashSet<char>>();
		var languageInfo = new LanguageInfoConfig();
		foreach (var pair in dicLanguages)
		{
			var groupName = languageInfo.GetLanguageInfoItem(pair.Key).groupName;
			if (!dic.ContainsKey(groupName))
			{
				dic.Add(groupName, new HashSet<char>());
			}
			foreach (var text in pair.Value)
			{
				foreach (var character in text)
				{
					dic[groupName].Add(character);
				}
			}
		}
		
		//return
		var dicResult = new Dictionary<LanguageGroupName, string>();
		foreach (var i in dic)
		{
			var arr = new char[i.Value.Count];
			i.Value.CopyTo(arr);
			dicResult.Add(i.Key, new string(arr));
		}
		return dicResult;
	}

	#endregion

	#region write data

	private void WriteLocalizationData(List<string> keys, Dictionary<SystemLanguage, List<string>> dicLanguages)
	{
		WriteDataFile("_key", keys);
		foreach (var i in dicLanguages)
		{
			WriteDataFile(i.Key.ToString(), i.Value);
		}
	}

	private void WriteDataFile(string filename, List<string> data)
	{
		var path = string.Format(textOutputPath, filename);
		StaticUtils.OpenFileForWrite(path, (StreamWriter stream) =>
		{
			for (var i = 0; i < data.Count; i++)
			{
				stream.Write(data[i]);
				if (i < data.Count - 1)
				{
					stream.Write('\0');
				}
			}
		}, true);
	}

	#endregion
}

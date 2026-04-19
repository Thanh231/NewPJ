
using System.Collections.Generic;
using UnityEngine;

public class ForbiddenWordsProcessor
{
    private List<string> listWords = new();
    
    public ForbiddenWordsProcessor(string resourcePath)
    {
        var listLang = LocalizationController.GetSupportedLanguages();
        foreach (var language in listLang)
        {
            var textAsset = Resources.Load<TextAsset>($"{resourcePath}/{language}.csv");
            if (textAsset != null)
            {
                ProcessLanguage(textAsset.text);
            }
        }
    }

    private void ProcessLanguage(string text)
    {
        var words = text.Split('\n');
        foreach (var word in words)
        {
            var trimmedWord = word.Trim();
            if (!string.IsNullOrEmpty(trimmedWord) && !listWords.Contains(trimmedWord))
            {
                listWords.Add(trimmedWord);
            }
        }
    }

    public bool IsValid(string content, out string forbiddenWord)
    {
        content = content.ToLower();
        foreach (var word in listWords)
        {
            if (content.Contains(word))
            {
                forbiddenWord = word;
                return false;
            }
        }

        forbiddenWord = null;
        return true;
    }
}

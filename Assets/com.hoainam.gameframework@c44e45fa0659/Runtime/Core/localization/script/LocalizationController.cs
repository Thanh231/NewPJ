
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using R3;

#if (UNITY_EDITOR || UNITY_ANDROID) && USE_SYSTEM_NOTIFICATION
using Unity.Notifications.Android;
#endif

#if (UNITY_EDITOR || UNITY_IOS) && USE_SYSTEM_NOTIFICATION
using Unity.Notifications.iOS;
#endif

public class LocalizationController : SingletonMonoBehaviour<LocalizationController>
{
    #region core

    public TMP_FontAsset fontAsset { get; private set; }
    public ReactiveProperty<SystemLanguage> languageRx { get; private set; }
    private readonly Dictionary<string, string> dicLocalizationText = new();
    private readonly Dictionary<SystemLanguage, Dictionary<string, string>> dicAllLocalizationText = new();
    private readonly Dictionary<Color, Material> dicFontMaterial = new();

    private void Start()
    {
#if UNITY_EDITOR
        //by default, korean language will line break incorrectly,
        //for example, "안녕하세요 안녕" => "안녕
        //                                하세요 안녕"
        //to make sure korean language only wrap by space,
        //go to TMP settings => check "Use modern line breaking".

        if (!TMP_Settings.useModernHangulLineBreakingRules)
        {
            Debug.LogError(
                "need to enable 'Use modern line breaking' in TMP settings, for more info, see comment in LocalizationController.Start()");
        }
#endif
    }

    #endregion

    #region load data

    public async UniTask LoadLocalizationData(ReactiveProperty<SystemLanguage> languageRx)
    {
        this.languageRx = languageRx;
        await LoadLocalizationData(languageRx.Value);
    }
    
    public async UniTask LoadLocalizationData(SystemLanguage language)
    {
        dicFontMaterial.Clear();

        //load font asset
        var languageInfo = new LanguageInfoConfig();
        var setType = languageInfo.GetLanguageInfoItem(language).groupName;
        fontAsset = await AssetManager.instance.LoadTmpFont($"{GetBaseAddressableName()}/font", setType.ToString());

        //load texts
        dicLocalizationText.Clear();

        var keys = await ReadDataFile("_key", AssetManager.instance);
        var texts = await ReadDataFile(language.ToString(), AssetManager.instance);
        for (var i = 0; i < keys.Count; i++)
        {
            dicLocalizationText.Add(keys[i], texts[i]);
        }
    }

    public async UniTask LoadAllLocalizationData()
    {
        var assetManager = new AssetManager();
        var keys = await ReadDataFile("_key", assetManager);
        var lLangs = GetSupportedLanguages();
        foreach (var lang in lLangs)
        {
            var dic = new Dictionary<string, string>();
            var texts = await ReadDataFile(lang.ToString(), assetManager);
            for (var i = 0; i < keys.Count; i++)
            {
                dic.Add(keys[i], texts[i]);
            }
            dicAllLocalizationText.Add(lang, dic);
        }
    }

    private async UniTask<List<string>> ReadDataFile(string filename, AssetManager assetManager)
    {
        var separator = '\0';
        var txt = (await assetManager.LoadText($"{GetBaseAddressableName()}/text", filename)).text;
        txt = txt.TrimEnd(separator);
        return new List<string>(txt.Split(separator));
    }

    private string GetBaseAddressableName()
    {
#if UNITY_EDITOR
        return "localization";
#endif

        var localFile = $"{RemoteAssetsPath.localAssetFolder}/{RemoteAssetsPath.assetFileName}";
        if (StaticUtils.CheckFileExist(localFile, true))
        {
            return "localization";
        }
        else
        {
            Debug.LogError("[localization] using local localization");
            return "localization-local";
        }
    }

    #endregion

    #region utils

    public void SetUnderlayColorForTMP(TMP_Text text, Color color)
    {
        //by default, all text use one single material fontSharedMaterial => 1 draw call
        //when text get fontMaterial, it will clone fontSharedMaterial => 1 more draw call

        if (dicFontMaterial.ContainsKey(color))
        {
            text.fontMaterial = dicFontMaterial[color];
            text.fontSharedMaterial = dicFontMaterial[color];
        }
        else
        {
            text.fontMaterial.SetColor("_UnderlayColor", color);
            dicFontMaterial.Add(color, text.fontMaterial);
        }
    }

    public Dictionary<SystemLanguage, string> GetLocalizationTextAllLangs(string key)
    {
        var dic = new Dictionary<SystemLanguage, string>();
        foreach (var i in dicAllLocalizationText)
        {
            dic.Add(i.Key, i.Value[key]);
        }

        return dic;
    }

    public static List<SystemLanguage> GetSupportedLanguages()
    {
        var langInfo = new LanguageInfoConfig();
        return new List<SystemLanguage>(langInfo.languageInfoItems.Keys);
    }
    
    //must be private, only set key+parameter for text object,
    //not set whole text for text object
    private string GetLocalizationText(string key)
    {
        if (dicLocalizationText.ContainsKey(key))
        {
            return dicLocalizationText[key];
        }
        else
        {
            throw new Exception($"[Localization] there's no key {key}");
        }
    }

    private string GetLocalizationText(string key, object[] parameters)
    {
        var txt = GetLocalizationText(key);
        if (parameters != null && parameters.Length > 0)
        {
            try
            {
                txt = string.Format(txt, parameters);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw new Exception(
                    $"[localize] format string fail, str={txt} nParams={parameters.Length}");
            }
        }

        return txt;
    }

    #endregion
    
    #region interact with external 3rd party

    public void SetupLocalizedText(LocalizedText localizedText)
    {
        if (string.IsNullOrEmpty(localizedText.key))
        {
            return;
        }
        var txt = GetLocalizationText(localizedText.key, localizedText.parameters);
        localizedText.SetText(txt);
    }
    
    public string LocalizedTextParameterToString(LocalizedTextParameter param)
    {
        return GetLocalizationText(param.key, param.parameters);
    }

#if (UNITY_EDITOR || UNITY_ANDROID) && USE_SYSTEM_NOTIFICATION

    public void SetContentAndroidNotification(string titleKey, object[] titleParams, string msgKey, object[] msgParams,
        ref AndroidNotification target)
    {
        target.Title = GetLocalizationText(titleKey, titleParams);
        target.Text = GetLocalizationText(msgKey, msgParams);
    }

#endif
    
#if (UNITY_EDITOR || UNITY_IOS) && USE_SYSTEM_NOTIFICATION

    public void SetContentIosNotification(string titleKey, object[] titleParams, string msgKey, object[] msgParams,
        iOSNotification target)
    {
        target.Title = GetLocalizationText(titleKey, titleParams);
        target.Body = GetLocalizationText(msgKey, msgParams);
    }

#endif
    
    #endregion
}
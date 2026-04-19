
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class SplashScreenController
{
    [Serializable]
    public class GameTitleSprite
    {
        public SystemLanguage language;
        public Sprite sprite;
    }
    
    [Header("game title")]
    public Sprite defaultGameTitle;
    public List<GameTitleSprite> lGameTitleSprites;
    public Image imgGameTitle;

    private void SetLanguage_title(SystemLanguage language)
    {
        var spriteObj = lGameTitleSprites.Find(x => x.language == language);
        imgGameTitle.sprite = spriteObj != null ? spriteObj.sprite : defaultGameTitle;
    }
}

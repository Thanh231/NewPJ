using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TextNotifyController : SingletonMonoBehaviour<TextNotifyController>
{
    #region classes

    public class TextCfg
    {
        public string key;
        public object[] lParams;

        public TextCfg(string key)
        {
            this.key = key;
        }
        
        public TextCfg(string key, object[] lParams)
        {
            this.key = key;
            this.lParams = lParams;
        }
    }
    
    public abstract class ColorCfg
    {
        public Color outline;

        public static ColorCfg SolidColor(Color outline, Color color)
        {
            return new SolidColorCfg()
            {
                outline = outline,
                color = color
            };
        }

        public static ColorCfg GradientColor(Color outline, Color colorUp, Color colorDown)
        {
            return new GradientColorCfg()
            {
                outline = outline,
                colorUp = colorUp,
                colorDown = colorDown
            };
        }
    }

    public class SolidColorCfg : ColorCfg
    {
        public Color color;
    }
    
    public class GradientColorCfg : ColorCfg
    {
        public Color colorUp;
        public Color colorDown;
    }
    
    public abstract class PositionCfg
    {
        public static PositionCfg CenterPosition()
        {
            return new CenterPositionCfg();
        }

        public static PositionCfg CustomPosition(Vector2 position, bool inWorldSpace)
        {
            return new CustomPositionCfg()
            {
                position = position,
                inWorldSpace = inWorldSpace,
            };
        }
    }
    
    public class CenterPositionCfg : PositionCfg
    {
    }
    
    public class CustomPositionCfg : PositionCfg
    {
        public Vector2 position;
        public bool inWorldSpace;
    }

    #endregion

    #region data members

    public ObjectPool textPool;
    
    private Camera _mainCamera;
    private Camera mainCamera
    {
        get
        {
            if (!_mainCamera)
            {
                _mainCamera = Camera.main;
            }

            return _mainCamera;
        }
    }

    #endregion

    #region notify

    private async UniTask NotifyTextInternal(TextCfg textCfg, ColorCfg colorCfg, PositionCfg positionCfg,
        string debugText)
    {
        var text = await textPool.Spawn<TextNotify>("text");
        text.Setup(textCfg, colorCfg, positionCfg, mainCamera, debugText);
        await UniTask.Delay(TimeSpan.FromSeconds(text.AnimTime));
        if (text)
        {
            textPool.Despawn(text.gameObject);
        }
    }

    public async UniTask NotifyText(TextCfg textCfg, ColorCfg colorCfg, PositionCfg positionCfg)
    {
        await NotifyTextInternal(textCfg, colorCfg, positionCfg, null);
    }

    public async UniTask NotifyText(string key, ColorCfg colorCfg)
    {
        await NotifyText(new TextCfg(key), colorCfg, PositionCfg.CenterPosition());
    }

    public async UniTask NotifyText(string key, ColorCfg colorCfg, Vector2 position, bool inWorldSpace)
    {
        await NotifyText(new TextCfg(key), colorCfg, PositionCfg.CustomPosition(position, inWorldSpace));
    }

    /// <summary>
    /// only use this for debug purpose, like cheat, GD tools, etc.
    /// </summary>
    /// <param name="debugText">direct text, no use localization</param>
    /// <param name="color">text color</param>
    public async UniTask NotifyText_debug(string debugText, Color color)
    {
        await NotifyTextInternal(null, ColorCfg.SolidColor(color, color), PositionCfg.CenterPosition(), debugText);
    }

    #endregion
}

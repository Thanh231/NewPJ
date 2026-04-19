
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioItemCfg
{
    public string audioClipName;
    public AudioClip audioClip;
}

public partial class GameFrameworkConfig
{
    [Header("audio")]
    public List<AudioItemCfg> audioClips;
}

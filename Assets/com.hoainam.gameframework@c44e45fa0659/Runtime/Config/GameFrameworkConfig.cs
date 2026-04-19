
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameFrameworkConfig", menuName = "Configs/GameFrameworkConfig", order = 1)]
public partial class GameFrameworkConfig : ScriptableObject
{
    private static GameFrameworkConfig _instance;
    public static GameFrameworkConfig instance
    {
        get
        {
            if (!_instance)
            {
                _instance = Resources.Load<GameFrameworkConfig>("GameFrameworkConfig");
                if (!_instance)
                {
                    throw new Exception(
                        "create config Configs/GameFrameworkConfig at folder Resources/GameFrameworkConfig");
                }
            }

            return _instance;
        }
    }
}
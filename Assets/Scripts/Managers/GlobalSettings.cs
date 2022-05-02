using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Settings/GlobalSettings")]
public class GlobalSettings : SingletonScriptableObject<GlobalSettings>
{
    [SerializeField] private string resourcesPath;
    [SerializeField] private GameSettings gameSettings;

    public static string ResourcesPath { get { return Instance.resourcesPath; } }
    public static GameSettings GameSettings { get { return Instance.gameSettings; } }
}

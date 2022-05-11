using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Settings/GlobalSettings")]
public class GlobalSettings : SingletonScriptableObject<GlobalSettings>
{

    [SerializeField] private string prefabsPickupPath;
    [SerializeField] private string prefabsEnemiesPath;
    [SerializeField] private string prefabsUIPath;
    [SerializeField] private string prefabsPlayerCharacterPath;
    [SerializeField] private GameSettings gameSettings;

    public static string PickupsPath { get { return Instance.prefabsPickupPath; } }
    public static string EnemiesPath { get { return Instance.prefabsEnemiesPath; } }
    public static string UIPath { get { return Instance.prefabsUIPath; } }
    public static string PlayerCharacterPath { get { return Instance.prefabsPlayerCharacterPath; } }
    public static GameSettings GameSettings { get { return Instance.gameSettings; } }
}

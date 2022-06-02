using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Settings/GlobalSettings")]
public class GlobalSettings : SingletonScriptableObject<GlobalSettings>
{

    [SerializeField] private string resourcesPath;
    [SerializeField] private string prefabsPickupPath;
    [SerializeField] private string prefabsEnemiesPath;
    [SerializeField] private string prefabsUIPath;
    [SerializeField] private string prefabsPlayerCharacterPath;
    [SerializeField] private string prefabsEquipmentPath;
    [SerializeField] private string prefabsMiscPath;
    [SerializeField] private string saveFileName;
    [SerializeField] private string loadSaveFileSettingName;
    [SerializeField] private GameSettings gameSettings;

    public static string ResourcesPath { get { return Instance.resourcesPath; } }
    public static string PickupsPath { get { return Instance.prefabsPickupPath; } }
    public static string EnemiesPath { get { return Instance.prefabsEnemiesPath; } }
    public static string UIPath { get { return Instance.prefabsUIPath; } }
    public static string PlayerCharacterPath { get { return Instance.prefabsPlayerCharacterPath; } }
    public static string EquipmentPath { get { return Instance.prefabsEquipmentPath; } }
    public static string MiscPath { get { return Instance.prefabsMiscPath; } }
    public static string SaveFileName { get { return Instance.saveFileName; } }
    public static string LoadSaveFileSettingName { get { return Instance.loadSaveFileSettingName; } }
    public static GameSettings GameSettings { get { return Instance.gameSettings; } }
}

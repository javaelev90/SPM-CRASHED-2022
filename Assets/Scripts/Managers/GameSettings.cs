using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Settings/GameSettings")]
public class GameSettings : ScriptableObject
{
    [SerializeField] private string gameTitle;
    [SerializeField] private string gameVersion;
    [SerializeField] private string gameSceneName;
    [SerializeField] Character characterChoice;

    public Character CharacterChoice { get; set; }
    public string GameTitle { get { return gameTitle; } }
    public string GameVersion { get { return gameVersion; } }
    public string GameSceneName { get { return gameSceneName; } }
}

public enum Character
{
    NONE = 0,
    SOLDIER = 1,
    ENGINEER = 2
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Settings/GameSettings")]
public class GameSettings : ScriptableObject
{
    [SerializeField] private string gameTitle;
    [SerializeField] private string gameVersion;
    [SerializeField] private string gameSceneName;
    [SerializeField] private string winSceneName;
    [SerializeField] private string characterChoicePropertyName;
    [SerializeField] private string particleEffectPath;

    public string GameTitle { get { return gameTitle; } }
    public string GameVersion { get { return gameVersion; } }
    public string GameSceneName { get { return gameSceneName; } }
    public string WinSceneName { get { return winSceneName; } }
    public string CharacterChoicePropertyName { get { return characterChoicePropertyName; } }
    public string ParticleEffectPath { get { return particleEffectPath; } }
}

public enum Character
{
    NONE = 0,
    SOLDIER = 1,
    ENGINEER = 2
}
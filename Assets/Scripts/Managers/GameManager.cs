using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log($"Oh no, you chose the {(Character)PlayerPrefs.GetInt(GlobalSettings.GameSettings.CharacterChoicePropertyName)} charater");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

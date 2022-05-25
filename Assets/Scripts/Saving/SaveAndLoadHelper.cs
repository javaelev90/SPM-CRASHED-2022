using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveAndLoadHelper
{
    private static string saveFileName = "GameSaveFile";

    public static string SaveFileName { 
        get {
            return saveFileName;
        }
        set {
            saveFileName = value;
        }
    }

    public static void SaveData(GameDataHolder gameDataHolder)
    {
        string gameDataJSON = JsonUtility.ToJson(gameDataHolder);
        File.WriteAllText(Application.persistentDataPath + $"/{SaveFileName}.json", gameDataJSON);
    }

    public static GameDataHolder LoadData()
    {
        string gameDataJSON = File.ReadAllText(Application.persistentDataPath + $"/{SaveFileName}.json");
        return JsonUtility.FromJson<GameDataHolder>(gameDataJSON);
    }

}

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

    public static bool SaveFileExists()
    {
        return File.Exists(Application.persistentDataPath + $"/{SaveFileName}.json");
    }

    public static void SaveData(GameDataHolder gameDataHolder)
    {
        string gameDataJSON = SerializeObjectToJson<GameDataHolder>(gameDataHolder);
        File.WriteAllText(Application.persistentDataPath + $"/{SaveFileName}.json", gameDataJSON);
    }

    public static GameDataHolder LoadData()
    {
        string gameDataJSON = File.ReadAllText(Application.persistentDataPath + $"/{SaveFileName}.json");
        return JsonUtility.FromJson<GameDataHolder>(gameDataJSON);
    }

    public static T LoadData<T>(string savedDataJSON)
    {
        return JsonUtility.FromJson<T>(savedDataJSON);
    }

    public static string SerializeObjectToJson<T>(T data)
    {
        return JsonUtility.ToJson(data);
    }

}

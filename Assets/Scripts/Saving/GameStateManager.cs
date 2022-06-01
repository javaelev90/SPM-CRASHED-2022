using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;
using System.Linq;
using Photon.Pun;

public class GameStateManager : MonoBehaviourPunCallbacks
{
    public static GameDataHolder gameDataHolder;
    private DataCollector dataCollector;
    private DataLoader dataLoader;

    private bool initialized = false;
    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (initialized == false)
        {
            gameDataHolder = new GameDataHolder();
            gameDataHolder.Initialize();
            dataCollector = new DataCollector(gameDataHolder);
            dataLoader = new DataLoader();
            LoadData();
            initialized = true;
        }
    }

    private void LoadData()
    {
        if (SaveAndLoadHelper.SaveFileExists())
        {
            gameDataHolder = SaveAndLoadHelper.LoadData();
            dataLoader.GameDataHolder = gameDataHolder;
        }
    }

    public void LoadPlayerData(ref GameObject player, Character character)
    {
        if (character == Character.ENGINEER)
        {
            dataLoader.LoadLocalPlayerData(ref player, dataLoader.GameDataHolder.engineerData);
        }
        else
        {
            dataLoader.LoadLocalPlayerData(ref player, dataLoader.GameDataHolder.soldierData);
        }
    }

    public void SaveGame()
    {
        gameDataHolder = dataCollector.CollectData();
        SaveAndLoadHelper.SaveData(gameDataHolder);
    }

    public bool SaveExists()
    {
        return SaveAndLoadHelper.SaveFileExists();
    }

    public void SyncOtherPlayerData(Character character)
    {
        string otherPlayerData = "";
        if (character == Character.ENGINEER)
        {
            otherPlayerData = SaveAndLoadHelper.SerializeObjectToJson(gameDataHolder.engineerData);
        }
        else
        {
            otherPlayerData = SaveAndLoadHelper.SerializeObjectToJson(gameDataHolder.soldierData);
        }
        photonView.RPC(nameof(SyncPlayerDataRPC), RpcTarget.Others, otherPlayerData);
    }

    [PunRPC]
    private void SyncPlayerDataRPC(string playerDataJSON)
    {
        PlayerData playerData = SaveAndLoadHelper.LoadData<PlayerData>(playerDataJSON);
        dataLoader.LoadLocalPlayerData(ref GameManager.player, playerData);
    }

    ////TEST
    //private void TestSaving()
    //{
    //    GameDataHolder gameDataHolder = new GameDataHolder();

    //    ProgressData progressData = new ProgressData();
    //    PlayerData playerData = new PlayerData();
    //    PickupData pickupData = new PickupData();

    //    gameDataHolder.pickupData = pickupData;
    //    gameDataHolder.playerData = playerData;
    //    gameDataHolder.progressData = progressData;

    //    progressData.timeOfDay = 150f;
    //    progressData.upgradeProgress = new List<Ship.ShipUpgradeCost>();
    //    progressData.upgradeProgress[0] = new Ship.ShipUpgradeCost
    //    {
    //        gooCost = 1,
    //        metalCost = 1,
    //        partAvalibul = false,
    //        partAttached = null,
    //        partMissing = null
    //    };

    //    playerData.currentHealth = 10;
    //    playerData.inventory.metal = 4;
    //    playerData.inventory.greenGoo = 7;
    //    playerData.inventory.alienMeat = 3;

    //    pickupData.metal = new List<Vector3>() { new Vector3(350f, 36f, 350f) };

    //    SaveAndLoadHelper.SaveData(gameDataHolder);
    //}



}

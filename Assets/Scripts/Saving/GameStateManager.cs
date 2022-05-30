using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;
using System.Linq;

public class GameStateManager : MonoBehaviour
{
    public static GameDataHolder gameDataHolder;
    private DataCollector dataCollector;

    private void Awake()
    {
        gameDataHolder = new GameDataHolder();
        dataCollector = new DataCollector(ref gameDataHolder);
    }

    public void LoadData()
    {
        SaveAndLoadHelper.LoadData();
    }

    public void SaveGame()
    {
        dataCollector.CollectData();
        SaveAndLoadHelper.SaveData(gameDataHolder);
    }

    //TEST
    private void TestSaving()
    {
        GameDataHolder gameDataHolder = new GameDataHolder();

        ProgressData progressData = new ProgressData();
        PlayerData playerData = new PlayerData();
        PickupData pickupData = new PickupData();

        gameDataHolder.pickupData = pickupData;
        gameDataHolder.playerData = playerData;
        gameDataHolder.progressData = progressData;

        progressData.timeOfDay = 150f;
        progressData.upgradeProgress = new List<Ship.ShipUpgradeCost>();
        progressData.upgradeProgress[0] = new Ship.ShipUpgradeCost
        {
            gooCost = 1,
            metalCost = 1,
            partAvalibul = false,
            partAttached = null,
            partMissing = null
        };

        playerData.currentHealth = 10;
        playerData.inventory.metal = 4;
        playerData.inventory.greenGoo = 7;
        playerData.inventory.alienMeat = 3;

        pickupData.metal = new List<Vector3>() { new Vector3(350f, 36f, 350f) };

        SaveAndLoadHelper.SaveData(gameDataHolder);
    }



}

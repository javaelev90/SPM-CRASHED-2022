using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{

    private void Start()
    {
        SaveGame();
    }

    public void SaveGame()
    {
        GameDataHolder gameDataHolder = new GameDataHolder();
        
        ProgressData progressData = new ProgressData();
        PlayerData playerData = new PlayerData();
        PickupData pickupData = new PickupData();

        gameDataHolder.pickupData = pickupData;
        gameDataHolder.playerData = playerData; 
        gameDataHolder.progressData = progressData;

        progressData.dayCycleTimer = 150f;
        progressData.upgradeProgress = new Ship.ShipUpgradeCost[4];
        progressData.upgradeProgress[0] = new Ship.ShipUpgradeCost
        {
            gooCost = 1,
            metalCost = 1,
            partAvalibul = false,
            partAttached = null,
            partMissing = null
        };
        playerData.currentHealth = 10;
        playerData.inventory = new PlayerData.Inventory();
        playerData.inventory.metal = 4;
        playerData.inventory.greenGoo = 7;
        playerData.inventory.alienMeat = 3;


        pickupData.metal = new List<Vector3>() { new Vector3(350f, 36f, 350f) };

        SaveAndLoadHelper.SaveData(gameDataHolder);
    }
}

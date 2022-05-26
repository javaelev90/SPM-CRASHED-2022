using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;
using System.Linq;

public class GameStateManager : MonoBehaviour
{
    public static GameDataHolder gameDataHolder;

    private void Awake()
    {
        gameDataHolder = new GameDataHolder();
        gameDataHolder.progressData = new ProgressData();
        gameDataHolder.playerData = new PlayerData();
        gameDataHolder.otherPlayerData = new PlayerData();
        gameDataHolder.pickupData = new PickupData();

        EventSystem.Instance.RegisterListener<GunDamageUpgradeEvent>(CollectGunDamageUpgradeEvent);
        EventSystem.Instance.RegisterListener<GunFireRateUpgradeEvent>(CollectGunFireRateUpgradeEvent);
        EventSystem.Instance.RegisterListener<TurretDamageUpgradeEvent>(CollectTurretDamageUpgradeEvent);
        EventSystem.Instance.RegisterListener<TurretHealthUpgradeEvent>(CollectTurretHealthUpgradeEvent);
    }

    private void Start()
    {
        SaveGame();
        //LoadData();
    }

    public void LoadData()
    {
        SaveAndLoadHelper.LoadData();
    }

    public void SaveGame()
    {
        CollectData();
        SaveAndLoadHelper.SaveData(gameDataHolder);
    }

    public void CollectData()
    {
        CollectPlayerData();
        CollectProgressData();
        CollectPickupData();
    }

    private void CollectPlayerData()
    {
        gameDataHolder.playerData.character = GameManager.character;

        HealthHandler healthHandler = GameManager.player.GetComponent<HealthHandler>();
        if (healthHandler)
        {
            gameDataHolder.playerData.currentHealth = healthHandler.CurrentHealth;
            gameDataHolder.playerData.maxHealth = healthHandler.MaxHealth;
        }

        InventorySystem inventorySystem = GameManager.player.GetComponent<InventorySystem>();
        if (inventorySystem)
        {
            gameDataHolder.playerData.inventory.alienMeat = inventorySystem.Amount<AlienMeat>();
            gameDataHolder.playerData.inventory.metal = inventorySystem.Amount<Metal>();
            gameDataHolder.playerData.inventory.greenGoo = inventorySystem.Amount<GreenGoo>();
        }
    }

    private void CollectProgressData()
    {
        LightingManager lightingManager = FindObjectOfType<LightingManager>();
        if (lightingManager)
        {
            gameDataHolder.progressData.timeOfDay = lightingManager.TimeOfDay;
        }
        
        Ship ship = FindObjectOfType<Ship>();
        if (ship)
        {
            gameDataHolder.progressData.upgradeLevel = ship.nextUpgrade;
            gameDataHolder.progressData.upgradeProgress = ship.shipUpgradeCost;
        }
    }

    private void CollectPickupData()
    {
        List<Pickup> pickupList = FindObjectsOfType<Pickup>().ToList();
        foreach (Pickup pickup in pickupList)
        {
            switch (pickup.getTyp())
            {
                case Pickup_Typs.Pickup.Metal:
                    gameDataHolder.pickupData.metal.Add(pickup.transform.position);
                    break;
                case Pickup_Typs.Pickup.GreenGoo:
                    gameDataHolder.pickupData.greenGoo.Add(pickup.transform.position);
                    break;
                case Pickup_Typs.Pickup.AlienMeat:
                    gameDataHolder.pickupData.alienMeat.Add(pickup.transform.position);
                    break;
            }
        }
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


    private void CollectGunDamageUpgradeEvent(GunDamageUpgradeEvent upgradeEvent)
    {
        gameDataHolder.playerData.upgrades.weaponDamageUpgrades.Add(upgradeEvent.UpgradeAmount);
    }
    private void CollectGunFireRateUpgradeEvent(GunFireRateUpgradeEvent upgradeEvent)
    {
        gameDataHolder.playerData.upgrades.weaponFireRateUpgrades.Add(upgradeEvent.UpgradePercent);
    }
    private void CollectTurretDamageUpgradeEvent(TurretDamageUpgradeEvent upgradeEvent)
    {
        gameDataHolder.playerData.upgrades.turretDamageUpgrades.Add(upgradeEvent.UpgradeAmount);
    }
    private void CollectTurretHealthUpgradeEvent(TurretHealthUpgradeEvent upgradeEvent)
    {
        gameDataHolder.playerData.upgrades.turretHealthUpgrades.Add(upgradeEvent.UpgradeAmount);
    }

}

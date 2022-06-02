using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;
using System.Linq;

public class DataCollector
{
    private GameDataHolder gameDataHolder;

    public DataCollector(GameDataHolder gameDataHolder)
    {
        this.gameDataHolder = gameDataHolder;
        EventSystem.Instance.RegisterListener<GunDamageUpgradeEvent>(CollectGunDamageUpgradeEvent);
        EventSystem.Instance.RegisterListener<GunFireRateUpgradeEvent>(CollectGunFireRateUpgradeEvent);
        EventSystem.Instance.RegisterListener<TurretDamageUpgradeEvent>(CollectTurretDamageUpgradeEvent);
        EventSystem.Instance.RegisterListener<TurretHealthUpgradeEvent>(CollectTurretHealthUpgradeEvent);
    }

    public GameDataHolder CollectData()
    {
        if (GameManager.character == Character.ENGINEER)
        {
            CollectPlayerData(gameDataHolder.engineerData, GameManager.player, Character.ENGINEER);
            //if (GameManager.otherPlayer != null) 
            //    CollectPlayerData(gameDataHolder.soldierData, GameManager.otherPlayer, Character.SOLDIER);
        }
        else
        {
            CollectPlayerData(gameDataHolder.soldierData, GameManager.player, Character.SOLDIER);
            //if(GameManager.otherPlayer != null) 
            //    CollectPlayerData(gameDataHolder.engineerData, GameManager.otherPlayer, Character.ENGINEER);
        }

        CollectProgressData();
        //CollectPickupData();

        return gameDataHolder;
    }

    public void CollectPlayerData(PlayerData playerData, GameObject playerObject, Character character)
    {
        playerData.character = character;
        playerData.position = playerObject.transform.position;

        HealthHandler healthHandler = playerObject.GetComponent<HealthHandler>();
        if (healthHandler)
        {
            playerData.currentHealth = healthHandler.CurrentHealth;
            playerData.maxHealth = healthHandler.MaxHealth;
        }

        InventorySystem inventorySystem = playerObject.GetComponent<InventorySystem>();
        if (inventorySystem)
        {
            playerData.inventory.alienMeat = inventorySystem.Amount<AlienMeat>();
            playerData.inventory.metal = inventorySystem.Amount<Metal>();
            playerData.inventory.greenGoo = inventorySystem.Amount<GreenGoo>();
        }

        if (playerData.character == Character.ENGINEER)
        {
            Turret[] turrets = GameObject.FindObjectsOfType<Turret>();
            PlayerData.TurretData turretData;
            HealthHandler turretHealthHandler;

            foreach (Turret turret in turrets)
            {
                turretHealthHandler = turret.gameObject.GetComponent<HealthHandler>();
                turretData = new PlayerData.TurretData
                {
                    position = turret.transform.position,
                    maxHealth = turretHealthHandler.MaxHealth,
                    currentHealth = turretHealthHandler.CurrentHealth,
                };
                playerData.turrets.Add(turretData);
            }
        }
        else
        {
            playerData.ammo = GameManager.player.GetComponent<SoldierCharacter>().weapon.currentAmmo;
        }
    }

    public void CollectProgressData()
    {
        LightingManager lightingManager = GameObject.FindObjectOfType<LightingManager>();
        if (lightingManager)
        {
            gameDataHolder.progressData.timeOfDay = lightingManager.TimeOfDay;
            gameDataHolder.progressData.isNight = lightingManager.IsNight;
        }

        Ship ship = GameObject.FindObjectOfType<Ship>();
        if (ship)
        {
            gameDataHolder.progressData.upgradeLevel = ship.nextUpgrade;

            foreach (Ship.ShipUpgradeCost shipProgress in ship.shipUpgradeCost)
            {
                if (shipProgress.partAttached != null && shipProgress.partMissing != null)
                {
                    gameDataHolder.progressData.upgradeProgress.Add(new ProgressData.ShipProgress
                    {
                        metalCost = shipProgress.metalCost,
                        gooCost = shipProgress.gooCost,
                        partAvailable = shipProgress.partAvalibul,
                        partAttachedName = shipProgress.partAttached.name,
                        partMissingName = shipProgress.partMissing.name
                    });
                }
            }
        }

        Level1 level1 = GameObject.FindObjectOfType<Level1>();
        if(level1)
        {
            gameDataHolder.progressData.tutorialIsDone = level1.TutorialIsDone;
        }
    }

    private void CollectPickupData()
    {
        List<Pickup> pickupList = GameObject.FindObjectsOfType<Pickup>().ToList();
        foreach (Pickup pickup in pickupList)
        {
            switch (pickup.getTyp())
            {
                case Pickup_Typs.Pickup.Metal:
                    gameDataHolder.pickupData.metal.Add(pickup.gameObject.transform.position);
                    break;
                case Pickup_Typs.Pickup.GreenGoo:
                    gameDataHolder.pickupData.greenGoo.Add(pickup.gameObject.transform.position);
                    break;
                case Pickup_Typs.Pickup.AlienMeat:
                    gameDataHolder.pickupData.alienMeat.Add(pickup.gameObject.transform.position);
                    break;
            }
        }
    }


    private void CollectGunDamageUpgradeEvent(GunDamageUpgradeEvent upgradeEvent)
    {
        gameDataHolder.soldierData.upgrades.weaponDamageUpgrades.Add(upgradeEvent.UpgradeAmount);
    }
    private void CollectGunFireRateUpgradeEvent(GunFireRateUpgradeEvent upgradeEvent)
    {
        gameDataHolder.soldierData.upgrades.weaponFireRateUpgrades.Add(upgradeEvent.UpgradePercent);
    }
    private void CollectTurretDamageUpgradeEvent(TurretDamageUpgradeEvent upgradeEvent)
    {
        gameDataHolder.engineerData.upgrades.turretDamageUpgrades.Add(upgradeEvent.UpgradeAmount);
    }
    private void CollectTurretHealthUpgradeEvent(TurretHealthUpgradeEvent upgradeEvent)
    {
        gameDataHolder.engineerData.upgrades.turretHealthUpgrades.Add(upgradeEvent.UpgradeAmount);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;
using Photon.Pun;

public class DataLoader
{
    public GameDataHolder GameDataHolder { get; set; }

    public void LoadLocalPlayerData(ref GameObject targetPlayer, PlayerData playerData)
    {
        targetPlayer.transform.position = playerData.position;

        HealthHandler healthHandler = targetPlayer.GetComponent<HealthHandler>();
        if (healthHandler)
        {
            healthHandler.MaxHealth = playerData.maxHealth;
            healthHandler.SetHealth(playerData.currentHealth);
        }

        InventorySystem inventorySystem = targetPlayer.GetComponent<InventorySystem>();
        inventorySystem.Initialize();
        if (inventorySystem)
        {
            inventorySystem.Add<AlienMeat>(playerData.inventory.alienMeat);
            inventorySystem.Add<Metal>(playerData.inventory.metal);
            inventorySystem.Add<GreenGoo>(playerData.inventory.greenGoo);
        }
        

        if (playerData.character == Character.ENGINEER)
        {
            LoadEngineerSpecificData(ref targetPlayer, playerData);
        }
        else
        {
            LoadSoldierSpecificData(ref targetPlayer, playerData);
        }
    }

    private void LoadEngineerSpecificData(ref GameObject targetPlayer, PlayerData playerData)
    {
        Engineer engineer = targetPlayer.GetComponent<Engineer>();

        foreach (PlayerData.TurretData turretData in playerData.turrets)
        {
            GameObject turret = engineer.CreateTurret(turretData.position);
            TurretHealthHandler turretHealthHandler = turret.GetComponent<TurretHealthHandler>();
            turretHealthHandler.CurrentHealth = turretData.currentHealth;
            turretHealthHandler.MaxHealth = turretData.maxHealth;
        }
        if (playerData.upgrades != null)
        {
            FireEngineerEvents(playerData);
        }
    }

    private void LoadSoldierSpecificData(ref GameObject targetPlayer, PlayerData playerData)
    {
        SoldierCharacter soldier = targetPlayer.GetComponent<SoldierCharacter>();
        soldier.weapon.SetAmmo(playerData.ammo);

        if (playerData.upgrades != null)
        {
            FireSoldierEvents(playerData.upgrades);
        }
    }

    private void FireEngineerEvents(PlayerData playerData)
    {
        foreach(int upgradeAmount in playerData.upgrades.turretDamageUpgrades)
        {
            EventSystem.Instance.FireEvent(new TurretDamageUpgradeEvent(upgradeAmount));
        }

        if (playerData.turrets.Count == 0)
        {
            foreach (int upgradeAmount in playerData.upgrades.turretHealthUpgrades)
            {
                EventSystem.Instance.FireEvent(new TurretHealthUpgradeEvent(upgradeAmount));
            }
        }
    }

    private void FireSoldierEvents(PlayerData.Upgrades playerUpgrades)
    {
        foreach (int upgradeAmount in playerUpgrades.weaponDamageUpgrades)
        {
            EventSystem.Instance.FireEvent(new GunDamageUpgradeEvent(upgradeAmount));
        }
        foreach (float upgradeAmount in playerUpgrades.weaponFireRateUpgrades)
        {
            EventSystem.Instance.FireEvent(new GunFireRateUpgradeEvent(upgradeAmount));
        }
    }

    public void LoadProgressData(ProgressData progressData)
    {
        EventStarter[] shipPickupEvents = GameObject.FindObjectsOfType<EventStarter>();
        Ship ship = GameObject.FindObjectOfType<Ship>();
        ship.Initialize();
        for (int index = 0; index < progressData.upgradeProgress.Count; index++)
        {
            ProgressData.ShipProgress shipProgress = progressData.upgradeProgress[index];
            ship.shipUpgradeCost[index].metalCost = shipProgress.metalCost;
            ship.shipUpgradeCost[index].gooCost = shipProgress.gooCost;
            ship.shipUpgradeCost[index].partAvalibul = shipProgress.partAvailable;
            GameObject attachedPart = FindChildObjectByName(ship.transform, "AttachedParts", shipProgress.partAttachedName);
            GameObject missingPart = FindChildObjectByName(ship.transform, "MissingParts", shipProgress.partMissingName);

            ship.shipUpgradeCost[index].partAttached = attachedPart;
            ship.shipUpgradeCost[index].partMissing = missingPart;
            attachedPart.SetActive(true);
            missingPart.SetActive(false);

            if (PhotonNetwork.IsMasterClient)
            {
                for (int index2 = 0; index2 < shipPickupEvents.Length; index2++)
                {
                    EventStarter shipPickupEvent = shipPickupEvents[index2];
                    if (shipPickupEvent.missingPart.name == shipProgress.partMissingName)
                    {
                        PhotonNetwork.Destroy(shipPickupEvent.gameObject);
                        break;
                    }
                }
            }
        }
        ship.nextUpgrade = progressData.upgradeLevel;
        EventSystem.Instance.FireEvent(new ShipUpgradeProgressionEvent(ship.nextUpgrade, ship.shipUpgradeCost.Count));
        Timer timer = GameObject.FindObjectOfType<Timer>();
        LightingManager lightingManager = GameObject.FindObjectOfType<LightingManager>();
        if (lightingManager)
        {
            lightingManager.TimeOfDay = progressData.timeOfDay;
            lightingManager.IsNight = progressData.isNight;
            lightingManager.TotalTimeWholeCycle = lightingManager.DayLength + lightingManager.NightLength;
            if (timer)
            {
                timer.LoadTime();
            }
            
            if (progressData.isNight && PhotonNetwork.IsMasterClient)
            {
                float nightTimeLeft = lightingManager.TotalTimeWholeCycle - lightingManager.TimeOfDay;
                if (nightTimeLeft < lightingManager.NightLength)
                {
                    lightingManager.SetupAndStartSpawning(nightTimeLeft);
                }

            }
        }

        Level1 level1 = GameObject.FindObjectOfType<Level1>();
        if (level1)
        {
            if(progressData.tutorialIsDone == true)
            {
                level1.TutorialIsDone = progressData.tutorialIsDone;
                level1.TutorialOver();
            }
        }
    }

    private GameObject FindChildObjectByName(Transform rootObject, string parentName, string objectName)
    {
        Transform parentTransform = rootObject.Find(parentName);
        return parentTransform.Find(objectName).gameObject;
    }
}

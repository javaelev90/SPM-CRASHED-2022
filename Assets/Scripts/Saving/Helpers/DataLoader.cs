using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (inventorySystem)
        {
            inventorySystem.Add<AlienMeat>(playerData.inventory.alienMeat);
            inventorySystem.Add<Metal>(playerData.inventory.metal);
            inventorySystem.Add<GreenGoo>(playerData.inventory.greenGoo);
        }

        if (playerData.character == Character.ENGINEER)
        {
            Engineer engineer = targetPlayer.GetComponent<Engineer>();

            foreach (PlayerData.TurretData turretData in playerData.turrets)
            {
                GameObject turret = engineer.CreateTurret();
                TurretHealthHandler turretHealthHandler = turret.GetComponent<TurretHealthHandler>();
                turret.transform.position = turretData.position;
                turretHealthHandler.CurrentHealth = turretData.currentHealth;
                turretHealthHandler.MaxHealth = turretData.maxHealth;
            }
        }
        else
        {
            SoldierCharacter soldier = targetPlayer.GetComponent<SoldierCharacter>();
            soldier.weapon.SetAmmo(playerData.ammo);
        }
    }

    //private void CollectPlayerData(PlayerData playerData, GameObject playerObject, Character character)
    //{
    //    playerData.character = character;

    //    HealthHandler healthHandler = playerObject.GetComponent<HealthHandler>();
    //    if (healthHandler)
    //    {
    //        playerData.currentHealth = healthHandler.CurrentHealth;
    //        playerData.maxHealth = healthHandler.MaxHealth;
    //    }

    //    InventorySystem inventorySystem = playerObject.GetComponent<InventorySystem>();
    //    if (inventorySystem)
    //    {
    //        playerData.inventory.alienMeat = inventorySystem.Amount<AlienMeat>();
    //        playerData.inventory.metal = inventorySystem.Amount<Metal>();
    //        playerData.inventory.greenGoo = inventorySystem.Amount<GreenGoo>();
    //    }

    //    if (playerData.character == Character.ENGINEER)
    //    {
    //        Turret[] turrets = GameObject.FindObjectsOfType<Turret>();
    //        PlayerData.TurretData turretData;
    //        HealthHandler turretHealthHandler;

    //        foreach (Turret turret in turrets)
    //        {
    //            turretHealthHandler = turret.gameObject.GetComponent<HealthHandler>();
    //            turretData = new PlayerData.TurretData
    //            {
    //                position = turret.transform.position,
    //                maxHealth = turretHealthHandler.MaxHealth,
    //                currentHealth = turretHealthHandler.CurrentHealth,
    //            };
    //            playerData.turrets.Add(turretData);
    //        }
    //    }
    //    else
    //    {
    //        playerData.ammo = GameManager.player.GetComponent<SoldierCharacter>().weapon.currentAmmo;
    //    }
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class PlayerData
{
    public Character character;
    public Inventory inventory;
    public Upgrades upgrades;
    public List<TurretData> turrets;
    public int currentHealth;
    public int maxHealth;
    public int ammo;
    public Vector3 position;

    public void Initialize()
    {
        inventory = new Inventory();
        upgrades = new Upgrades();
        upgrades.Initialize();
        turrets = new List<TurretData>();
    }

    [System.Serializable]
    public class Inventory
    {
        public int greenGoo;
        public int metal;
        public int alienMeat;
    }

    [System.Serializable]
    public class TurretData
    {
        public int currentHealth;
        public int maxHealth;
        public Vector3 position;
    }

    [System.Serializable]
    public class Upgrades
    {
        public List<int> turretDamageUpgrades;
        public List<int> turretHealthUpgrades;
        public List<int> weaponDamageUpgrades;
        public List<float> weaponFireRateUpgrades;

        public void Initialize()
        {
            turretDamageUpgrades = new List<int>();
            turretHealthUpgrades = new List<int>();
            weaponDamageUpgrades = new List<int>();
            weaponFireRateUpgrades = new List<float>();
        }
    }

}

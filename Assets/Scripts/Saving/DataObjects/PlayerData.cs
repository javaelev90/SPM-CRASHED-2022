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
    public float currentHealth;
    public float maxHealth;

    public PlayerData()
    {
        inventory = new Inventory();
        upgrades = new Upgrades();
    }

    [System.Serializable]
    public class Inventory
    {
        public int greenGoo;
        public int metal;
        public int alienMeat;
    }

    [System.Serializable]
    public class Upgrades
    {
        public List<int> turretDamageUpgrades;
        public List<int> turretHealthUpgrades;
        public List<int> weaponDamageUpgrades;
        public List<float> weaponFireRateUpgrades;

        public Upgrades()
        {
            turretDamageUpgrades = new List<int>();
            turretHealthUpgrades = new List<int>();
            weaponDamageUpgrades = new List<int>();
            weaponFireRateUpgrades = new List<float>();
        }
    }

}

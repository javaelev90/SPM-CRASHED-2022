using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class PlayerData
{
    public Character character;
    public Inventory inventory;
    public float currentHealth;

    [System.Serializable]
    public class Inventory
    {
        public int greenGoo;
        public int metal;
        public int alienMeat;
    }

}

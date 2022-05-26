using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressData
{
    public int upgradeLevel;
    public List<Ship.ShipUpgradeCost> upgradeProgress;
    public float timeOfDay;
    
    public ProgressData()
    {
        upgradeProgress = new List<Ship.ShipUpgradeCost>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressData
{
    // Ship progress
    public int upgradeLevel;
    public List<ShipProgress> upgradeProgress;

    // Lightmanager settings
    public float timeOfDay;
    public bool isNight;

    // Tutorial
    public bool tutorialIsDone;

    public void Initialize()
    {
        upgradeProgress = new List<ShipProgress>();
    }

    [System.Serializable]
    public class ShipProgress
    {
        public int metalCost;
        public int gooCost;
        public bool partAvailable;
        public string partMissingName;
        public string partAttachedName;
    }
}

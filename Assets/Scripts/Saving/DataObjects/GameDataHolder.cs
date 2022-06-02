using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameDataHolder
{
    public PlayerData soldierData;
    public PlayerData engineerData;
    public ProgressData progressData;
    public PickupData pickupData;

    public void Initialize()
    {
        progressData = new ProgressData();
        progressData.Initialize();
        soldierData = new PlayerData();
        soldierData.Initialize();
        engineerData = new PlayerData();
        engineerData.Initialize();
        //pickupData = new PickupData();
    }
}

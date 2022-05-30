using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameDataHolder
{
    public PlayerData playerData;
    public PlayerData otherPlayerData;
    public ProgressData progressData;
    public PickupData pickupData;

    public GameDataHolder()
    {
        progressData = new ProgressData();
        playerData = new PlayerData();
        otherPlayerData = new PlayerData();
        pickupData = new PickupData();
    }
}

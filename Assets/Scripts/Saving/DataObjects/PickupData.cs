using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PickupData
{
    public List<Vector3> greenGoo;
    public List<Vector3> metal;
    public List<Vector3> alienMeat;

    public PickupData()
    {
        greenGoo = new List<Vector3>();
        metal = new List<Vector3>();
        alienMeat = new List<Vector3>();
    }
}

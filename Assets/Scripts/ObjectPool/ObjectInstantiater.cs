using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectInstantiater : MonoBehaviourPunCallbacks
{
    [Header("Green Goo")]
    [SerializeField] GameObject greenGooPrefab;
    [SerializeField] List<Transform> greenGooPositions;
    [Header("Metal")]
    [SerializeField] GameObject metalPrefab;
    [SerializeField] List<Transform> metalPositions;
    [Header("Alien Meat")]
    [SerializeField] GameObject alienMeatPrefab;
    [SerializeField] List<Transform> alienMeatPositions;

    [SerializeField] string prefabPath = "Prefabs/Pickups/";

    public void InitializeWorld()
    {
        CreatePickups();
    }

    public void CreatePickups()
    {
        CreatePrefabs(greenGooPrefab, greenGooPositions);
        CreatePrefabs(metalPrefab, metalPositions);
        CreatePrefabs(alienMeatPrefab, alienMeatPositions);
    }

    private void CreatePrefabs(GameObject prefab, List<Transform> positions)
    {
        foreach (Transform location in positions)
        {
            PhotonNetwork.InstantiateRoomObject(prefabPath + prefab.name, location.position, location.rotation);
        }
    }
}

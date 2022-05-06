using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectInstantiater : MonoBehaviourPunCallbacks
{
    [Tooltip("How far above the terrain the resources should be instantiated.")]
    [SerializeField] private float yOffset = 1f;
    [Header("Green Goo")]
    [SerializeField] GameObject greenGooPrefab;
    [SerializeField] List<Transform> greenGooPositions;
    [Header("Metal")]
    [SerializeField] GameObject metalPrefab;
    [SerializeField] List<Transform> metalPositions;

    [SerializeField] string prefabPath = "Prefabs/Pickups/";

    public void InitializeWorld()
    {
        CreatePickups();
    }

    public void CreatePickups()
    {
        CreatePrefabs(greenGooPrefab, greenGooPositions);
        CreatePrefabs(metalPrefab, metalPositions);
    }

    private void CreatePrefabs(GameObject prefab, List<Transform> positions)
    {
        foreach (Transform location in positions)
        {
            float y = Terrain.activeTerrain.SampleHeight(new Vector3(location.position.x, 10f, location.position.z));
            Vector3 newLocation = new Vector3(location.position.x, y + yOffset, location.position.z);
            PhotonNetwork.InstantiateRoomObject(prefabPath + prefab.name, newLocation, location.rotation);
        }
    }
}

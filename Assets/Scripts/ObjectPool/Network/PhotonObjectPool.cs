using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonObjectPool : MonoBehaviourPunCallbacks
{
    [SerializeField] public GameObject pooledObjectPrefab;
    [SerializeField] private int maxPoolSize;
    public Queue<PooledObject> pooledObjects;
    public Dictionary<int, PooledObject> activeObjects;

    private void Start()
    {
        if (!pooledObjectPrefab.GetComponent<PooledObject>())
        {
            throw new MissingComponentException();
        }
        pooledObjects = new Queue<PooledObject>();
        activeObjects = new Dictionary<int, PooledObject>();

        LoadPool();
    }

    private void LoadPool()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        for(int i = 0; i < maxPoolSize; i++)
        {
            PooledObject pooledObject = Instantiate(
                    pooledObjectPrefab.transform.position,
                    Quaternion.identity
                );
            pooledObject.ObjectPool = this;
            pooledObject.UpdateActiveState(false);
            pooledObjects.Enqueue(pooledObject);
        }
    }

    private PooledObject Instantiate(Vector3 position, Quaternion rotation)
    {
        return PhotonNetwork.InstantiateRoomObject("Prefabs/Enemies/"+pooledObjectPrefab.name, position, rotation).GetComponent<PooledObject>();
    }

    public void Spawn(Vector3 position)
    {
        Debug.Log(position);
        if(pooledObjects.Count > 0)
        {
            photonView.RPC(nameof(MasterSpawn), RpcTarget.MasterClient, position);
        }
    }

    public void DeSpawn(int gameObjectPhotonId)
    {
        photonView.RPC(nameof(MasterDeSpawn), RpcTarget.MasterClient, gameObjectPhotonId);
    }

    [PunRPC]
    private void MasterSpawn(Vector3 position)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            Debug.LogError($"[PhotonObjectPool::MasterSpawn] Only the master is allowed to spawn objects.");
            return;
        }
        PooledObject pooledObject = pooledObjects.Dequeue();
        pooledObject.transform.position = position;
        activeObjects.Add(pooledObject.photonView.ViewID, pooledObject);
        pooledObject.UpdateActiveState(true);
    }

    [PunRPC]
    private void MasterDeSpawn(int gameObjectPhotonId)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            Debug.LogError($"[PhotonObjectPool::MasterDeSpawn] Only the master is allowed to despawn objects.");
            return;
        }
        if(activeObjects.TryGetValue(gameObjectPhotonId, out PooledObject pooledObject))
        {
            pooledObject.Recycle();
            pooledObject.UpdateActiveState(false);
            if (activeObjects.Remove(gameObjectPhotonId))
            {
                pooledObjects.Enqueue(pooledObject);
            }
        }
    }

}

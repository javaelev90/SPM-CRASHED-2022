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
        return PhotonNetwork.Instantiate("Prefabs/Enemies/"+pooledObjectPrefab.name, position, rotation).GetComponent<PooledObject>();
    }

    public void DeSpawnPool()
    {
        if (activeObjects.Count > 0)
        {
            photonView.RPC(nameof(DeSpawnPoolMaster), RpcTarget.MasterClient);
        }
    }

    public void Spawn(Vector3 position, int photonViewTargetId)
    {
        if(pooledObjects.Count > 0)
        {
            photonView.RPC(nameof(MasterSpawn), RpcTarget.MasterClient, position, photonViewTargetId);
        }
    }

    public void DeSpawn(int gameObjectPhotonId)
    {
        photonView.RPC(nameof(MasterDeSpawn), RpcTarget.MasterClient, gameObjectPhotonId);
    }

    [PunRPC]
    private void MasterSpawn(Vector3 position, int photonViewTargetId)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            Debug.LogError($"[PhotonObjectPool::MasterSpawn] Only the master is allowed to spawn objects.");
            return;
        }
        PooledObject pooledObject = pooledObjects.Dequeue();
        pooledObject.transform.position = position;
        pooledObject.transform.SetParent(transform);
        pooledObject.ObjectPool = this;
        pooledObject.photonViewTargetId = photonViewTargetId;
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

    [PunRPC]
    private void DeSpawnPoolMaster()
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            Debug.LogError($"[PhotonObjectPool::DeSpawnPoolMaster] Only the master is allowed to spawn objects.");
            return;
        }
        foreach (var item in activeObjects)
        {
            item.Value.Recycle();
            item.Value.UpdateActiveState(false);
            if (activeObjects.Remove(item.Key))
            {
                pooledObjects.Enqueue(item.Value);
            }
        }
    }

}

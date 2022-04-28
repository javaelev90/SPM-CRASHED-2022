using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonObjectPool : MonoBehaviourPunCallbacks
{
    [SerializeField] public GameObject pooledObjectPrefab;
    [SerializeField] private int maxPoolSize;
    public List<PooledObject> pooledObjects;
    public Dictionary<int, PooledObject> activeObjects;
    //private DefaultPool objectPool;

    private void Start()
    {
        pooledObjects = new List<PooledObject>();
        activeObjects = new Dictionary<int, PooledObject>();

        //objectPool = PhotonNetwork.PrefabPool as DefaultPool;
        
        //if (objectPool != null && pooledObjectPrefab != null)
        //{
        //    objectPool.ResourceCache.Add(pooledObjectPrefab.name, pooledObjectPrefab);
        //}
        LoadPool();
    }

    private void LoadPool()
    {
        for(int i = 0; i < maxPoolSize; i++)
        {
            PooledObject pooledObject = Instantiate(
                    pooledObjectPrefab.transform.position,
                    Quaternion.identity,
                    null
                );
            pooledObject.ObjectPool = this;
            //pooledObject.gameObject.SetActive(false);
            pooledObject.photonView.RPC("UpdateActiveState", RpcTarget.All, false);
            pooledObjects.Add(pooledObject);
        }
    }

    private PooledObject Instantiate(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        return PhotonNetwork.Instantiate("Prefabs/" +pooledObjectPrefab.name, position, rotation).GetComponent<PooledObject>();
    }

    public void Spawn(Vector3 position)
    {
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
        PooledObject pooledObject = pooledObjects[0];
        pooledObject.photonView.RPC("UpdateActiveState", RpcTarget.All, true);
        pooledObject.transform.position = position;
        pooledObjects.RemoveAt(0);
        activeObjects.Add(pooledObject.gameObject.GetPhotonView().ViewID, pooledObject);
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
            pooledObject.photonView.RPC("UpdateActiveState", RpcTarget.All, false);
        }
    }

}

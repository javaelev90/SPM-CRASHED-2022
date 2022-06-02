using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class PhotonObjectPool : MonoBehaviourPunCallbacks
{
    [SerializeField] public PooledObject pooledObjectPrefab;
    [SerializeField] private int maxPoolSize;
    public Queue<PooledObject> pooledObjects = new Queue<PooledObject>();
    public Dictionary<int, PooledObject> activeObjects = new Dictionary<int, PooledObject>();

    private void Awake()
    {
        LoadPool();
    }

    private void LoadPool()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        for(int i = 0; i < maxPoolSize; i++)
        {
            if (pooledObjectPrefab.navMeshAgent != null)
            {
                pooledObjectPrefab.navMeshAgent.enabled = false;
            }
            PooledObject pooledObject = Instantiate(
                    pooledObjectPrefab.transform.position,
                    Quaternion.identity
                );
            pooledObjects.Enqueue(pooledObject);
        }
    }

    private PooledObject Instantiate(Vector3 position, Quaternion rotation)
    {

        return PhotonNetwork.Instantiate(GlobalSettings.EnemiesPath + pooledObjectPrefab.name, position, rotation, 0, new object[] { photonView.ViewID }).GetComponent<PooledObject>();
    }

    public void DeSpawnPool()
    {
        if (activeObjects.Count > 0)
        {
            photonView.RPC(nameof(DeSpawnPoolMaster), RpcTarget.MasterClient);
        }
    }

    public void Spawn(Vector3 position, Quaternion rotation, int photonViewTargetId)
    {
        if(pooledObjects.Count > 0)
        {
            photonView.RPC(nameof(MasterSpawn), RpcTarget.MasterClient, position, rotation, photonViewTargetId);
        }
    }

    public void SpawnWithParameters(Vector3 position, Quaternion rotation, int photonViewTargetId, object[] parameters)
    {
        if (pooledObjects.Count > 0)
        {
            photonView.RPC(nameof(MasterSpawnWithParameters), RpcTarget.MasterClient, position, rotation, photonViewTargetId, parameters);
        }
    }

    public void DeSpawn(int gameObjectPhotonId)
    {
        photonView.RPC(nameof(MasterDeSpawn), RpcTarget.MasterClient, gameObjectPhotonId);
    }

    [PunRPC]
    private void MasterSpawn(Vector3 position, Quaternion rotation, int photonViewTargetId)
    {
        MasterSpawnHelper(position, rotation, photonViewTargetId, null, nameof(MasterSpawn));
    }

    [PunRPC]
    private void MasterSpawnWithParameters(Vector3 position, Quaternion rotation, int photonViewTargetId, object[] parameters)
    {
        MasterSpawnHelper(position, rotation, photonViewTargetId, parameters, nameof(MasterSpawnWithParameters));
    }

    [PunRPC]
    private void MasterDeSpawn(int gameObjectPhotonId)
    {
        if(activeObjects.TryGetValue(gameObjectPhotonId, out PooledObject pooledObject))
        {
            MasterDespawnHelper(gameObjectPhotonId, pooledObject, nameof(MasterDeSpawn));
        }
    }

    [PunRPC]
    private void DeSpawnPoolMaster()
    {
        foreach (var item in activeObjects)
        {
            MasterDespawnHelper(item.Key, item.Value, nameof(DeSpawnPoolMaster));
        }
    }

    private void MasterDespawnHelper(int gameObjectPhotonId, PooledObject pooledObject, string methodName)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            Debug.LogError($"[PhotonObjectPool::{methodName}] Only the master is allowed to despawn objects.");
            return;
        }
        pooledObject.Recycle();
        pooledObject.UpdateActiveState(false);
        if (activeObjects.Remove(gameObjectPhotonId))
        {
            pooledObjects.Enqueue(pooledObject);
        }
    }

    private void MasterSpawnHelper(Vector3 position, Quaternion rotation, int photonViewTargetId, object[] parameters, string methodName)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            Debug.LogError($"[PhotonObjectPool::{methodName}] Only the master is allowed to spawn objects.");
            return;
        }
        PooledObject pooledObject = pooledObjects.Dequeue();
        pooledObject.transform.SetParent(transform);
        pooledObject.transform.SetPositionAndRotation(position, rotation);
        // Set transform parameters
        if (pooledObject.navMeshAgent != null)
        {
            SpawnOnNavMesh(position, pooledObject.navMeshAgent);
            //pooledObject.photonView.RPC(nameof(pooledObject.SpawnOnNavMesh), RpcTarget.All, position);
        }

        // Set initialization parameters
        pooledObject.ObjectPool = this;
        //pooledObject.photonView.RPC(nameof(pooledObject.SetTargetViewId), RpcTarget.All, photonViewTargetId);
        pooledObject.photonViewTargetId = photonViewTargetId;

        if (parameters != null)
        {
            pooledObject.Initialize(parameters);
        }
        
        // Activate object
        activeObjects.Add(pooledObject.photonView.ViewID, pooledObject);

        // This will be used when culling is fully implemented
        if (pooledObject.shouldBeCulled == false)
        {
            pooledObject.UpdateActiveState(true);
        }
        //if (pooledObject.shouldBeCulled == true)
        //{
        //    pooledObject.UpdateActiveState(true);
        //}
    }

    private void SpawnOnNavMesh(Vector3 spawnPosition, NavMeshAgent navMeshAgent)
    {
        navMeshAgent.Warp(GetPositionOnNavMesh(spawnPosition));
        navMeshAgent.enabled = true;
    }

    private Vector3 GetPositionOnNavMesh(Vector3 spawnPosition)
    {
        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 10f, -1))
        {
            return hit.position;
        }
        return spawnPosition;
    }
}

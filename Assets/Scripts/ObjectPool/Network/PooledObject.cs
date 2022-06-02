using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using UnityEngine.AI;

public class PooledObject : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IRecycleable
{
    public enum RecyclingBehavior
    {
        Nothing,
        ThisTransform,
        Everything,
        EverythingAndCustom
    }

    // Customization functions for resetting/initializing a pooled object.
    // These are set by the object extending or using the PooledObject class.
    public Action CustomRecycleFunction { get; set; }
    public Action<object[]> CustomInitializeFunction;

    [Header("Pooling preferences")]
    public RecyclingBehavior recyclingBehaviour;
    public PhotonObjectPool ObjectPool { get; set; }

    [Header("If object hierarchy is using a NavMeshAgent")]
    public NavMeshAgent navMeshAgent;

    [Header("Initialization preferences")]
    public int photonViewTargetId = -1;

    [Header("Performance settings")]
    public bool shouldBeCulled = true;

    public void Initialize(object[] parameters)
    {
        CustomInitializeFunction?.Invoke(parameters);
    }

    public void DeSpawn()
    {
        ObjectPool.DeSpawn(photonView.ViewID);
    }

    public void Recycle()
    {
        photonView.RPC(nameof(RecycleRPC), RpcTarget.All);
    }

    [PunRPC]
    private void RecycleRPC()
    {
        switch(recyclingBehaviour)
        {
            case RecyclingBehavior.Nothing:
                break;
            case RecyclingBehavior.ThisTransform:
                RecycleTransform();
                break;
            case RecyclingBehavior.Everything:
                RecycleUtils.ResetGameObjectComponents(ObjectPool.pooledObjectPrefab.transform, transform);
                break;
            case RecyclingBehavior.EverythingAndCustom:
                RecycleUtils.ResetGameObjectComponents(ObjectPool.pooledObjectPrefab.transform, transform);
                RecycleCustom();
                break;
        }
        photonViewTargetId = -1;
    }

    public void UpdateActiveState(bool active)
    {
        photonView.RPC(nameof(UpdateActiveStateRPC), RpcTarget.All, active);
    }

    [PunRPC]
    private void UpdateActiveStateRPC(bool active)
    {
        gameObject.SetActive(active);
    }

    private void RecycleTransform()
    {
        transform.localScale = ObjectPool.pooledObjectPrefab.transform.localScale;
        transform.rotation = ObjectPool.pooledObjectPrefab.transform.rotation;
    }

    private void RecycleCustom()
    {
        CustomRecycleFunction?.Invoke();
    }

    [PunRPC]
    public void SetTargetViewId(int id)
    {
        photonViewTargetId = id;
    }

    [PunRPC]
    public void SpawnOnNavMesh(Vector3 spawnPosition)
    {
        if (NavMesh.SamplePosition(spawnPosition, out NavMeshHit hit, 10f, -1))
        {
            spawnPosition = hit.position;
        }

        navMeshAgent.Warp(spawnPosition);
        navMeshAgent.enabled = true;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        ObjectPool = PhotonView.Find((int)info.photonView.InstantiationData[0]).gameObject.GetComponent<PhotonObjectPool>();
        transform.SetParent(ObjectPool.transform);
        gameObject.SetActive(false);
    }

    public override bool Equals(object obj)
    {
        if (this == null) return false;
        if (obj == null) return false;

        return obj is PooledObject @object &&
               photonView.GetInstanceID() == @object.photonView.GetInstanceID();
    }

    public override int GetHashCode()
    {
        if(this == null) return 0;

        int hashCode = -164824088;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + (photonView != null ? photonView.GetHashCode() : 1);
        return hashCode;
    }
}

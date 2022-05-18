using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using System.Reflection;

public class PooledObject : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IRecycleable
{
    public enum RecyclingBehavior
    {
        Nothing,
        Transform,
        Custom,
        CustomAndTransform
    }

    public RecyclingBehavior recyclingBehaviour;
    public PhotonObjectPool ObjectPool { get; set; }
    public Action CustomRecycleFunction { get; set; }
    public Action<object[]> CustomInitializeFunction;
    public int photonViewTargetId = -1;
    public int photonGroup = 0;
    private void Start()
    {
        photonGroup = photonView.Group;
    }

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
            case RecyclingBehavior.Transform:
                RecycleUtils.ResetGameObjectComponents(ObjectPool.pooledObjectPrefab.transform, transform);
                break;
            case RecyclingBehavior.Custom:
                RecycleCustom();
                RecycleUtils.ResetGameObjectComponents(ObjectPool.pooledObjectPrefab.transform, transform);
                break;
            case RecyclingBehavior.CustomAndTransform:
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

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        ObjectPool = PhotonView.Find((int)info.photonView.InstantiationData[0]).gameObject.GetComponent<PhotonObjectPool>();
        transform.SetParent(ObjectPool.transform);
        gameObject.SetActive(false);
    }
}

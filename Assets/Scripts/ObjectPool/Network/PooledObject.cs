using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class PooledObject : MonoBehaviourPunCallbacks, IRecycleable
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
    public List<PooledObjectPhotonView> photonViewObjects;
    public int photonViewTargetId = -1;

    public void Recycle()
    {
        switch(recyclingBehaviour)
        {
            case RecyclingBehavior.Nothing:
                break;
            case RecyclingBehavior.Transform:
                RecycleTransform();
                RecycleChildObject();
                break;
            case RecyclingBehavior.Custom:
                RecycleCustom();
                RecycleChildObject();
                break;
            case RecyclingBehavior.CustomAndTransform:
                RecycleTransform();
                RecycleCustom();
                RecycleChildObject();
                break;
        }
        photonViewTargetId = -1;
    }

    public void UpdateActiveState(bool active)
    {
        photonView.RPC(nameof(UpdateActiveStateRPC), RpcTarget.All, active);
    }

    public void DeSpawn()
    {
        ObjectPool.DeSpawn(photonView.ViewID);
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

    private void RecycleChildObject()
    {
        foreach (PooledObjectPhotonView pooledObjectPhotonView in photonViewObjects)
        {
            pooledObjectPhotonView.Recycle();
        }
    }
}

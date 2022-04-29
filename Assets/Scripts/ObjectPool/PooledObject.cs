using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class PooledObject : MonoBehaviour
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
    public PhotonView photonViewObject;

    public void Recycle()
    {
        switch(recyclingBehaviour)
        {
            case RecyclingBehavior.Nothing:
                break;
            case RecyclingBehavior.Transform:
                RecycleTransform();
                break;
            case RecyclingBehavior.Custom:
                RecycleCustom();
                break;
            case RecyclingBehavior.CustomAndTransform:
                RecycleTransform();
                RecycleCustom();
                break;
        }
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

    public void UpdateActiveState(bool active)
    {
        photonViewObject.RPC("UpdateActiveState", RpcTarget.All, active);
    }

}

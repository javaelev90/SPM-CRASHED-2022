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
    public List<PooledObjectPhotonView> photonViewObjects;
    public int photonViewTargetId = -1;

    

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
                //RecycleTransform();
                ResetGameObjectComponents(ObjectPool.pooledObjectPrefab.transform, transform);
                break;
            case RecyclingBehavior.Custom:
                RecycleCustom();
                ResetGameObjectComponents(ObjectPool.pooledObjectPrefab.transform, transform);
                break;
            case RecyclingBehavior.CustomAndTransform:
                //RecycleTransform();
                ResetGameObjectComponents(ObjectPool.pooledObjectPrefab.transform, transform);
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

    protected void ResetGameObjectComponents(Transform prefab, Transform destination)
    {
        ResetComponents(prefab.gameObject, destination.gameObject);
        for (int childIndex = 0; childIndex < prefab.childCount; childIndex++)
        {
            ResetGameObjectComponents(prefab.GetChild(childIndex), destination.GetChild(childIndex));
        }
    }

    protected void ResetComponents(GameObject prefab, GameObject destination)
    {
        Component[] prefabComponents = prefab.GetComponents<Component>();
        Component[] destinationComponents = destination.GetComponents<Component>();
        for (int componentIndex = 0; componentIndex < prefabComponents.Length; componentIndex++)
        {
            if (prefabComponents[componentIndex] is PhotonView)
            {
                continue;
            } 
            else if (prefabComponents[componentIndex] is PhotonTransformViewClassic
                || prefabComponents[componentIndex] is PhotonTransformView)
            {
                destination.transform.SetPositionAndRotation(prefab.transform.position, prefab.transform.rotation);
                destination.transform.localScale = prefab.transform.localScale;
                continue;
            }
            else if(prefabComponents[componentIndex] is ParticleSystem)
            {
                ((ParticleSystem)destinationComponents[componentIndex]).Stop();
                ((ParticleSystem)destinationComponents[componentIndex]).Clear();
            }
            ResetPublicValues(
                    prefabComponents[componentIndex],
                    destinationComponents[componentIndex]
            );

        }
    }

    protected void ResetPublicValues(Component source, Component destination)
    {
        Type type = source.GetType();
        FieldInfo[] fields = type.GetFields();
        foreach (FieldInfo field in fields)
        {
            if (field.IsPublic && field.FieldType.IsValueType)
            {
                field.SetValue(destination, field.GetValue(source));
            }
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        ObjectPool = PhotonView.Find((int)info.photonView.InstantiationData[0]).gameObject.GetComponent<PhotonObjectPool>();
        transform.SetParent(ObjectPool.transform);
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LocalPooledObject : MonoBehaviour
{
    public enum RecyclingBehavior
    {
        Nothing,
        Transform,
        Custom,
        CustomAndTransform
    }

    public RecyclingBehavior recyclingBehaviour;
    public LocalObjectPool ObjectPool { get; set; }
    public Action CustomRecycleFunction { get; set; }

    public void Recycle()
    {
        switch (recyclingBehaviour)
        {
            case RecyclingBehavior.Nothing:
                break;
            case RecyclingBehavior.Transform:
                RecycleTransform();
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
    }

    public void UpdateActiveState(bool active)
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
}

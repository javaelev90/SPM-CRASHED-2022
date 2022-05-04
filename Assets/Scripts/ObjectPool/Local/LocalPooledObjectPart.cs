using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LocalPooledObjectPart : MonoBehaviour
{
    [SerializeField] public LocalPooledObject rootObject;
    public Action customRecyclingFunction;

    public void Recycle()
    {
        customRecyclingFunction?.Invoke();
    }

    public void UpdateActiveState(bool active)
    {
        rootObject.gameObject.SetActive(active);
    }

}

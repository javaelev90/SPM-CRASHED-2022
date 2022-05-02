using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalObjectPool : MonoBehaviour
{
    [SerializeField] public GameObject pooledObjectPrefab;
    [SerializeField] private int maxPoolSize;
    public Queue<PooledObject> pooledObjects;
    public Dictionary<int, PooledObject> activeObjects;

   

}

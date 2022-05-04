using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalObjectPool : MonoBehaviour
{
    [SerializeField] public GameObject pooledObjectPrefab;
    [SerializeField] private int maxPoolSize;
    public Queue<LocalPooledObject> pooledObjects;

    private void Awake()
    {
        if (!pooledObjectPrefab.GetComponent<LocalPooledObject>())
        {
            throw new MissingComponentException();
        }
        pooledObjects = new Queue<LocalPooledObject>();

        LoadPool();
    }

    private void LoadPool()
    {
        for (int i = 0; i < maxPoolSize; i++)
        {
            LocalPooledObject pooledObject = Instantiate(
                    pooledObjectPrefab.transform.position,
                    Quaternion.identity
                );
            pooledObject.ObjectPool = this;
            pooledObject.UpdateActiveState(false);
            pooledObjects.Enqueue(pooledObject);
        }
    }

    private LocalPooledObject Instantiate(Vector3 position, Quaternion rotation)
    {
        return Object.Instantiate(pooledObjectPrefab, position, rotation).GetComponent<LocalPooledObject>();
    }

    public void Spawn(Vector3 position)
    {
        LocalPooledObject pooledObject = pooledObjects.Dequeue();
        pooledObject.transform.position = position;
        pooledObject.UpdateActiveState(true);
    }

    public void DeSpawn(GameObject pooledObject)
    {
        LocalPooledObject localPooledObject = pooledObject.GetComponent<LocalPooledObject>();
        if (localPooledObject)
        {
            localPooledObject.Recycle();
            localPooledObject.UpdateActiveState(false);
            pooledObjects.Enqueue(localPooledObject);
        }
    }
}

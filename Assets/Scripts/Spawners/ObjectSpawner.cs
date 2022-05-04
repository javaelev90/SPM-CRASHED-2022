using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Object pool")]
    [Tooltip("The spawner will use this pool to get objects to spawn")]
    [SerializeField] private PhotonObjectPool objectPool;
    [Header("Spawner settings")]
    [Range(0, 80)] [SerializeField] int numberToSpawn = 0;
    [SerializeField] private float delayBetweenSpawns = 1f;
    [Range(0, 60)] [SerializeField] private float spawnRadius = 1f;
    [Tooltip("If checked objects will be spawned in radius, otherwise they will be spawned on position.")]
    [SerializeField] private bool spawnWithinRadius = true;

    private float cooldownCounter = 0f;
    private float yOffset = 1f;
    private int spawnedObjects = 0;
    private bool spawnerIsTriggered = false;

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && spawnerIsTriggered)
        {
            SpawnObjects();
        }
    }

    public void ResetSpawner()
    {
        spawnedObjects = 0;
        spawnerIsTriggered = false;
    }

    public void TriggerSpawner()
    {
        spawnerIsTriggered = true;
    }

    private void SpawnObjects()
    {
        cooldownCounter += Time.deltaTime;
        if (cooldownCounter > delayBetweenSpawns && spawnedObjects < numberToSpawn)
        {
            if (spawnWithinRadius)
            {
                SpawnObjectsWithinRadius();
            }
            else
            {
                SpawnObjectsOnPosition();
            }

            cooldownCounter = 0;
            spawnedObjects++;
        }
        if (spawnedObjects == numberToSpawn)
        {
            ResetSpawner();
        }
    }

    private void SpawnMultipleObjects(int numberToSpawn)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int spawned = 0;
        while (spawned < numberToSpawn)
        {
            SpawnObjectsWithinRadius();
            spawned++;
        }
    }

    private void SpawnObjectsWithinRadius()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Vector3 xzPosition = Random.insideUnitCircle * spawnRadius;
        float y = Terrain.activeTerrain.SampleHeight(new Vector3(transform.position.x + xzPosition.x, 0f, transform.position.z + xzPosition.z));
        Vector3 spawnPosition = new Vector3(transform.position.x + xzPosition.x, y + yOffset, transform.position.z + xzPosition.z);
        objectPool.Spawn(spawnPosition);
    }

    private void SpawnObjectsOnPosition()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        float y = Terrain.activeTerrain.SampleHeight(new Vector3(transform.position.x, 0f, transform.position.z));
        Vector3 spawnPosition = new Vector3(transform.position.x, y + yOffset, transform.position.z);
        objectPool.Spawn(spawnPosition);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}

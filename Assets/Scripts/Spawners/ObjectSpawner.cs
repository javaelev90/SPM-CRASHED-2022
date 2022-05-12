using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using System.Linq;

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
    [Header("Initial Target")]
    [SerializeField] PhotonView initialTarget;

    [Header("Custom waypoint settings")]
    [SerializeField] List<Transform> wayPoints;
    [SerializeField] float wayPointSpawnRadius = 4f;

    private float cooldownCounter = 0f;
    [SerializeField] private float yOffset = 1f;
    private int spawnedObjects = 0;
    public bool spawnerIsTriggered = true;
    private int photonViewTargetId = -1;

    public float TotalSpawnDuration { set { delayBetweenSpawns = value / numberToSpawn; } }

    private void Start()
    {
        if (initialTarget)
        {
            photonViewTargetId = initialTarget.ViewID;
        }
        cooldownCounter = delayBetweenSpawns;
    }

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

        if (wayPoints.Count > 0)
        {
            objectPool.SpawnWithParameters(spawnPosition, transform.rotation, photonViewTargetId, new object[] { wayPointSpawnRadius, GetWayPointPositions().ToArray() });

        }
        else
        {
            objectPool.Spawn(spawnPosition, transform.rotation, photonViewTargetId);
        }
    }

    private void SpawnObjectsOnPosition()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        float y = Terrain.activeTerrain.SampleHeight(new Vector3(transform.position.x, 0f, transform.position.z));
        Vector3 spawnPosition = new Vector3(transform.position.x, y + yOffset, transform.position.z);

        if(wayPoints.Count > 0)
        {
            objectPool.SpawnWithParameters(spawnPosition, transform.rotation, photonViewTargetId, new object[] { wayPointSpawnRadius, GetWayPointPositions().ToArray() });

        }
        else
        {
            objectPool.Spawn(spawnPosition, transform.rotation, photonViewTargetId);
        }
    }

    private List<object> GetWayPointPositions()
    {
        List<object> wayPointPositions = new List<object>();
        foreach (Transform wayPoint in wayPoints)
        {
            wayPointPositions.Add(wayPoint.position);
        }
        return wayPointPositions;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightSpawnersHandler : MonoBehaviour
{
    [Header("Spawners")]
    [SerializeField] private ObjectSpawner[] spawners;

    // Start is called before the first frame update
    public void SetupSpawners(float spawnDuration)
    {
        foreach(ObjectSpawner spawner in spawners)
        {
            spawner.TotalSpawnDuration = spawnDuration;
            spawner.ResetSpawner();
        }
    }

    public void StartNightSpawning()
    {
        int numberOfSpawnsStarted = 0;
        foreach(ObjectSpawner spawner in spawners)
        {
            numberOfSpawnsStarted++;
            spawner.TriggerSpawner();
        }
    }

    public void StopNightSpawning()
    {
        int numberOfSpawnsStopped = 0;
        foreach (ObjectSpawner spawner in spawners)
        {
            numberOfSpawnsStopped++;
            spawner.ResetSpawner();
        }
    }
}

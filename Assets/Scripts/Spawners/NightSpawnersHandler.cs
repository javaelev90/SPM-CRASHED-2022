using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightSpawnersHandler : MonoBehaviour
{
    [Header("Spawners")]
    [SerializeField] private ObjectSpawner[] spawners;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartNightSpawning()
    {
        foreach(ObjectSpawner spawner in spawners)
        {
            spawner.TriggerSpawner();
        }
    }

    public void StopNightSpawning()
    {
        foreach (ObjectSpawner spawner in spawners)
        {
            spawner.ResetSpawner();
        }
    }
}

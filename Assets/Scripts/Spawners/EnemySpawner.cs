using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private PhotonObjectPool objectPool;
    [Range(0, 80)] [SerializeField] int numberToSpawn = 0;
    [SerializeField] private float delayBetweenSpawns = 1f;
    private SphereCollider spawnerBounds;
    private float cooldownCounter = 0f;
    private float yOffset = 1f;

    private bool resetpress = false;

    private void Start()
    {
        spawnerBounds = GetComponent<SphereCollider>();
        //objectPool = FindObjectOfType<PhotonObjectPool>();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            cooldownCounter += Time.deltaTime;
            if(cooldownCounter > delayBetweenSpawns && Keyboard.current.tKey.isPressed && !resetpress)
            {
                //if (spawnedEnemies < numberToSpawn)
                //{
                SpawnEnemies(numberToSpawn);
                //}
                //cooldownCounter = 0;
                resetpress = true;
            }
            else if (!Keyboard.current.tKey.isPressed && resetpress)
            {
                resetpress = false;
            }
        }
    }

    private void SpawnEnemies(int numberToSpawn)
    {
        int spawned = 0;
        while (spawned < numberToSpawn)
        {
            SpawnEnemy();
            spawned++;
        }
    }

    private void SpawnEnemy()
    {
        float x = Random.Range(spawnerBounds.bounds.min.x, spawnerBounds.bounds.max.x);
        float z = Random.Range(spawnerBounds.bounds.min.z, spawnerBounds.bounds.max.z);
        float y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0f, z));
        Vector3 spawnPosition = new Vector3(x, y + yOffset, z);
        objectPool.Spawn(spawnPosition);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius);
    }
}

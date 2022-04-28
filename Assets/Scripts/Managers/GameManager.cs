using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonObjectPool objectPool;
    List<PooledObject> pooledObjects;
    float timeDelay = 0.5f;
    float timer = 0f;
    void Start()
    {
        Debug.Log($"Oh no, you chose the {(Character)PlayerPrefs.GetInt(GlobalSettings.GameSettings.CharacterChoicePropertyName)} charater");
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            timer += Time.deltaTime;
            if (Keyboard.current.rKey.wasPressedThisFrame && timer > timeDelay)
            {
                float x = Random.Range(1, 5);
                float z = Random.Range(1, 5);
                objectPool.Spawn(new Vector3(x, 1f, z));
                timer = 0;
            }
            if (Keyboard.current.tKey.wasPressedThisFrame && timer > timeDelay)
            {
                pooledObjects = new List<PooledObject>(FindObjectsOfType<PooledObject>());
                pooledObjects.RemoveAll(pooledObject => !pooledObject.gameObject.activeSelf);
                
                if (pooledObjects.Count > 0)
                {
                    objectPool.DeSpawn(pooledObjects[0].photonView.ViewID);
                }
                timer = 0;
            }
        }
       
    }
}

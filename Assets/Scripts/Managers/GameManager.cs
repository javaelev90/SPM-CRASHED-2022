using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject objectPool;
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private Transform spawnPoint;
    private bool IsMine { get { return photonView.IsMine; } }

    private void Awake()
    {
        Initialize();
    }

    void Start()
    {
        Debug.Log($"Oh no, you chose the {(Character)PlayerPrefs.GetInt(GlobalSettings.GameSettings.CharacterChoicePropertyName)} charater");
    }

    private void Initialize()
    {
        
        PhotonNetwork.Instantiate("Prefabs/" + characterPrefab.name, spawnPoint.position, spawnPoint.rotation);

        if(PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.InstantiateRoomObject("Prefabs/" + objectPool.name, Vector3.zero, Quaternion.identity);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            
        }
       
    }
}

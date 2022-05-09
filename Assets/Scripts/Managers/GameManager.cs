using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Ship ship; 
    [SerializeField] private GameObject objectPool;
    [SerializeField] private GameObject soldierPrefab;
    [SerializeField] private GameObject engineerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private ObjectInstantiater objectInstantiater;
    public static GameObject playerObject;

    private Character character;

    private bool IsMine { get { return photonView.IsMine; } }
    private bool gameIsOver = false;
    private void Awake()
    {
        character = (Character)PlayerPrefs.GetInt(GlobalSettings.GameSettings.CharacterChoicePropertyName);
        Initialize();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (ship.allShipPartsCollected && gameIsOver == false)
            {
                gameIsOver = true;
                PhotonNetwork.LoadLevel(GlobalSettings.GameSettings.WinSceneName);
            }
        }

    }

    void Start()
    {
        Debug.Log($"Oh no, you chose the {character} charater");
    }

    private void Initialize()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            objectInstantiater.InitializeWorld();
        }

        if (character == Character.SOLDIER)
        {
            playerObject = PhotonNetwork.Instantiate("Prefabs/" + soldierPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            playerObject = PhotonNetwork.Instantiate("Prefabs/" + engineerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }

    }

}

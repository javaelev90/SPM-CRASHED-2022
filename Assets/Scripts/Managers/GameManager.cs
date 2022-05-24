using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Ship ship; 
    [SerializeField] private GameObject soldierPrefab;
    [SerializeField] private GameObject engineerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private ObjectInstantiater objectInstantiater;
    [SerializeField] private ObjectCulling objectCulling;
    public static GameObject playerObject;

    public static Character character;

    public GameObject loadScene;

    private bool IsMine { get { return photonView.IsMine; } }
    private bool gameIsOver = false;
    
    private void Awake()
    {
        character = (Character)PlayerPrefs.GetInt(GlobalSettings.GameSettings.CharacterChoicePropertyName);
        Initialize();
        Debug.Log($"Oh no, you chose the {character} charater");
        Destroy(loadScene, 10);

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

    private void Initialize()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.SetInterestGroups(1, true);
            //PhotonNetwork.SetInterestGroups(2, true);
            //PhotonNetwork.SetSendingEnabled(0, true);
            //PhotonNetwork.SetSendingEnabled(1, true);
            //PhotonNetwork.SetSendingEnabled(2, true);

            objectInstantiater.InitializeWorld();
        }
        else
        {
            //PhotonNetwork.SetInterestGroups(1, false);
            //PhotonNetwork.SetInterestGroups(2, true);
            //PhotonNetwork.SetInterestGroups(new byte[] { 1 }, new byte[] { 2 });
        }

        if (character == Character.SOLDIER)
        {
            playerObject = PhotonNetwork.Instantiate(GlobalSettings.PlayerCharacterPath + soldierPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            playerObject = PhotonNetwork.Instantiate(GlobalSettings.PlayerCharacterPath + engineerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    objectCulling.Initialize(playerObject, character);
        //}

    }
}

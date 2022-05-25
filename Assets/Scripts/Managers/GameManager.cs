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
    [SerializeField] public ObjectCulling objectCulling;
    public static GameObject player;
    public static GameObject otherPlayer;

    public static Character character;

    private bool IsMine { get { return photonView.IsMine; } }
    private bool gameIsOver = false;
    
    private void Awake()
    {
        character = (Character)PlayerPrefs.GetInt(GlobalSettings.GameSettings.CharacterChoicePropertyName);
        Initialize();
        Debug.Log($"Oh no, you chose the {character} charater");
        StartCoroutine(FindOtherPlayer(character));
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

    IEnumerator FindOtherPlayer(Character character)
    {
        while (otherPlayer == null)
        {
            if (character == Character.SOLDIER)
                otherPlayer = FindObjectOfType<Engineer>()?.gameObject;
            else
                otherPlayer = FindObjectOfType<SoldierCharacter>()?.gameObject;

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void Initialize()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            objectInstantiater.InitializeWorld();
        }


        if (character == Character.SOLDIER)
        {
            player = PhotonNetwork.Instantiate(GlobalSettings.PlayerCharacterPath + soldierPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            player = PhotonNetwork.Instantiate(GlobalSettings.PlayerCharacterPath + engineerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            objectCulling.Initialize(player, character);
        }

    }
}

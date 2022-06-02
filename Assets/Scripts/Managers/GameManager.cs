using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using EventCallbacksSystem;

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
    public static bool gameIsPaused = false;
    public static PrefabManager prefabManager;
    public static Character character;
    public static GameManager Instance { get { return instance; } }
    private static GameManager instance;

    public static GameStateManager gameStateManager;
    public GameObject loadScene;

    private bool IsMine { get { return photonView.IsMine; } }
    private bool gameIsOver = false;
   
    private bool loadSaveFile = false;
    
    private void Awake()
    {
        PrefabManager.LoadPrefabs();
        instance = this;
        gameStateManager = GetComponent<GameStateManager>();
        character = (Character)PlayerPrefs.GetInt(GlobalSettings.GameSettings.CharacterChoicePropertyName);
        Debug.Log($"Oh no, you chose the {character} charater");
        loadSaveFile = PlayerPrefs.GetInt(GlobalSettings.LoadSaveFileSettingName) == 1;
        Initialize();
        Destroy(loadScene, 5);
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

            if(!player.GetComponent<HealthHandler>().isAlive && !otherPlayer.GetComponent<HealthHandler>().isAlive && gameIsOver == false)
            {
                photonView.RPC(nameof(FireGameOverEvent), RpcTarget.All, "Both players died!");
                gameIsOver = true;
            }

            if(!ship.GetComponent<HealthHandler>().isAlive && gameIsOver == false)
            {
                photonView.RPC(nameof(FireGameOverEvent), RpcTarget.All, "Ship has been destroyed");
                gameIsOver = true;
            }
        }
    }

    [PunRPC]
    private void FireGameOverEvent(string text)
    {
        EventSystem.Instance.FireEvent(new GameOverEvent(text));
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

        // Sync data if save file has been loaded
        if (PhotonNetwork.IsMasterClient && gameStateManager.SaveExists() && loadSaveFile)
        {
            // Should send data for the other character role
            if (character == Character.SOLDIER)
            {
                gameStateManager.SyncOtherPlayerData(Character.ENGINEER);
            }
            else
            {
                gameStateManager.SyncOtherPlayerData(Character.SOLDIER);
            }
            // Send progress data
            gameStateManager.SyncProgressData();
        }
    }

    private void Initialize()
    {
        // Initialize save/load file manager
        gameStateManager.Initialize();
        // Store reference of other player when it has been created
        StartCoroutine(FindOtherPlayer(character));

        // Create some networked objects
        if (PhotonNetwork.IsMasterClient)
        {
            objectInstantiater.InitializeWorld();
        }
        // Instantiate the chosen player prefab
        if (character == Character.SOLDIER)
        {
            player = PhotonNetwork.Instantiate(GlobalSettings.PlayerCharacterPath + soldierPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            player = PhotonNetwork.Instantiate(GlobalSettings.PlayerCharacterPath + engineerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }
        // Load data from save file
        if (PhotonNetwork.IsMasterClient && gameStateManager.SaveExists() && loadSaveFile)
        {
            gameStateManager.LoadPlayerData(ref player, character);
            gameStateManager.LoadProgressData();
        }
        // Start enemy culling
        if (PhotonNetwork.IsMasterClient)
        {
            objectCulling.Initialize(player, character);
        }
        // Setting to allow GameObjects to receive RPCs when Time.timeScale = 0
        PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 0;
    }

    public void PauseGame(bool paused)
    {
        photonView.RPC(nameof(PauseGameRPC), RpcTarget.All, paused);
    }

    [PunRPC]
    public void PauseGameRPC(bool paused)
    {
        gameIsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
    }

    public class PrefabManager
    {
        public static GameObject stunEffectPrefab;
        private static bool hasLoadedPrefabs = false;

        public static void LoadPrefabs()
        {
            if (hasLoadedPrefabs == false)
            {
                System.Object[] prefabs = Resources.LoadAll(GlobalSettings.GameSettings.ParticleEffectPath, typeof(GameObject));

                if (prefabs.Length > 0)
                {
                    foreach (System.Object i in prefabs)
                    {
                        if (((GameObject)i).name == "P_hitImpact_stunGun")
                        {
                            stunEffectPrefab = (GameObject)i;
                        }
                    }
                }
                hasLoadedPrefabs = true;
            }
        }
    }
}

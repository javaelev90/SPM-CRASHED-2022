using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;
using System.Linq;
using Photon.Pun;

public class GameStateManager : MonoBehaviourPunCallbacks
{
    public static GameDataHolder gameDataHolder;
    private DataCollector dataCollector;
    private DataLoader dataLoader;

    private bool initialized = false;
    private bool receivedRequestedData = false;
    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (initialized == false)
        {
            InitializeGameDataHolder();
            dataCollector = new DataCollector(gameDataHolder);
            dataLoader = new DataLoader();
            LoadData();
            initialized = true;
        }
    }

    private void InitializeGameDataHolder()
    {
        gameDataHolder = new GameDataHolder();
        gameDataHolder.Initialize();
    }

    private void LoadData()
    {
        if (SaveAndLoadHelper.SaveFileExists())
        {
            gameDataHolder = SaveAndLoadHelper.LoadData();
            dataLoader.GameDataHolder = gameDataHolder;
        }
    }

    public void LoadPlayerData(ref GameObject player, Character character)
    {
        if (character == Character.ENGINEER)
        {
            dataLoader.LoadLocalPlayerData(ref player, dataLoader.GameDataHolder.engineerData);
        }
        else
        {
            dataLoader.LoadLocalPlayerData(ref player, dataLoader.GameDataHolder.soldierData);
        }
    }

    public void LoadProgressData()
    {
        dataLoader.LoadProgressData(gameDataHolder.progressData);
    }

    public void SaveGame()
    {
        InitializeGameDataHolder();
        gameDataHolder = dataCollector.CollectData();
        if (GameManager.otherPlayer != null)
        {
            CollectOtherPlayerData();
            StartCoroutine(WaitForRequestedData());
        }
        else
        {
            SaveAndLoadHelper.SaveData(gameDataHolder);
        }
    }

    IEnumerator WaitForRequestedData()
    {
        while(receivedRequestedData == false)
        {
            yield return new WaitForSeconds(0.2f);
        }
        SaveAndLoadHelper.SaveData(gameDataHolder);
    }

    public bool SaveExists()
    {
        return SaveAndLoadHelper.SaveFileExists();
    }

    public void SyncOtherPlayerData(Character otherCharacter)
    {
        string otherPlayerData = "";
        if (otherCharacter == Character.ENGINEER)
        {
            otherPlayerData = SaveAndLoadHelper.SerializeObjectToJson(gameDataHolder.engineerData);
        }
        else
        {
            otherPlayerData = SaveAndLoadHelper.SerializeObjectToJson(gameDataHolder.soldierData);
        }
        photonView.RPC(nameof(SyncPlayerDataRPC), RpcTarget.Others, otherPlayerData);
    }

    [PunRPC]
    private void SyncPlayerDataRPC(string playerDataJSON)
    {
        PlayerData playerData = SaveAndLoadHelper.LoadData<PlayerData>(playerDataJSON);
        dataLoader.LoadLocalPlayerData(ref GameManager.player, playerData);
    }

    public void SyncProgressData()
    {
        string progressDataJSON = SaveAndLoadHelper.SerializeObjectToJson(gameDataHolder.progressData);
        photonView.RPC(nameof(SyncProgressDataRPC), RpcTarget.Others, progressDataJSON);
    }

    [PunRPC]
    private void SyncProgressDataRPC(string progressDataJSON)
    {
        ProgressData progressData = SaveAndLoadHelper.LoadData<ProgressData>(progressDataJSON);
        dataLoader.LoadProgressData(progressData);
    }

    public void CollectOtherPlayerData()
    {
        receivedRequestedData = false;
        photonView.RPC(nameof(RequestPlayerDataRPC), RpcTarget.Others);
    }
    
    [PunRPC]
    private void RequestPlayerDataRPC()
    {
        if (GameManager.character == Character.SOLDIER)
        {
            SendPlayerDataRequest(gameDataHolder.soldierData);
        }
        else
        {
            SendPlayerDataRequest(gameDataHolder.engineerData);
        }
    }

    private void SendPlayerDataRequest(PlayerData playerData)
    {
        dataCollector.CollectPlayerData(playerData, GameManager.player, GameManager.character);
        photonView.RPC(nameof(ReceivePlayerDataRPC), RpcTarget.Others, SaveAndLoadHelper.SerializeObjectToJson(playerData));
    }

    [PunRPC]
    private void ReceivePlayerDataRPC(string playerDataJSON)
    {
        PlayerData playerData = SaveAndLoadHelper.LoadData<PlayerData>(playerDataJSON);
        if (playerData.character == Character.SOLDIER)
        {
            gameDataHolder.soldierData = playerData;
        }
        else
        {
            gameDataHolder.engineerData = playerData;
        }
        receivedRequestedData = true;
    }



}

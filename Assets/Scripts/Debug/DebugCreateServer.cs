using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class DebugCreateServer : MonoBehaviourPunCallbacks
{
    [SerializeField] private string customRoomName;
    [SerializeField] private GameObject customPlayerPrefab;
    [SerializeField] private Transform spawnPosition;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (customRoomName.Trim() == "")
        {
            customRoomName = "Room " + gameObject.GetInstanceID();
        }
        PhotonNetwork.GameVersion = "0.1";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.PlayerTtl = 50;
        roomOptions.PublishUserId = true;
        PhotonNetwork.JoinOrCreateRoom(customRoomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        SceneManager.sceneLoaded += InitLevel;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void InitLevel(Scene scene, LoadSceneMode mode)
    {
        if (customPlayerPrefab != null)
        {
            PhotonNetwork.Instantiate(GlobalSettings.PlayerCharacterPath + customPlayerPrefab.name, spawnPosition.position, spawnPosition.rotation);
        }
    }

}

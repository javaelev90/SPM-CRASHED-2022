using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField lobbyNameInput;
    [SerializeField] GameObject playerList;
    [SerializeField] GameObject lobbyPlayerPrefab;

    private ArrayList currentRoomList = new ArrayList();
    public Character PlayerChoice { get; set; }
    public bool IsMaster { get { return PhotonNetwork.IsMasterClient; } }
    // Joined server
    // Click character
    //  Send RPC, choose character
    //      receiving - disable character
    //      sending   - highlight selection

    void Start()
    {
        PhotonNetwork.GameVersion = "0.1";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        if (PhotonNetwork.LocalPlayer.NickName == "")
        {
            PhotonNetwork.LocalPlayer.NickName = "Player";
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public void JoinRoom()
    {
        if (lobbyNameInput.text.Length > 1)
        {
            PhotonNetwork.JoinRoom(lobbyNameInput.text);
        }
    }

    public void CreateRoom()
    {
        if (lobbyNameInput.text.Length > 1)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            roomOptions.PlayerTtl = 50;
            roomOptions.PublishUserId = true;

            PhotonNetwork.CreateRoom(lobbyNameInput.text, roomOptions, null, null);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room successfully!");
        if (IsMaster)
        {
            AddPlayer(PhotonNetwork.LocalPlayer);
        }
        else
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                AddPlayer(player);
            }
        }
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        AddPlayer(newPlayer);
    }

    private void AddPlayer(Player newPlayer)
    {
        Debug.Log($"User {newPlayer.UserId} connected");
        GameObject playerInfo = PhotonNetwork.Instantiate("Prefabs/UI/MenuLobby/" + lobbyPlayerPrefab.name, playerList.transform.position, Quaternion.identity);
        playerInfo.transform.SetParent(playerList.transform, false);
        playerInfo.GetComponent<LobbyPlayerMenuHandler>().SetPlayerName(newPlayer.NickName);
        playerInfo.GetComponent<LobbyPlayerMenuHandler>().LobbyPlayerId = newPlayer.UserId;
        currentRoomList.Add(playerInfo);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        RemovePlayer(otherPlayer);
    }

    private void RemovePlayer(Player otherPlayer)
    {
        foreach (GameObject item in currentRoomList)
        {
            if (item.GetComponent<LobbyPlayerMenuHandler>().LobbyPlayerId == otherPlayer.UserId)
            {
                Destroy(item);
                break;
            }
        }
    }

    private bool AreAllPlayersReady()
    {
        foreach (GameObject player in currentRoomList)
        {
            LobbyPlayerMenuHandler playerInfo = player.GetComponent<LobbyPlayerMenuHandler>();
            if (!playerInfo.IsReady)
            {
                return false;
            }
        }
        return true;
    }

    public void StartGame()
    {
        if (IsMaster && AreAllPlayersReady())
        {
            Debug.Log("Everyone is ready, starting game.");
            PhotonNetwork.LoadLevel("Game");
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

}

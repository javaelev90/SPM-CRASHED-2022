using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField lobbyNameInput;
    [SerializeField] GameObject playerList;
    [SerializeField] GameObject lobbyPlayerPrefab;

    private List<GameObject> currentRoomList = new List<GameObject>();

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

            PhotonNetwork.CreateRoom(lobbyNameInput.text, roomOptions, null, null);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room successfully!");
        if (PhotonNetwork.IsMasterClient)
        {
            AddPlayer(PhotonNetwork.LocalPlayer);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        AddPlayer(newPlayer);
    }

    private void AddPlayer(Player newPlayer)
    {
        GameObject playerInfo = Instantiate(lobbyPlayerPrefab, playerList.transform.position, Quaternion.identity);
        playerInfo.transform.parent = playerList.transform;
        playerInfo.GetComponent<LobbyPlayerMenuHandler>().SetPlayerName(newPlayer.NickName);
        playerInfo.GetComponent<LobbyPlayerMenuHandler>().LobbyPlayerId = newPlayer.ActorNumber;
        currentRoomList.Add(playerInfo);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        RemovePlayer(otherPlayer);
    }

    private void RemovePlayer(Player otherPlayer)
    {
        currentRoomList.RemoveAll(item => 
        {
            if(item.GetComponent<LobbyPlayerMenuHandler>().LobbyPlayerId == otherPlayer.ActorNumber)
            {
                Destroy(item);
                return true;
            }
            return false;
        });
    }


    public void StartGame()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

}
